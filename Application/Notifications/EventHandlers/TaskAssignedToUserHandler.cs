using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notifications.EventHandlers
{
    public class TaskAssignedToUserHandler : INotificationHandler<TaskAssignedToUser>
    {
        private readonly INotificationService _notificationService;

        public TaskAssignedToUserHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle( TaskAssignedToUser notification, CancellationToken cancellationToken)
        {
            await _notificationService.SendAsync(
                notification.AssignedToUserId,
                "New Task Assigned",
                $"You have been assigned: \"{notification.TaskTitle}\"",
                NotificationType.TaskAssigned,
                notification.TaskId,
                cancellationToken);
        }
    }
}
