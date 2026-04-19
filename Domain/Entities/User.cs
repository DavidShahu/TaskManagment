using Domain.Enums;
using Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    //domain driven design 
    public class User : AggregateRoot
    {
        private User(
            Guid id,
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            UserRole role) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.Now;
            IsActive = true;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }


        //craete new user with the data from the register form if all data is valid
        public static User Create(
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            UserRole role = UserRole.User)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (!email.Contains('@'))
                throw new ArgumentException("Email is invalid");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password is required");

            return new User(
                Guid.NewGuid(),
                firstName,
                lastName,
                email.ToLower(),
                passwordHash,
                role);
        }


        //deactivate user
        public void Deactivate() => IsActive = false;
        //activat user if is inactive
        public void Activate() => IsActive = true;

        //password reset by admin
        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash is required");

            PasswordHash = newPasswordHash;
        }
    }
}
