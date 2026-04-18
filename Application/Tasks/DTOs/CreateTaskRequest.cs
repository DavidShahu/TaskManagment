using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tasks.DTOs
{
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? ProjectId { get; set; }

        // Admin can assign to another user => If null → assigned to requesting user
        public Guid? AssignedToUserId { get; set; }
        public decimal? EstimatedHours { get; set; }
        public Guid? TaskTypeId { get; set; }

    }
}
