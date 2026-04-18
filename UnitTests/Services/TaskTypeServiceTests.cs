using Application.Common.Interfaces;
using Application.TaskTypes.DTOs;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Services;
using Moq;

namespace UnitTests.Services;

public class TaskTypeServiceTests
{
    private readonly Mock<ITaskTypeRepository> _repoMock;
    private readonly TaskTypeService _service;

    public TaskTypeServiceTests()
    {
        _repoMock = new Mock<ITaskTypeRepository>();
        _service = new TaskTypeService(_repoMock.Object);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnTaskTypeResponse()
    {
        _repoMock.Setup(r =>
            r.AddAsync(It.IsAny<TaskType>(), default))
            .Returns(Task.CompletedTask);

        _repoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        var request = new CreateTaskTypeRequest
        {
            Name = "Testing",
            Icon = "🧪",
            Color = "#667eea"
        };

        var result = await _service.CreateAsync(request);

        result.Should().NotBeNull();
        result.Name.Should().Be("Testing");
        result.Icon.Should().Be("🧪");
        result.Color.Should().Be("#667eea");
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllTypes()
    {
        var types = new List<TaskType>
        {
            TaskType.Create("Programming", "💻", "#667eea"),
            TaskType.Create("Design", "🎨", "#ed64a6"),
            TaskType.Create("Bug Fix", "🐛", "#e53e3e")
        };

        _repoMock.Setup(r =>
            r.GetAllAsync(default))
            .ReturnsAsync(types);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Name == "Programming");
    }

    [Fact]
    public async Task Update_WithNonExistentId_ShouldThrow()
    {
        _repoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((TaskType?)null);

        var act = async () => await _service.UpdateAsync(
            Guid.NewGuid(),
            new UpdateTaskTypeRequest
            {
                Name = "Updated",
                Icon = "🔥",
                Color = "#000000"
            });

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Task type not found*");
    }

    [Fact]
    public async Task Update_WithValidData_ShouldUpdateType()
    {
        var type = TaskType.Create("Old Name", "📋", "#000000");

        _repoMock.Setup(r =>
            r.GetByIdAsync(type.Id, default))
            .ReturnsAsync(type);

        _repoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(type.Id,
            new UpdateTaskTypeRequest
            {
                Name = "New Name",
                Icon = "🔥",
                Color = "#667eea"
            });

        result.Name.Should().Be("New Name");
        result.Icon.Should().Be("🔥");
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldDeactivate()
    {
        var type = TaskType.Create("Programming", "💻", "#667eea");

        _repoMock.Setup(r =>
            r.GetByIdAsync(type.Id, default))
            .ReturnsAsync(type);

        _repoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        await _service.DeleteAsync(type.Id);

        type.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Activate_WithValidId_ShouldActivate()
    {
        var type = TaskType.Create("Programming", "💻", "#667eea");
        type.Deactivate();

        _repoMock.Setup(r =>
            r.GetByIdAsync(type.Id, default))
            .ReturnsAsync(type);

        _repoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        await _service.ActivateAsync(type.Id);

        type.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ShouldThrow()
    {
        _repoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((TaskType?)null);

        var act = async () =>
            await _service.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Task type not found*");
    }
}