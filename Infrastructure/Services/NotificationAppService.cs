using Application.Common.Interfaces;
using Application.Notifications.DTOs;
using Application.Notifications.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{

    public class NotificationAppService : INotificationAppService
    {
        private readonly INotificationRepository _repository;

        public NotificationAppService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<NotificationResponse>> GetMyNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var notifications = await _repository.GetByUserIdAsync(userId, cancellationToken);

            return notifications.Select(n => new NotificationResponse
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                RelatedEntityId = n.RelatedEntityId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _repository.GetUnreadCountAsync(userId, cancellationToken);
        }

        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _repository.MarkAllAsReadAsync(userId, cancellationToken);
        }

        public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
        {
            var notification = await _repository.GetByIdAsync(notificationId, cancellationToken);

            if (notification is null)
                throw new KeyNotFoundException("Notification not found");

            notification.MarkAsRead();
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
