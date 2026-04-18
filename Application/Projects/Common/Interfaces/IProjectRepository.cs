using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{

    public interface IProjectRepository
    {
        Task<List<Project>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
        Task<List<Project>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Project project, CancellationToken cancellationToken = default);
        Task<bool> MemberExistsAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
        Task AddMemberAsync(ProjectMember member, CancellationToken cancellationToken = default);
        Task RemoveMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
