using Application.Common.Interfaces;
using Application.Tasks.DTOs;
using Application.Tasks.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{

    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;

        public TaskService(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }



        private static TaskResponse MapToResponse(TaskItem task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status.ToString(),
            CreatedAt = task.CreatedAt,
            OwnerId = task.OwnerId,
            OwnerName = task.Owner is not null
                ? $"{task.Owner.FirstName} {task.Owner.LastName}"
                : string.Empty,
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.Name,
            EstimatedHours = task.EstimatedHours,
            LoggedHours = task.LoggedHours,
            RemainingHours = task.RemainingHours,
            ProgressPercentage = task.ProgressPercentage,
            CreatedByUserId = task.CreatedByUserId,
            IsAssignedByAdmin = task.CreatedByUserId != task.OwnerId,
            TaskTypeId = task.TaskTypeId,
            TaskTypeName = task.TaskType?.Name,
            TaskTypeIcon = task.TaskType?.Icon,
            TaskTypeColor = task.TaskType?.Color,

            CreatedByName = task.CreatedBy is not null ? $"{task.CreatedBy.FirstName} {task.CreatedBy.LastName}" : string.Empty,

            TimeLogs = task.TimeLogs.Select(tl => new TimeLogResponse
            {
                Id = tl.Id,
                Hours = tl.Hours,
                Note = tl.Note,
                LoggedAt = tl.LoggedAt,
                UserId = tl.UserId,
                UserName = tl.User is not null
                    ? $"{tl.User.FirstName} {tl.User.LastName}"
                    : string.Empty
            }).OrderByDescending(tl => tl.LoggedAt).ToList()
        };

        public async Task<List<TaskResponse>> GetMyTasksAsync(  Guid userId, CancellationToken cancellationToken = default)
        {
            var tasks = await _taskRepository.GetByOwnerIdAsync(userId, cancellationToken);
            return tasks.Select(MapToResponse).ToList();
        }

        public async Task<List<TaskResponse>> GetByProjectAsync(  Guid projectId, CancellationToken cancellationToken = default)
        {
            var tasks = await _taskRepository.GetByProjectIdAsync(projectId, cancellationToken);
            return tasks.Select(MapToResponse).ToList();
        }

        public async Task<List<TaskResponse>> GetAllAsync( CancellationToken cancellationToken = default)
        {
            var tasks = await _taskRepository.GetAllAsync(cancellationToken);
            return tasks.Select(MapToResponse).ToList();
        }

        public async Task<TaskResponse> GetByIdAsync(  Guid id,  CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException("Task not found");

            return MapToResponse(task);
        }

        public async Task<TaskResponse> CreateAsync(  CreateTaskRequest request,  Guid requestingUserId, CancellationToken cancellationToken = default)
        {
            // Determine who owns the task
            // Admin can assign to another user via AssignedToUserId
            var ownerId = request.AssignedToUserId ?? requestingUserId;

            // Verify owner exists
            var owner = await _userRepository
                .GetByIdAsync(ownerId, cancellationToken);

            if (owner is null)
                throw new KeyNotFoundException("Assigned user not found");

            // Verify project exists if provided
            if (request.ProjectId.HasValue)
            {
                var project = await _projectRepository
                    .GetByIdAsync(request.ProjectId.Value, cancellationToken);

                if (project is null)
                    throw new KeyNotFoundException("Project not found");
            }
            Guid? taskTypeId = request.TaskTypeId == Guid.Empty ? null : request.TaskTypeId;

            var task = TaskItem.Create(
                request.Title,
                request.Description,
                request.DueDate,
                ownerId,
                requestingUserId,
                request.ProjectId,
                request.EstimatedHours,
                taskTypeId);

            await _taskRepository.AddAsync(task, cancellationToken);
            await _taskRepository.SaveChangesAsync(cancellationToken);

            var created = await _taskRepository
                .GetByIdAsync(task.Id, cancellationToken);

            return MapToResponse(created!);
        }

        public async Task<TaskResponse> UpdateAsync( Guid id, UpdateTaskRequest request,  Guid requestingUserId, bool isAdmin,  CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException("Task not found");

            // Only owner or admin can update
            if (!isAdmin && task.OwnerId != requestingUserId)
                throw new UnauthorizedAccessException(
                    "You can only update your own tasks");

            if (!isAdmin && task.CreatedByUserId != requestingUserId)
                throw new UnauthorizedAccessException(
                    "You cannot edit tasks assigned to you by an admin");
            Guid? taskTypeId = request.TaskTypeId == Guid.Empty ? null : request.TaskTypeId;

            task.Update(
                request.Title,
                request.Description,
                request.DueDate,
                request.EstimatedHours,
                taskTypeId);

            await _taskRepository.SaveChangesAsync(cancellationToken);
            return MapToResponse(task);
        }

        public async Task DeleteAsync( Guid id, Guid requestingUserId,  bool isAdmin, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException("Task not found");

            if (!isAdmin && task.OwnerId != requestingUserId)
                throw new UnauthorizedAccessException(
                    "You can only delete your own tasks");

            if (!isAdmin && task.CreatedByUserId != requestingUserId)
                throw new UnauthorizedAccessException(
                    "You cannot delete tasks assigned to you by an admin");

            await _taskRepository.DeleteAsync(task, cancellationToken);
            await _taskRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsDoneAsync(  Guid id, Guid requestingUserId, bool isAdmin,  CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException("Task not found");

            if (!isAdmin && task.OwnerId != requestingUserId)
                throw new UnauthorizedAccessException(
                    "You can only update your own tasks");

            task.MarkAsDone(requestingUserId);

            await _taskRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsOpenAsync(  Guid id,  Guid requestingUserId,  bool isAdmin,  CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException("Task not found");

            if (!isAdmin && task.OwnerId != requestingUserId)
                throw new UnauthorizedAccessException(
                    "You can only update your own tasks");

            task.MarkAsOpen();
            await _taskRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<TaskResponse> LogTimeAsync(  Guid taskId,  Guid userId,  LogTimeRequest request,  CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository
                .GetByIdAsync(taskId, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException("Task not found");

            // Create time log entry
            var timeLog = TimeLog.Create(taskId, userId, request.Hours, request.Note);

            // Update total logged hours on task
            task.LogTime(request.Hours);

            await _taskRepository.AddTimeLogAsync(timeLog, cancellationToken);
            await _taskRepository.SaveChangesAsync(cancellationToken);

            return MapToResponse(task);
        }




        public async Task<TimerResponse?> GetActiveTimerAsync(  Guid userId,  CancellationToken cancellationToken = default)
        {
            var timer = await _taskRepository
                .GetActiveTimerAsync(userId, cancellationToken);

            if (timer is null) return null;

            return MapTimerToResponse(timer);
        }

        public async Task<TimerResponse> StartTimerAsync( Guid taskId,  Guid userId,  CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository
                .GetByIdAsync(taskId, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException("Task not found");

            // Stop existing timer if any
            var existing = await _taskRepository
                .GetActiveTimerAsync(userId, cancellationToken);

            if (existing is not null)
                await _taskRepository.RemoveTimerAsync(existing, cancellationToken);

            var timer = ActiveTimer.Create(userId, taskId);
            await _taskRepository.AddTimerAsync(timer, cancellationToken);
            await _taskRepository.SaveChangesAsync(cancellationToken);

            // Reload with includes
            var created = await _taskRepository
                .GetActiveTimerAsync(userId, cancellationToken);

            return MapTimerToResponse(created!);
        }

        public async Task<double> StopTimerAsync(  Guid userId,  CancellationToken cancellationToken = default)
        {
            var timer = await _taskRepository
                .GetActiveTimerAsync(userId, cancellationToken);

            if (timer is null)
                throw new KeyNotFoundException("No active timer found");

            var hours = timer.GetElapsedHours();

            await _taskRepository.RemoveTimerAsync(timer, cancellationToken);
            await _taskRepository.SaveChangesAsync(cancellationToken);

            return hours;
        }

        private static TimerResponse MapTimerToResponse(ActiveTimer timer)
        {
            var elapsed = DateTime.Now - timer.StartedAt;
            return new TimerResponse
            {
                TaskId = timer.TaskId,
                TaskTitle = timer.Task?.Title ?? string.Empty,
                StartedAt = timer.StartedAt,
                ElapsedHours = timer.GetElapsedHours(),
                ElapsedFormatted = $"{(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}"
            };
        }

    }
}
