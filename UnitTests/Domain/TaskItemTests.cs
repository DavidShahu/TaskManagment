using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

using TaskStatus = Domain.Enums.TaskStatus;
namespace UnitTests.Domain;

public class TaskItemTests
{
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldCreateTask()
    {
        var task = TaskItem.Create(
            "Test Task",
            "Description",
            DateTime.Now.AddDays(7),
            _userId,
            _userId);

        task.Should().NotBeNull();
        task.Title.Should().Be("Test Task");
        task.Status.Should().Be(TaskStatus.Open);
        task.LoggedHours.Should().Be(0);
        task.OwnerId.Should().Be(_userId);
        task.CreatedByUserId.Should().Be(_userId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_ShouldThrow(string title)
    {
        var act = () => TaskItem.Create(
            title, null, null, _userId, _userId);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title*");
    }

    [Fact]
    public void Create_WithPastDueDate_ShouldThrow()
    {
        var act = () => TaskItem.Create(
            "Task", null,
            DateTime.Now.AddDays(-1),
            _userId, _userId);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*future*");
    }

    [Fact]
    public void Create_WithNegativeEstimatedHours_ShouldThrow()
    {
        var act = () => TaskItem.Create(
            "Task", null, null,
            _userId, _userId, null, -1);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*greater than 0*");
    }

    [Fact]
    public void MarkAsDone_ShouldChangeStatus()
    {
        var task = TaskItem.Create(
            "Task", null, null, _userId, _userId);

        task.MarkAsDone(_userId);

        task.Status.Should().Be(TaskStatus.Done);
    }

    [Fact]
    public void MarkAsOpen_AfterDone_ShouldChangeStatus()
    {
        var task = TaskItem.Create(
            "Task", null, null, _userId, _userId);

        task.MarkAsDone(_userId);
        task.MarkAsOpen();

        task.Status.Should().Be(TaskStatus.Open);
    }

    [Fact]
    public void LogTime_ShouldIncreaseLoggedHours()
    {
        var task = TaskItem.Create(
            "Task", null, null, _userId, _userId,
            null, 10);

        task.LogTime(3);
        task.LogTime(2);

        task.LoggedHours.Should().Be(5);
    }

    [Fact]
    public void LogTime_WithZeroHours_ShouldThrow()
    {
        var task = TaskItem.Create(
            "Task", null, null, _userId, _userId);

        var act = () => task.LogTime(0);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*greater than 0*");
    }

    [Fact]
    public void LogTime_WithNegativeHours_ShouldThrow()
    {
        var task = TaskItem.Create(
            "Task", null, null, _userId, _userId);

        var act = () => task.LogTime(-1);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*greater than 0*");
    }

    [Fact]
    public void ProgressPercentage_WithEstimatedHours_ShouldCalculate()
    {
        var task = TaskItem.Create(
            "Task", null, null, _userId, _userId,
            null, 10);

        task.LogTime(5);

        task.ProgressPercentage.Should().Be(50);
    }

    [Fact]
    public void RemainingHours_ShouldDecrease_WhenTimeLogged()
    {
        var task = TaskItem.Create(
            "Task", null, null, _userId, _userId,
            null, 10);

        task.LogTime(3);

        task.RemainingHours.Should().Be(7);
    }

    [Fact]
    public void ProgressPercentage_WithNoEstimate_ShouldBeNull()
    {
        var task = TaskItem.Create(
            "Task", null, null, _userId, _userId);

        task.ProgressPercentage.Should().BeNull();
    }

    [Fact]
    public void IsAssignedByAdmin_WhenCreatedByDifferentUser_ShouldBeTrue()
    {
        var adminId = Guid.NewGuid();
        var task = TaskItem.Create(
            "Task", null, null,
            _userId,   // owner
            adminId);  // created by admin

        task.CreatedByUserId.Should().NotBe(task.OwnerId);
    }
}