using Domain.Events;
using Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Project : AggregateRoot
    {

        public string Name { get; private set; } 

        public string Description { get; private set; } 

        public Guid CreatedByUserId { get; private set; } 

        public DateTime CreatedAt { get; private set; }

        public bool IsActive { get; private set; }

        private Project (Guid id, string name, string description, Guid createdByUserId)  : base(id)
        {
            Name = name;
            Description = description;
            CreatedByUserId = createdByUserId;
            IsActive = true;
            CreatedAt = DateTime.Now;
        }


        public User? CreatedBy { get; private set; }
        public ICollection<ProjectMember> Members { get; private set; } = new List<ProjectMember>();
        public ICollection<TaskItem> Tasks { get; private set; }  = new List<TaskItem>();

        //creates new project
        public static Project Create( string name, string description, Guid createdByUserId)
        {
            if(string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required");

            if (name.Length > 100)
                throw new ArgumentException("Project name cannot be longer than 100 characters");

            return new Project (Guid.NewGuid(), name, description, createdByUserId);
        }


        //updates project
        public void Update (string name, string description)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Project name is required");

            if (name.Length > 100)
                throw new ArgumentException("Project name cannot be longer than 100 characters");

            Name = name;
            Description = description;
        }

        //deactivates a selected project (soft delte)
        public void Deactivete() => IsActive = false;

        //activates a selected project
        public void Activate () => IsActive = true;

        public void AddMember(Guid userId)
        {
            RaiseDomainEvent(new UserAddedToProject(
                Id, Name, userId));
        }
    }


}
