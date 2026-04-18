using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{

    public class TaskTypeRepository : ITaskTypeRepository
    {
        private readonly AppDbContext _context;

        public TaskTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskType>> GetAllAsync( CancellationToken cancellationToken = default)
        {
            List<TaskType> types = await _context.TaskTypes
                .OrderBy(t => t.Name)
                .ToListAsync(cancellationToken);
            return types;
        }

        public async Task<TaskType?> GetByIdAsync(  Guid id, CancellationToken cancellationToken = default)
        {
            TaskType? type = await _context.TaskTypes
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            return type;
        }

        public async Task AddAsync( TaskType taskType, CancellationToken cancellationToken = default)
        {
            await _context.TaskTypes.AddAsync(taskType, cancellationToken);
        }

        public async Task SaveChangesAsync( CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
