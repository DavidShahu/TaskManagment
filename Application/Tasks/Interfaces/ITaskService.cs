using Application.Tasks.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tasks.Interfaces
{

    public interface ITaskService
    {
        Task<List<TaskResponse>> GetMyTasksAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<List<TaskResponse>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);

        Task<List<TaskResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TaskResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<TaskResponse> CreateAsync(CreateTaskRequest request, Guid ownerId, CancellationToken cancellationToken = default);

        Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, Guid requestingUserId, bool isAdmin, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, Guid requestingUserId, bool isAdmin, CancellationToken cancellationToken = default);

        Task MarkAsDoneAsync(Guid id, Guid requestingUserId, bool isAdmin, CancellationToken cancellationToken = default);

        Task MarkAsOpenAsync(Guid id, Guid requestingUserId, bool isAdmin, CancellationToken cancellationToken = default);

        Task<TaskResponse> LogTimeAsync(Guid taskId, Guid userId, LogTimeRequest request, CancellationToken cancellationToken = default);

        Task<TimerResponse?> GetActiveTimerAsync( Guid userId,  CancellationToken cancellationToken = default);

        Task<TimerResponse> StartTimerAsync(  Guid taskId, Guid userId, CancellationToken cancellationToken = default);

        Task<double> StopTimerAsync(  Guid userId, CancellationToken cancellationToken = default);
    }
}
