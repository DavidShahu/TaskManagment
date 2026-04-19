using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore;
using Domain.Primitives;
using MediatR; 

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {

        private readonly IMediator _mediator;

        public AppDbContext( DbContextOptions<AppDbContext> options,  IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        //adds the user entity
        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<TimeLog> TimeLogs => Set<TimeLog>();
        public DbSet<ActiveTimer> ActiveTimers => Set<ActiveTimer>();
        public DbSet<TaskType> TaskTypes => Set<TaskType>();
        public DbSet<Notification> Notifications => Set<Notification>();


        public override async Task<int> SaveChangesAsync(
       CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            // Publish domain events after save
            await PublishDomainEventsAsync(cancellationToken);

            return result;
        }

        private async Task PublishDomainEventsAsync(
            CancellationToken cancellationToken)
        {
            var aggregates = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var events = aggregates
                .SelectMany(a => a.DomainEvents)
                .ToList();

            aggregates.ForEach(a => a.ClearDomainEvents());

            foreach (var domainEvent in events)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);
        }

    }
}
