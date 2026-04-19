using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public static class NotificationType
    {
        public const string TaskAssigned = "TaskAssigned";
        public const string TaskCompleted = "TaskCompleted";
        public const string AddedToProject = "AddedToProject";
        public const string RemovedFromProject = "RemovedFromProject";
    }
}
