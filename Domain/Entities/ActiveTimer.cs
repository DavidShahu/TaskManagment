using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ActiveTimer
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TaskId { get; set; }

        public DateTime StartedAt { get; set; }

         
        public User? User { get; set; }
        public TaskItem? Task { get; set; }

        public static ActiveTimer Create(Guid userId, Guid taskId)
        {
            return new ActiveTimer
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TaskId = taskId,
                StartedAt = DateTime.Now
            };
        }

        public double GetElapsedHours()
        {
            return Math.Round(
                (DateTime.Now - StartedAt).TotalHours, 2);
        }
    }
}
