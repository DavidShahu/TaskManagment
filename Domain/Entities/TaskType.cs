using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TaskType
    {
        private TaskType(Guid id, string name, string icon, string color)
        {
            Id = id;
            Name = name;
            Icon = icon;
            Color = color;
            IsActive = true;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string? Icon { get; private set; }  // emoji icon
        public string? Color { get; private set; } // hex color
        public bool IsActive { get; private set; }

         public ICollection<TaskItem> Tasks { get; private set; }
            = new List<TaskItem>();

        public static TaskType Create(string name, string icon, string color)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Task type name is required");


            return new TaskType(Guid.NewGuid(), name, icon, color);
        }

        public void Update(string name, string icon, string color)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Task type name is required");

            Name = name;
            Icon = icon;
            Color = color;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
