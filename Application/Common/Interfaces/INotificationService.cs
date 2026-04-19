using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface INotificationService
    {
        //to send the notifcation to the user
        Task SendAsync(Guid userId, string title, string message, string type, Guid? relatedEntityId = null, CancellationToken cancellationToken = default);
    }
}
