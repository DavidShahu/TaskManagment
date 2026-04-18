using Application.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserResponse> GetByIdAsync(Guid id,  CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
    }
}
