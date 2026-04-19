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

    public class TaskMarkedAsDoneHandler
        : INotificationHandler<TaskMarkedAsDone>
    {
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public TaskMarkedAsDoneHandler(
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task Handle( TaskMarkedAsDone notification, CancellationToken cancellationToken)
        {
            var completedBy = await _userRepository
                .GetByIdAsync(
                    notification.CompletedByUserId,
                    cancellationToken);

            var name = completedBy is not null
                ? $"{completedBy.FirstName} {completedBy.LastName}"
                : "A user";

            await _notificationService.SendAsync(
                notification.CreatedByUserId,
                "Task Completed",
                $"\"{notification.TaskTitle}\" was completed by {name}",
                NotificationType.TaskCompleted,
                notification.TaskId,
                cancellationToken);
        }
    }
}
