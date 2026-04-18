using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{

    public interface ITaskTypeRepository
    {
        Task<List<TaskType>> GetAllAsync( CancellationToken cancellationToken = default);

        Task<TaskType?> GetByIdAsync(  Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(  TaskType taskType, CancellationToken cancellationToken = default);

        Task SaveChangesAsync( CancellationToken cancellationToken = default);
    }
}
