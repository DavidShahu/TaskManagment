using Application.Common.Interfaces;
using Application.Users.DTOs;
using Application.Users.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public static UserResponse MapToResponse(User user) => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
        };


        public async Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            return users.Select(MapToResponse).ToList();
        }

        public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            return MapToResponse(user);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {

            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            user.Deactivate();
            await _userRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
        {

            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (isActive)
            {
                user.Activate();
            }
            else  
            {
                user.Deactivate();
            }

            await _userRepository.SaveChangesAsync(cancellationToken);
        }


    }
}
