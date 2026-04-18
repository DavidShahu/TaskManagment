using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<List<Project>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
        { 
            List<Project>  projects = await _context.Projects
                .Include(p => p.CreatedBy).Include(p => p.Members).ThenInclude(p => p.User).Where(p=> includeInactive ||  p.IsActive == true).OrderByDescending(p=> p.CreatedAt).ToListAsync(cancellationToken);

            return projects;
        }

        public async Task<List<Project>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            List<Project> userProjects =await _context.Projects.Include(p => p.CreatedBy).Include(x=> x.Members).ThenInclude(pm=> pm.User).Where(x=> x.IsActive && x.Members.Any(m=> m.UserId == userId)).ToListAsync(cancellationToken);
            return userProjects;
        }

        public async Task<Project> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            Project project = await _context.Projects.Include(p => p.CreatedBy).Include(p => p.Members).ThenInclude(m => m.User).FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
            return project;
        }

        public async Task AddAsync(Project newProject, CancellationToken cancellationToken = default)  => await _context.Projects.AddAsync(newProject, cancellationToken);

        public async Task<bool> MemberExistsAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            bool memberAlreadyExists = await _context.ProjectMembers.AnyAsync(p => p.ProjectId == projectId && p.UserId == userId);
            return memberAlreadyExists;
        }
         
        public async Task AddMemberAsync(ProjectMember projectMember , CancellationToken cancellationToken = default) => _context.ProjectMembers.AddAsync(projectMember, cancellationToken);

        public async Task RemoveMemberAsync(  Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            var member = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm =>
                    pm.ProjectId == projectId &&
                    pm.UserId == userId, cancellationToken);

            if (member is not null)
                _context.ProjectMembers.Remove(member);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

    }
}
