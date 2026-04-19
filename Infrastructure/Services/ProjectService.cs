using Application.Common.Interfaces;
using Application.Projects.DTOs.ProjectDTOs;
using Application.Projects.Interfaces.Projects;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public ProjectService(
            IProjectRepository projectRepository,
            IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }
        private static ProjectResponse MapToResponse(Project project) => new()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedByUserId = project.CreatedByUserId,
            CreatedByName = project.CreatedBy is not null
         ? $"{project.CreatedBy.FirstName} {project.CreatedBy.LastName}"
         : string.Empty,
            CreatedAt = project.CreatedAt,
            IsActive = project.IsActive,
            Members = project.Members.Select(m => new ProjectMemberResponse
            {
                UserId = m.UserId,
                FullName = m.User is not null
                    ? $"{m.User.FirstName} {m.User.LastName}"
                    : string.Empty,
                Email = m.User?.Email ?? string.Empty,
                JoinedAt = m.JoinedAt
            }).ToList()
        };

        //get all project (the damin will also get the ones that are deactivaed so they have the option to activate them again)
        public async Task<List<ProjectResponse>> GetAllAsync(bool includeDeactivated = false, CancellationToken cancellationToken = default)
        {
            var projects = await _projectRepository.GetAllAsync(includeDeactivated, cancellationToken);
            return projects.Select(MapToResponse).ToList() ?? new List<ProjectResponse>(); ;
        }

        //get all project a user is part of
        public async Task<List<ProjectResponse>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var projects = await _projectRepository.GetByUserIdAsync(userId, cancellationToken);
            return projects.Select(MapToResponse).ToList() ?? new List<ProjectResponse>();
        }

        // get a project by id
        public async Task<ProjectResponse> GetByIdAsync( Guid id, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, cancellationToken);

            if (project is null)
                throw new KeyNotFoundException("Project not found");

            return MapToResponse(project);
        }

        //creates a new project
        public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
        {
            var projectToAdd = Project.Create(
                    request.Name,
                    request.Description ?? null,
                    createdByUserId
                );

            await _projectRepository.AddAsync(projectToAdd, cancellationToken);
            await _projectRepository.SaveChangesAsync(cancellationToken);

            var createdProject = await _projectRepository.GetByIdAsync(projectToAdd.Id, cancellationToken);

            if (createdProject is null)
                throw new KeyNotFoundException("Project was not created");

            return MapToResponse(createdProject);
        }

        //updates a project
        public async Task<ProjectResponse> UpdateAsync( Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository .GetByIdAsync(id, cancellationToken);

            if (project is null)
                throw new KeyNotFoundException("Project not found");

            project.Update(request.Name, request.Description);
            await _projectRepository.SaveChangesAsync(cancellationToken);

            return MapToResponse(project);
        }

        //deactivates a project
        public async Task DeleteAsync( Guid id, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, cancellationToken);

            if (project is null)
                throw new KeyNotFoundException("Project not found");

            project.Deactivete();
            await _projectRepository.SaveChangesAsync(cancellationToken);
        }

        //adds a user to the project
        public async Task AddMemberAsync( Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository
                .GetByIdAsync(projectId, cancellationToken);

            if (project is null)
                throw new KeyNotFoundException("Project not found");

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user is null)
                throw new KeyNotFoundException("User not found");

            var alreadyMember = await _projectRepository.MemberExistsAsync(projectId, userId, cancellationToken);

            if (alreadyMember)
                throw new InvalidOperationException("User is already a member");

            var member = new ProjectMember
            {
                ProjectId = projectId,
                UserId = userId,
                JoinedAt = DateTime.Now
            };

            await _projectRepository.AddMemberAsync(member, cancellationToken);

            //raise domain event for addind a member to the project
            project.AddMember(userId);
            await _projectRepository.SaveChangesAsync(cancellationToken);
        }

        // deletes a user from the project 
        public async Task RemoveMemberAsync( Guid projectId,  Guid userId,  CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository
                .GetByIdAsync(projectId, cancellationToken);

            if (project is null)
                throw new KeyNotFoundException("Project not found");

            var isMember = await _projectRepository
                .MemberExistsAsync(projectId, userId, cancellationToken);

            if (!isMember)
                throw new KeyNotFoundException("User is not a member of this project");

            await _projectRepository
                .RemoveMemberAsync(projectId, userId, cancellationToken);
            await _projectRepository.SaveChangesAsync(cancellationToken);
        }

        // reactivaes an deactivated project
        public async Task ActivateAsync( Guid id, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, cancellationToken);

            if (project is null)
                throw new KeyNotFoundException("Project not found");

            project.Activate();
            await _projectRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
