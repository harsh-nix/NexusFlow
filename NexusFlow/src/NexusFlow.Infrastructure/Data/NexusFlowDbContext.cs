using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Enums;
using TaskStatus = NexusFlow.Domain.Enums.TaskStatus;

namespace NexusFlow.Infrastructure.Data
{
    public class NexusFlowDbContext : DbContext
    {
        public NexusFlowDbContext(DbContextOptions<NexusFlowDbContext> options)
            : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<TaskAssignee> TaskAssignees { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).HasConversion<int>();

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // RefreshToken
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Organization
            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Organization)
                      .WithMany(o => o.Departments)
                      .HasForeignKey(e => e.OrganizationId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Team
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Department)
                      .WithMany(d => d.Teams)
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // TeamMember
            modelBuilder.Entity<TeamMember>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Team)
                      .WithMany(t => t.TeamMembers)
                      .HasForeignKey(e => e.TeamId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.TeamMembers)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Project
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.HasOne(e => e.Organization)
                      .WithMany(o => o.Projects)
                      .HasForeignKey(e => e.OrganizationId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // ProjectMember
            modelBuilder.Entity<ProjectMember>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Role).HasConversion<int>();
                entity.HasOne(e => e.Project)
                      .WithMany(p => p.ProjectMembers)
                      .HasForeignKey(e => e.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.ProjectMembers)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // ProjectTask
            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.Priority).HasConversion<int>();
                entity.Property(e => e.AssignmentNote).HasMaxLength(2000);
                entity.HasOne(e => e.Project)
                      .WithMany(p => p.Tasks)
                      .HasForeignKey(e => e.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // SubTask
            modelBuilder.Entity<SubTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.ParentTask)
                      .WithMany(t => t.SubTasks)
                      .HasForeignKey(e => e.ParentTaskId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TaskAssignee
            modelBuilder.Entity<TaskAssignee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Task)
                      .WithMany(t => t.Assignees)
                      .HasForeignKey(e => e.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Comment
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Type)
                      .HasConversion<int>()
                      .HasDefaultValue(CommentType.Comment);
                entity.HasOne(e => e.Task)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(e => e.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.RelatedTask)
                      .WithMany()
                      .HasForeignKey(e => e.RelatedTaskId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            });
        }

        // Auto-set CreatedAt and UpdatedAt on save
        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}