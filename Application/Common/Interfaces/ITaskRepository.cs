using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

        Task<List<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);

        Task<List<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default);

        Task AddTimeLogAsync(TimeLog timeLog, CancellationToken cancellationToken = default);

        Task<ActiveTimer?> GetActiveTimerAsync(Guid userId, CancellationToken cancellationToken = default);

        Task AddTimerAsync(ActiveTimer timer, CancellationToken cancellationToken = default);

        Task RemoveTimerAsync(ActiveTimer timer, CancellationToken cancellationToken = default);
    }
}
