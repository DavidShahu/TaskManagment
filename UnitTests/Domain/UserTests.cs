using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;


namespace UnitTests.Domain
{
    public class UserTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var firstName = "David";
            var lastName = "Shahu";
            var email = "david@test.com";
            var passwordHash = "hashedpassword123";

            // Act
            var user = User.Create(firstName, lastName, email, passwordHash);

            // Assert
            user.Should().NotBeNull();
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Email.Should().Be(email.ToLower());
            user.Role.Should().Be(UserRole.User);
            user.IsActive.Should().BeTrue();
            user.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void Create_WithAdminRole_ShouldCreateAdminUser()
        {
            // Act
            var user = User.Create(
                "Admin", "User",
                "admin@test.com",
                "hash",
                UserRole.Admin);

            // Assert
            user.Role.Should().Be(UserRole.Admin);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyFirstName_ShouldThrow(string firstName)
        {
            // Act
            var act = () => User.Create(
                firstName, "Shahu",
                "david@test.com", "hash");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*First name*");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyLastName_ShouldThrow(string lastName)
        {
            var act = () => User.Create(
                "David", lastName,
                "david@test.com", "hash");

            act.Should().Throw<ArgumentException>()
                .WithMessage("*Last name*");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyEmail_ShouldThrow(string email)
        {
            var act = () => User.Create(
                "David", "Shahu",
                email, "hash");

            act.Should().Throw<ArgumentException>()
                .WithMessage("*Email*");
        }

        [Fact]
        public void Create_WithInvalidEmail_ShouldThrow()
        {
            var act = () => User.Create(
                "David", "Shahu",
                "notanemail", "hash");

            act.Should().Throw<ArgumentException>()
                .WithMessage("*Email is invalid*");
        }

        [Fact]
        public void Create_ShouldLowercaseEmail()
        {
            var user = User.Create(
                "David", "Shahu",
                "DAVID@TEST.COM", "hash");

            user.Email.Should().Be("david@test.com");
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveFalse()
        {
            var user = User.Create(
                "David", "Shahu",
                "david@test.com", "hash");

            user.Deactivate();

            user.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Activate_ShouldSetIsActiveTrue()
        {
            var user = User.Create(
                "David", "Shahu",
                "david@test.com", "hash");

            user.Deactivate();
            user.Activate();

            user.IsActive.Should().BeTrue();
        }

    }
}
