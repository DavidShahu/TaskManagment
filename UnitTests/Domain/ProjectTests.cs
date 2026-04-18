using Domain.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Domain
{
    public class ProjectTests
    {
        private readonly Guid _userId = Guid.NewGuid();

        [Fact]
        public void Create_WithValidData_ShouldCreateProject()
        {
            var project = Project.Create(
                "Test Project",
                "Description",
                _userId);

            project.Should().NotBeNull();
            project.Name.Should().Be("Test Project");
            project.Description.Should().Be("Description");
            project.CreatedByUserId.Should().Be(_userId);
            project.IsActive.Should().BeTrue();
            project.Id.Should().NotBe(Guid.Empty);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyName_ShouldThrow(string name)
        {
            var act = () => Project.Create(name, null, _userId);

            act.Should().Throw<ArgumentException>()
                .WithMessage("*Project name*");
        }

        [Fact]
        public void Create_WithNameExceeding100Chars_ShouldThrow()
        {
            var longName = new string('a', 101);

            var act = () => Project.Create(longName, null, _userId);

            act.Should().Throw<ArgumentException>()
                .WithMessage("*100 characters*");
        }

        [Fact]
        public void Update_WithValidData_ShouldUpdateProject()
        {
            var project = Project.Create("Old Name", "Old Desc", _userId);

            project.Update("New Name", "New Desc");

            project.Name.Should().Be("New Name");
            project.Description.Should().Be("New Desc");
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveFalse()
        {
            var project = Project.Create("Test", null, _userId);

            project.Deactivete();

            project.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Activate_ShouldSetIsActiveTrue()
        {
            var project = Project.Create("Test", null, _userId);

            project.Deactivete();
            project.Activate();

            project.IsActive.Should().BeTrue();
        }
    }
}
