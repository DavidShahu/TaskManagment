using Application.Common.Interfaces;
using Application.Tasks.DTOs;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Services;
using Moq;
using TaskStatus = Domain.Enums.TaskStatus;

namespace UnitTests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IProjectRepository> _projectRepoMock;
    private readonly TaskService _taskService;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _adminId = Guid.NewGuid();

    public TaskServiceTests()
    {
        _taskRepoMock = new Mock<ITaskRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _projectRepoMock = new Mock<IProjectRepository>();
        _taskService = new TaskService(
            _taskRepoMock.Object,
            _userRepoMock.Object,
            _projectRepoMock.Object);
    }

    private User CreateUser(Guid id, string email = "test@test.com")
    {
        var user = User.Create("John", "Doe", email, "hash");
        return user;
    }

    private TaskItem CreateTask(
        Guid ownerId,
        Guid createdById,
        string title = "Test Task")
    {
        return TaskItem.Create(
            title,
            "Description",
            DateTime.Now.AddDays(7),
            ownerId,
            createdById);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnTaskResponse()
    {
        var user = CreateUser(_userId);
        var request = new CreateTaskRequest
        {
            Title = "Test Task",
            Description = "Description",
            DueDate = DateTime.Now.AddDays(7),
            EstimatedHours = 8
        };

        _userRepoMock.Setup(r =>
            r.GetByIdAsync(_userId, default))
            .ReturnsAsync(user);

        _taskRepoMock.Setup(r =>
            r.AddAsync(It.IsAny<TaskItem>(), default))
            .Returns(Task.CompletedTask);

        _taskRepoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(CreateTask(_userId, _userId, request.Title));

        // Act
        var result = await _taskService.CreateAsync(request, _userId);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task Create_WithNonExistentUser_ShouldThrow()
    {
        _userRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((User?)null);

        var act = async () => await _taskService.CreateAsync(
            new CreateTaskRequest { Title = "Test" }, _userId);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*user*");
    }

    [Fact]
    public async Task Create_AdminAssigningToUser_ShouldUseAssignedUserId()
    {
        var assignedUser = CreateUser(_userId, "user@test.com");
        var request = new CreateTaskRequest
        {
            Title = "Admin Task",
            AssignedToUserId = _userId // Admin assigning to user
        };

        _userRepoMock.Setup(r =>
            r.GetByIdAsync(_userId, default))
            .ReturnsAsync(assignedUser);

        _taskRepoMock.Setup(r =>
            r.AddAsync(It.IsAny<TaskItem>(), default))
            .Returns(Task.CompletedTask);

        _taskRepoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        var createdTask = CreateTask(_userId, _adminId, "Admin Task");
        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _taskService.CreateAsync(request, _adminId);

        // Assert
        result.Should().NotBeNull();
        _taskRepoMock.Verify(r =>
            r.AddAsync(It.Is<TaskItem>(t =>
                t.OwnerId == _userId), default),
            Times.Once);
    }


    [Fact]
    public async Task GetById_WithNonExistentId_ShouldThrow()
    {
        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((TaskItem?)null);

        var act = async () =>
            await _taskService.GetByIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Task not found*");
    }

    [Fact]
    public async Task GetMyTasks_ShouldReturnOnlyUserTasks()
    {
        var tasks = new List<TaskItem>
        {
            CreateTask(_userId, _userId, "Task 1"),
            CreateTask(_userId, _userId, "Task 2")
        };

        _taskRepoMock.Setup(r =>
            r.GetByOwnerIdAsync(_userId, default))
            .ReturnsAsync(tasks);

        var result = await _taskService.GetMyTasksAsync(_userId);

        result.Should().HaveCount(2);
    }


    [Fact]
    public async Task Update_AsOwner_ShouldSucceed()
    {
        var task = CreateTask(_userId, _userId);

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(task);

        _taskRepoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        var request = new UpdateTaskRequest
        {
            Title = "Updated Title",
            EstimatedHours = 5
        };

        var result = await _taskService
            .UpdateAsync(task.Id, request, _userId, false);

        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task Update_AsNonOwner_ShouldThrow()
    {
        var task = CreateTask(_userId, _userId);
        var otherUserId = Guid.NewGuid();

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(task);

        var act = async () => await _taskService
            .UpdateAsync(task.Id, new UpdateTaskRequest
            {
                Title = "Hacked"
            }, otherUserId, false);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Update_TaskAssignedByAdmin_UserCannotEdit()
    {
        // Task owned by user but created by admin
        var task = CreateTask(_userId, _adminId);

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(task);

        var act = async () => await _taskService
            .UpdateAsync(task.Id, new UpdateTaskRequest
            {
                Title = "Try edit"
            }, _userId, false);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*assigned*");
    }

    [Fact]
    public async Task Update_AsAdmin_ShouldAlwaysSucceed()
    {
        var task = CreateTask(_userId, _userId);

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(task);

        _taskRepoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        var result = await _taskService
            .UpdateAsync(task.Id, new UpdateTaskRequest
            {
                Title = "Admin Edit"
            }, _adminId, true); 

        result.Should().NotBeNull();
    }


    [Fact]
    public async Task Delete_AsOwner_ShouldSucceed()
    {
        var task = CreateTask(_userId, _userId);

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(task);

        _taskRepoMock.Setup(r =>
            r.DeleteAsync(task, default))
            .Returns(Task.CompletedTask);

        _taskRepoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        var act = async () => await _taskService
            .DeleteAsync(task.Id, _userId, false);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Delete_AsNonOwner_ShouldThrow()
    {
        var task = CreateTask(_userId, _userId);
        var otherUserId = Guid.NewGuid();

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(task);

        var act = async () => await _taskService
            .DeleteAsync(task.Id, otherUserId, false);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ShouldThrow()
    {
        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((TaskItem?)null);

        var act = async () => await _taskService
            .DeleteAsync(Guid.NewGuid(), _userId, false);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Task not found*");
    }


    [Fact]
    public async Task MarkAsDone_AsOwner_ShouldSucceed()
    {
        var task = CreateTask(_userId, _userId);

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(task);

        _taskRepoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        await _taskService.MarkAsDoneAsync(task.Id, _userId, false);

        task.Status.Should().Be(TaskStatus.Done);
    }

    [Fact]
    public async Task MarkAsOpen_AfterDone_ShouldSucceed()
    {
        var task = CreateTask(_userId, _userId);
        task.MarkAsDone(_userId);

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(task);

        _taskRepoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        await _taskService.MarkAsOpenAsync(task.Id, _userId, false);

        task.Status.Should().Be(TaskStatus.Open);
    }
     

    [Fact]
    public async Task LogTime_WithValidHours_ShouldUpdateLoggedHours()
    {
        var task = CreateTask(_userId, _userId);

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(task);

        _taskRepoMock.Setup(r =>
            r.AddTimeLogAsync(It.IsAny<TimeLog>(), default))
            .Returns(Task.CompletedTask);

        _taskRepoMock.Setup(r =>
            r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        await _taskService.LogTimeAsync(
            task.Id, _userId,
            new LogTimeRequest { Hours = 2, Note = "Fixed bug" });

        task.LoggedHours.Should().Be(2);
    }

    [Fact]
    public async Task LogTime_WithInvalidHours_ShouldThrow()
    {
        var task = CreateTask(_userId, _userId);

        _taskRepoMock.Setup(r =>
            r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(task);

        var act = async () => await _taskService.LogTimeAsync(
            task.Id, _userId,
            new LogTimeRequest { Hours = -1 });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*greater than 0*");
    }
}