﻿using Affiliate.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Affiliate.Application.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public virtual DbSet<User> Users { set; get; }
    public virtual DbSet<Role> Roles { set; get; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<TransactionPoint> TransactionPoints { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // it should be placed here, otherwise it will rewrite the following settings!
        base.OnModelCreating(builder);

        // Custom application mappings
        builder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(450).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Guid).IsRequired();
            entity.HasIndex(e => e.Guid).IsUnique();
            entity.Property(e => e.Password).IsRequired();
        });

        builder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Guid).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Guid).IsUnique();
        });

        builder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.RoleId);
            entity.Property(e => e.UserId);
            entity.Property(e => e.RoleId);
            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasForeignKey(d => d.RoleId).HasPrincipalKey(p=>p.Guid);
            entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasForeignKey(d => d.UserId).HasPrincipalKey(p=>p.Guid);
        });

        builder.Entity<Role>().HasData(
            new Role { Id = 1, Guid = Guid.NewGuid(), Name = CustomRoles.Master },
            new Role { Id = 2, Guid = Guid.NewGuid(), Name = CustomRoles.Admin },
            new Role { Id = 3, Guid = Guid.NewGuid(), Name = CustomRoles.Editor },
            new Role { Id = 4, Guid = Guid.NewGuid(), Name = CustomRoles.User }
        );
        
        //TransactionPoint
        builder.Entity<TransactionPoint>(entity =>
        {
            entity.Property(e => e.TransactionType).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Guid).IsRequired();
        });
    }
}