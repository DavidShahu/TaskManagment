using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProjectMember
    {
        public Guid ProjectId { get;  set; }
        public Guid UserId { get;  set; }

        public DateTime JoinedAt { get;  set; }

        // navigation to user and project tables
        public User? User { get;  set; }
        public Project? Project { get;  set; }


    }
}
