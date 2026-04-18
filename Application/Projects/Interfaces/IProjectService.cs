

using Application.Projects.DTOs.ProjectDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Projects.Interfaces.Projects
{
    public interface IProjectService
    {
        Task<List<ProjectResponse>> GetAllAsync(bool includeDeactivated = false, CancellationToken cancellationToken = default);
        Task<List<ProjectResponse>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ProjectResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ProjectResponse> CreateAsync(CreateProjectRequest request, Guid createdByUserId, CancellationToken cancellationToken = default);
        Task<ProjectResponse> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
        Task RemoveMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
        Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
