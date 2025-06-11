using Microsoft.EntityFrameworkCore;
using Tutorial10.Data.Models;

namespace Tutorial10.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();
    public DbSet<DeviceEmployee> DeviceEmployees => Set<DeviceEmployee>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DeviceEmployee>()
            .HasOne(de => de.Device)
            .WithMany(d => d.DeviceEmployees)
            .HasForeignKey(de => de.DeviceId);

        modelBuilder.Entity<DeviceEmployee>()
            .HasOne(de => de.Employee)
            .WithMany(e => e.DeviceEmployees)
            .HasForeignKey(de => de.EmployeeId);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Person)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PersonId);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Position)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PositionId);

        modelBuilder.Entity<Device>()
            .HasOne(d => d.DeviceType)
            .WithMany(dt => dt.Devices)
            .HasForeignKey(d => d.DeviceTypeId);
    }
}