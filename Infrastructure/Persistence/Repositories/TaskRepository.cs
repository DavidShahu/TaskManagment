using Application.Common.Interfaces;
using Application.Tasks.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{


    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
        {
            List<TaskItem> tasks = await _context.Tasks
                .Include(t => t.Owner)
                .Include(t => t.Project)
                .Include(t => t.TimeLogs).ThenInclude(tl => tl.User)
                .Include(t => t.TaskType)
                .Where(t => t.OwnerId == ownerId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
            return tasks;
        }

        public async Task<List<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            List<TaskItem> tasks = await _context.Tasks
                .Include(t => t.Owner)
                .Include(t => t.Project)
                .Include(t => t.TimeLogs).ThenInclude(tl => tl.User)
                .Include(t => t.TaskType)
                .Where(t => t.ProjectId == projectId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
            return tasks;
        }

        public async Task<List<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List<TaskItem> tasks = await _context.Tasks
                .Include(t => t.Owner)
                .Include(t => t.Project)
                .Include(t => t.TimeLogs).ThenInclude(tl => tl.User)
                .Include(t => t.TaskType)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
            return tasks;
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            TaskItem? task = await _context.Tasks
                .Include(t => t.Owner)
                .Include(t => t.Project)
                .Include(t => t.TimeLogs).ThenInclude(tl => tl.User)
                .Include(t => t.TaskType)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            return task;
        }

        public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            await _context.Tasks.AddAsync(task, cancellationToken);
        }

        public async Task AddTimeLogAsync(TimeLog timeLog, CancellationToken cancellationToken = default)
        {
            await _context.TimeLogs.AddAsync(timeLog, cancellationToken);
        }

        public async Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            _context.Tasks.Remove(task);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ActiveTimer?> GetActiveTimerAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            ActiveTimer? timer = await _context.ActiveTimers
                .Include(t => t.Task)
                .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);
            return timer;
        }

        public async Task AddTimerAsync(ActiveTimer timer, CancellationToken cancellationToken = default)
        {
            await _context.ActiveTimers.AddAsync(timer, cancellationToken);
        }

        public async Task RemoveTimerAsync(ActiveTimer timer, CancellationToken cancellationToken = default)
        {
            _context.ActiveTimers.Remove(timer);
        }
    }
}
