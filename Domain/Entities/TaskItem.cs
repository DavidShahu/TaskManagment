using Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
 
namespace Domain.Entities
{

    public class TaskItem : AggregateRoot
    {
        private TaskItem(
            Guid id,
            string title,
            string? description,
            DateTime? dueDate,
            Guid ownerId,
            Guid createdByUserId,
            Guid? projectId,
            decimal? estimatedHours,
            Guid? taskTypeId) : base(id)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            OwnerId = ownerId;
            CreatedByUserId = createdByUserId;
            ProjectId = projectId;
            EstimatedHours = estimatedHours;
            Status = Enums.TaskStatus.Open;
            CreatedAt = DateTime.Now;
            TaskTypeId = taskTypeId;
        }

        public string Title { get; private set; }
        public string? Description { get; private set; }
        public DateTime? DueDate { get; private set; }
        public Enums.TaskStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid OwnerId { get; private set; }
        public Guid CreatedByUserId { get; private set; }
        public Guid? ProjectId { get; private set; }

        // Time tracking
        public decimal? EstimatedHours { get; private set; }
        public decimal LoggedHours { get; private set; } = 0;
        public Guid? TaskTypeId { get; private set; }
         
        public User? Owner { get; private set; }
        public Project? Project { get; private set; }
        public ICollection<TimeLog> TimeLogs { get; private set; }
            = new List<TimeLog>();

        public TaskType TaskType { get; private set; }

        public static TaskItem Create(
            string title,
            string? description,
            DateTime? dueDate,
            Guid ownerId,
            Guid createdByUserId,
            Guid? projectId = null,
            decimal? estimatedHours = null,
            Guid? taskTypeId = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            if (title.Length > 200)
                throw new ArgumentException(
                    "Title cannot exceed 200 characters");

            if (dueDate.HasValue && dueDate.Value <= DateTime.Now)
                throw new ArgumentException(
                    "Due date must be in the future");

            if (estimatedHours.HasValue && estimatedHours.Value <= 0)
                throw new ArgumentException(
                    "Estimated hours must be greater than 0");

            return new TaskItem(
                Guid.NewGuid(),
                title,
                description,
                dueDate,
                ownerId,
                createdByUserId,
                projectId,
                estimatedHours,
                taskTypeId);
        }

        public void Update(
            string title,
            string? description,
            DateTime? dueDate,
            decimal? estimatedHours,
            Guid? taskTypeId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            if (title.Length > 200)
                throw new ArgumentException(
                    "Title cannot exceed 200 characters");

            if (dueDate.HasValue && dueDate.Value <= DateTime.Now)
                throw new ArgumentException(
                    "Due date must be in the future");

            if (estimatedHours.HasValue && estimatedHours.Value <= 0)
                throw new ArgumentException(
                    "Estimated hours must be greater than 0");

            Title = title;
            Description = description;
            DueDate = dueDate;
            EstimatedHours = estimatedHours;
            TaskTypeId = taskTypeId;
        }

        public void LogTime(decimal hours)
        {
            if (hours <= 0)
                throw new ArgumentException(
                    "Hours logged must be greater than 0");

            LoggedHours += hours;
        }

        public decimal? RemainingHours => EstimatedHours.HasValue
            ? Math.Max(0, EstimatedHours.Value - LoggedHours)
            : null;

        public decimal? ProgressPercentage => EstimatedHours.HasValue &&
            EstimatedHours.Value > 0
            ? Math.Min(100, (LoggedHours / EstimatedHours.Value) * 100)
            : null;

        public void MarkAsDone() => Status = Enums.TaskStatus.Done;
        public void MarkAsOpen() => Status = Enums.TaskStatus.Open;
    }
}
