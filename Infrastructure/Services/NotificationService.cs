using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Notifications.DTOs;
using Domain.Entities;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;


namespace Infrastructure.Services
{

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            INotificationRepository repository,
            IHubContext<NotificationHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
        }

        public async Task SendAsync(
            Guid userId,
            string title,
            string message,
            string type,
            Guid? relatedEntityId = null,
            CancellationToken cancellationToken = default)
        {
            // Save to DB
            var notification = Notification.Create(
                userId, title, message, type, relatedEntityId);

            await _repository.AddAsync(notification, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            // Push via SignalR
            await _hubContext.Clients
                .Group(userId.ToString())
                .SendAsync("ReceiveNotification", new NotificationResponse
                {
                    Id = notification.Id,
                    Title = notification.Title,
                    Message = notification.Message,
                    Type = notification.Type,
                    RelatedEntityId = notification.RelatedEntityId,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt
                }, cancellationToken);
        }
    }
}
