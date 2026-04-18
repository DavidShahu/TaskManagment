using Application.Common.Interfaces;
using Application.TaskTypes.DTOs;
using Application.TaskTypes.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TaskTypeService : ITaskTypeService
    {
        private readonly ITaskTypeRepository _repository;

        public TaskTypeService(ITaskTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaskTypeResponse>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var types = await _repository.GetAllAsync(cancellationToken);
            return types.Select(MapToResponse).ToList();
        }

        public async Task<TaskTypeResponse> CreateAsync(
            CreateTaskTypeRequest request,
            CancellationToken cancellationToken = default)
        {
            var taskType = TaskType.Create(
                request.Name, request.Icon, request.Color);

            await _repository.AddAsync(taskType, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return MapToResponse(taskType);
        }

        public async Task<TaskTypeResponse> UpdateAsync(
            Guid id,
            UpdateTaskTypeRequest request,
            CancellationToken cancellationToken = default)
        {
            var taskType = await _repository.GetByIdAsync(id, cancellationToken);

            if (taskType is null)
                throw new KeyNotFoundException("Task type not found");

            taskType.Update(request.Name, request.Icon, request.Color);
            await _repository.SaveChangesAsync(cancellationToken);

            return MapToResponse(taskType);
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var taskType = await _repository.GetByIdAsync(id, cancellationToken);

            if (taskType is null)
                throw new KeyNotFoundException("Task type not found");

            taskType.Deactivate();
            await _repository.SaveChangesAsync(cancellationToken);
        }

        public async Task ActivateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var taskType = await _repository.GetByIdAsync(id, cancellationToken);

            if (taskType is null)
                throw new KeyNotFoundException("Task type not found");

            taskType.Activate();
            await _repository.SaveChangesAsync(cancellationToken);
        }

        private static TaskTypeResponse MapToResponse(TaskType t) => new()
        {
            Id = t.Id,
            Name = t.Name,
            Icon = t.Icon,
            Color = t.Color,
            IsActive = t.IsActive
        };
    }
}
