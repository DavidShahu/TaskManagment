using Application.Auth.DTOs;
using Application.Auth.Interfaces;
using Application.Common.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {

        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }


        //register new user with the data from the register form
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            // check if email is unique 
            if (await _userRepository.EmailExistsAsync(request.Email ,cancellationToken))
                throw new InvalidOperationException("Email already exists");

            // Validate password length
            if (request.Password.Length < 8)
                throw new ArgumentException(
                    "Password must be at least 8 characters");

            // hash password so we dont save a plain string
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // create user (in the entity)
            var user = User.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                passwordHash);

            // save
            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            // generate token
            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            // find user with eamil
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
                throw new UnauthorizedAccessException("Invalid credentials");

            // check if active
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated");

            // verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            // generate token
            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

    }
}
