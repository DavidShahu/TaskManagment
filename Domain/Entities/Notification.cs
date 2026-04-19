using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification
    {


        private Notification(
            Guid id,
            Guid userId,
            string title,
            string message,
            string type,
            Guid? relatedEntityId = null
        )
        {
            Id = id;
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            RelatedEntityId = relatedEntityId;
            IsRead = false;
            CreatedAt = DateTime.Now;
        }


        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string Type { get; private set; }
        public Guid? RelatedEntityId { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public User? User { get; private set; }



        public static Notification Create(
            Guid userId,
            string title,
            string message,
            string type,
            Guid? relatedEntityId = null
        )
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required");

            return new Notification(
                Guid.NewGuid(),
                userId,
                title,
                message,
                type,
                relatedEntityId);
        }

        public void MarkAsRead() => IsRead = true;
    }
}
