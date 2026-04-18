using Application.Common.Interfaces;
using Application.Projects.DTOs;
using Application.Projects.DTOs.ProjectDTOs;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Services;
using Moq;

namespace UnitTests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly ProjectService _projectService;
    private readonly Guid _adminId = Guid.NewGuid();

    public ProjectServiceTests()
    {
        _projectRepoMock = new Mock<IProjectRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _projectService = new ProjectService(
            _projectRepoMock.Object,
            _userRepoMock.Object);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnProjectResponse()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            Name = "Test Project",
            Description = "Test Description"
        };

        var admin = User.Create(
            "Admin", "User",
            "admin@test.com", "hash",
            UserRole.Admin);

        _userRepoMock.Setup(r =>
            r.GetByIdAsync(_adminId, default))
            .ReturnsAsync(admin);

        _projectRepoMock.Setup(r =>
            r.AddAsync(It.IsAny<Project>(), default))
            .Returns(Task.CompletedTask);

        _projectRepoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        _projectRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Guid id, CancellationToken ct) =>
            {
                var p = Project.Create(
                    request.Name,
                    request.Description,
                    _adminId);
                return p;
            });

        // Act
        var result = await _projectService
            .CreateAsync(request, _adminId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ShouldThrow()
    {
        _projectRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Project?)null);

        var act = async () =>
            await _projectService.GetByIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Project not found*");
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ShouldThrow()
    {
        _projectRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Project?)null);

        var act = async () =>
            await _projectService.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Project not found*");
    }

    [Fact]
    public async Task AddMember_WhenAlreadyMember_ShouldThrow()
    {
        var project = Project.Create("Test", null, _adminId);
        var userId = Guid.NewGuid();
        var user = User.Create(
            "John", "Doe",
            "john@test.com", "hash");

        _projectRepoMock.Setup(r =>
            r.GetByIdAsync(project.Id, default))
            .ReturnsAsync(project);

        _userRepoMock.Setup(r =>
            r.GetByIdAsync(userId, default))
            .ReturnsAsync(user);

        _projectRepoMock.Setup(r =>
            r.MemberExistsAsync(project.Id, userId, default))
            .ReturnsAsync(true);

        var act = async () =>
            await _projectService.AddMemberAsync(project.Id, userId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already a member*");
    }
}