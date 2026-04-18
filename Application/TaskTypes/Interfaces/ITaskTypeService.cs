using Application.TaskTypes.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TaskTypes.Interfaces
{
    public interface ITaskTypeService
    {
        Task<List<TaskTypeResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TaskTypeResponse> CreateAsync(CreateTaskTypeRequest request, CancellationToken cancellationToken = default);

        Task<TaskTypeResponse> UpdateAsync(Guid id, UpdateTaskTypeRequest request, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
