using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    public class TimeLog
    {
        private TimeLog(
            Guid id,
            Guid taskId,
            Guid userId,
            decimal hours,
            string? note,
            DateTime loggedAt)
        {
            Id = id;
            TaskId = taskId;
            UserId = userId;
            Hours = hours;
            Note = note;
            LoggedAt = loggedAt;
        }

        public Guid Id { get; private set; }
        public Guid TaskId { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Hours { get; private set; }
        public string? Note { get; private set; }
        public DateTime LoggedAt { get; private set; }

        // Navigation properties
        public TaskItem? Task { get; private set; }
        public User? User { get; private set; }

        public static TimeLog Create(
            Guid taskId,
            Guid userId,
            decimal hours,
            string? note = null)
        {
            if (hours <= 0)
                throw new ArgumentException(
                    "Hours must be greater than 0");

            if (hours > 24)
                throw new ArgumentException(
                    "Cannot log more than 24 hours at once");

            return new TimeLog(
                Guid.NewGuid(),
                taskId,
                userId,
                hours,
                note,
                DateTime.Now);
        }
    }
}
