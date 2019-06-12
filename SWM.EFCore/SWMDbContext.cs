using SWM.Core.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SWM.Core.Accounts;
using SWM.Core;
using SWM.Core.Files;

namespace SWM.EFCore
{
    public class SWMDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<FileEntity> Files { get; set; }
        public DbSet<SharedFileEntity> SharedFiles { get; set; }

        public SWMDbContext(DbContextOptions<SWMDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AccountEntity>()
                 .HasMany(u => u.Users);
            modelBuilder.Entity<AccountEntity>()
                .HasData(new AccountEntity { Id = 1, Name = AccountTypes.Free.ToString(), Type = AccountTypes.Free, StorageSize = 10240, Price = 0 },
                         new AccountEntity { Id = 2, Name = AccountTypes.Premium.ToString(), Type = AccountTypes.Premium, StorageSize = 512000, Price = 250 });

            modelBuilder.Entity<SharedFileEntity>()
                .HasIndex(sf => new { sf.FileId, sf.UserId })
                .IsUnique();
            modelBuilder.Entity<SharedFileEntity>()
                .HasKey(sf => new { sf.FileId, sf.UserId });

            modelBuilder.Entity<SharedFileEntity>()
                .HasOne(bc => bc.File)
                .WithMany(b => b.UsersSharedFiles)
                .HasForeignKey(bc => bc.FileId);

            modelBuilder.Entity<SharedFileEntity>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.UsersSharedFiles)
                .HasForeignKey(bc => bc.UserId);

            modelBuilder.Entity<UserEntity>()
                .HasAlternateKey(u => u.Username);
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
