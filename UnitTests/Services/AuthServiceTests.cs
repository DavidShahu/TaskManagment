using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Auth.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Services;
using Moq;

namespace UnitTests.Services
{
    public class AuthServiceTests
    {

        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _authService = new AuthService(
                _userRepoMock.Object,
                _jwtServiceMock.Object);
        }

        // REGISTER TESTS
        [Fact]
        public async Task Register_WithValidData_ShouldReturnAuthResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FirstName = "David",
                LastName = "Shahu",
                Email = "david@test.com",
                Password = "Password12!"
            };

            _userRepoMock.Setup(r =>
                r.EmailExistsAsync(request.Email, default))
                .ReturnsAsync(false);

            _userRepoMock.Setup(r =>
                r.AddAsync(It.IsAny<User>(), default))
                .Returns(Task.CompletedTask);

            _userRepoMock.Setup(r =>
                r.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _jwtServiceMock.Setup(j =>
                j.GenerateToken(It.IsAny<User>()))
                .Returns("fake-jwt-token");

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be("fake-jwt-token");
            result.Email.Should().Be(request.Email.ToLower());
            result.Role.Should().Be(UserRole.User.ToString());
        }

        [Fact]
        public async Task Register_WithExistingEmail_ShouldThrow()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FirstName = "David",
                LastName = "Shahu",
                Email = "existing@test.com",
                Password = "Password123"
            };

            _userRepoMock.Setup(r =>
                r.EmailExistsAsync(request.Email, default))
                .ReturnsAsync(true);

            // Act
            var act = async () =>
                await _authService.RegisterAsync(request);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Email already exists*");
        }

        [Fact]
        public async Task Register_WithShortPassword_ShouldThrow()
        {
            var request = new RegisterRequest
            {
                FirstName = "David",
                LastName = "Shahu",
                Email = "david@test.com",
                Password = "short"
            };

            _userRepoMock.Setup(r =>
                r.EmailExistsAsync(request.Email, default))
                .ReturnsAsync(false);

            var act = async () =>
                await _authService.RegisterAsync(request);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*8 characters*");
        }

        // LOGIN TESTS
        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnAuthResponse()
        {
            // Arrange
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
            var user = User.Create(
                "David", "Shahu",
                "david@test.com",
                passwordHash);

            _userRepoMock.Setup(r =>
                r.GetByEmailAsync("david@test.com", default))
                .ReturnsAsync(user);

            _jwtServiceMock.Setup(j =>
                j.GenerateToken(user))
                .Returns("fake-jwt-token");

            var request = new LoginRequest
            {
                Email = "david@test.com",
                Password = "Password123"
            };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be("fake-jwt-token");
        }

        [Fact]
        public async Task Login_WithWrongPassword_ShouldThrow()
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
            var user = User.Create(
                "David", "Shahu",
                "david@test.com",
                passwordHash);

            _userRepoMock.Setup(r =>
                r.GetByEmailAsync("david@test.com", default))
                .ReturnsAsync(user);

            var request = new LoginRequest
            {
                Email = "david@test.com",
                Password = "WrongPassword"
            };

            var act = async () =>
                await _authService.LoginAsync(request);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Invalid credentials*");
        }

        [Fact]
        public async Task Login_WithNonExistentEmail_ShouldThrow()
        {
            _userRepoMock.Setup(r =>
                r.GetByEmailAsync("notfound@test.com", default))
                .ReturnsAsync((User?)null);

            var request = new LoginRequest
            {
                Email = "notfound@test.com",
                Password = "Password123"
            };

            var act = async () =>
                await _authService.LoginAsync(request);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Invalid credentials*");
        }

        [Fact]
        public async Task Login_WithDeactivatedUser_ShouldThrow()
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
            var user = User.Create(
                "David", "Shahu",
                "david@test.com",
                passwordHash);
            user.Deactivate();

            _userRepoMock.Setup(r =>
                r.GetByEmailAsync("david@test.com", default))
                .ReturnsAsync(user);

            var request = new LoginRequest
            {
                Email = "david@test.com",
                Password = "Password123"
            };

            var act = async () =>
                await _authService.LoginAsync(request);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*deactivated*");
        }
    }
}
