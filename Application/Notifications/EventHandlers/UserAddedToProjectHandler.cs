using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using MediatR;


namespace Application.Notifications.EventHandlers
{
    public class UserAddedToProjectHandler : INotificationHandler<UserAddedToProject>
    {
        private readonly INotificationService _notificationService;

        public UserAddedToProjectHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(
            UserAddedToProject notification,
            CancellationToken cancellationToken)
        {
            await _notificationService.SendAsync(
                notification.UserId,
                "Added to Project",
                $"You have been added to project \"{notification.ProjectName}\"",
                NotificationType.AddedToProject,
                notification.ProjectId,
                cancellationToken);
        }
    }
}
