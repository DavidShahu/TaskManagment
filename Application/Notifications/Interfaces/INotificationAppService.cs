using Application.Notifications.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notifications.Interfaces
{
    public interface INotificationAppService
    {
        Task<List<NotificationResponse>> GetMyNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);

        Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);

        Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    }
}
