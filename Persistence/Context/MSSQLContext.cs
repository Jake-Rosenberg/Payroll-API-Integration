using Domain.Common;
using Domain.Entities.AD;
using Domain.Entities.UKG;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence.Common;

namespace Persistence.Context
{
    public interface IMSSQLContext
    {
        public DbSet<Employee> UKG_Employees { get; set; }
        public DbSet<D_LatePaycodeEdits> UKG_D_LatePunches { get; set; }
        public DbSet<ADProfile> AD_Employees { get; set; }
        public DbSet<HoursWorked> UKG_HoursWorked { get; set; }
        public DbSet<WorkedShift> UKG_WorkedShifts { get; set; }
        public DbSet<PaycodeEdit> UKG_PaycodeEdits { get; set; }
        public DbSet<ScheduleShift> UKG_ScheduleShifts { get; set; }
        Task TruncateTables();
    }

    public class MSSQLContext(IConfiguration configuration) : DbContext, IMSSQLContext
    {
        private readonly IConfiguration _config = configuration;

        public DbSet<Employee> UKG_Employees { get; set; }
        public DbSet<D_LatePaycodeEdits> UKG_D_LatePunches { get; set; } // need to rename to D_Late Exceptions
        public DbSet<ADProfile> AD_Employees { get; set; }
        public DbSet<HoursWorked> UKG_HoursWorked { get; set; }
        public DbSet<WorkedShift> UKG_WorkedShifts { get; set; }
        public DbSet<PaycodeEdit> UKG_PaycodeEdits { get; set; }
        public DbSet<ScheduleShift> UKG_ScheduleShifts { get; set; }

        // Overriding ServiceConfig to specify connection string
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringService.MSSQLConnectionString(_config);

            optionsBuilder
                .UseSqlServer(connectionString)
                .LogTo(Console.WriteLine, LogLevel.Information);

            optionsBuilder
                .UseSqlServer(connectionString, b => b.MigrationsAssembly("Persistence")
                .UseCompatibilityLevel(120)); // Added this to make steps 5 and 6 function

            optionsBuilder.EnableSensitiveDataLogging();

        }

        // Setting keys on model creation
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(p => p.PersonId);

                entity.Property(e => e.PersonId)
                    .ValueGeneratedNever();

                // TODO: this is apparently not necessary since it's defined below
                entity.HasMany(c => c.D_Lates)
                    .WithOne(e => e.Employee)
                    .HasForeignKey(e => e.PersonId)
                    .IsRequired();

            });

            modelBuilder.Entity<D_LatePaycodeEdits>(entity =>
            {
                entity.HasKey(p => p.TransactionId);

                entity.Property(e => e.TransactionId)
                    .ValueGeneratedNever();

                entity.HasOne(e => e.Employee)
                    .WithMany(c => c.D_Lates)
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ADProfile>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HoursWorked>(entity =>
            {
                entity.HasKey(h => h.HoursWorkedId);

                entity.Property(h => h.PersonId); // TODO: determine if this is needed

                entity.HasOne(h => h.Employee)
                    .WithOne(e => e.HoursWorked)
                    .HasForeignKey<HoursWorked>(h => h.PersonId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<WorkedShift>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.Property(p => p.Id)
                    .ValueGeneratedNever();

                entity.HasOne(h => h.ScheduleShift)
                    .WithOne(e => e.WorkedShift)
                    .HasForeignKey<WorkedShift>(j => j.ScheduledShiftId);

                entity.HasOne(h => h.Employee)
                    .WithMany(e => e.WorkedShift)
                    .HasForeignKey(j => j.PersonId)
                    .IsRequired();
            });

            modelBuilder.Entity<PaycodeEdit>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.Property(p => p.Id)
                    .ValueGeneratedNever();

                entity.HasOne(h => h.Employee)
                    .WithMany(e => e.PaycodeEdit)
                    .HasForeignKey(j => j.PersonId)
                    .IsRequired();
            });

            modelBuilder.Entity<ScheduleShift>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.Property(p => p.Id)
                    .ValueGeneratedNever();

                entity.HasOne(e => e.Employee)
                    .WithMany(e => e.ScheduleShift)
                    .HasForeignKey(j => j.PersonId)
                    .IsRequired();
            });
        }

        // This method sets the CreatedOn field for the tables
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.Now.ToUniversalTime();
                        break;
                    //case EntityState.Modified:
                    //    entry.Entity.LastUpdated = DateTime.Now.ToUniversalTime();
                    //    break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        //Truncate AD_Employees
        public async Task TruncateTables()
        {
            if (Database.GetPendingMigrations().Any())
            {
                // Apply any pending migrations to create the tables
                await Database.MigrateAsync();
            }

            if (Database.GetAppliedMigrations().Any())
            {

                AD_Employees.Where(ade => true).ExecuteDelete();

            }
        }
    }
}
