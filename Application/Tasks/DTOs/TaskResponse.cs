using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tasks.DTOs
{
    public class TaskResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public Guid? ProjectId { get; set; }
        public string? ProjectName { get; set; }

        // Time tracking
        public decimal? EstimatedHours { get; set; }
        public decimal LoggedHours { get; set; }
        public decimal? RemainingHours { get; set; }
        public decimal? ProgressPercentage { get; set; }

        public List<TimeLogResponse> TimeLogs { get; set; } = new();

        public Guid CreatedByUserId { get; set; }
        public bool IsAssignedByAdmin { get; set; }

        public Guid? TaskTypeId { get; set; }
        public string? TaskTypeName { get; set; }
        public string? TaskTypeIcon { get; set; }
        public string? TaskTypeColor { get; set; }

        public bool IsOverdue => DueDate.HasValue &&
            DueDate.Value.Date < DateTime.Now.Date &&
            Status == "Open";

        public string CreatedByName { get; set; } = string.Empty;

    }

    public class TimeLogResponse
    {
        public Guid Id { get; set; }
        public decimal Hours { get; set; }
        public string? Note { get; set; }
        public DateTime LoggedAt { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
