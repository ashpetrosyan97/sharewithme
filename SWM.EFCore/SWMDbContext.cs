using Microsoft.EntityFrameworkCore;
using SWM.Core.Accounts;
using SWM.Core.Users;

namespace SWM.EFCore
{
    public class SWMDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        //public DbSet<FileEntity> Files { get; set; }
       // public DbSet<SharedFileEntity> SharedFiles { get; set; }

        public SWMDbContext(DbContextOptions<SWMDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>()
                 .HasMany(u => u.Users);
            modelBuilder.Entity<Account>()
                .HasData(new Account { Id = 1, Name = AccountTypes.Free.ToString(), Type = AccountTypes.Free, StorageSize = 10240, Price = 0 },
                         new Account { Id = 2, Name = AccountTypes.Premium.ToString(), Type = AccountTypes.Premium, StorageSize = 512000, Price = 250 });

            /*modelBuilder.Entity<SharedFileEntity>()
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
                .HasForeignKey(bc => bc.UserId);*/

            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Username);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
