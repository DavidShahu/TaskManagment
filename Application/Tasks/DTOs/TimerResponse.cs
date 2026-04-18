using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tasks.DTOs
{
    public class TimerResponse
    {
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public double ElapsedHours { get; set; }
        public string ElapsedFormatted { get; set; } = string.Empty;
    }
}
