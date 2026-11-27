using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetCare.Data.Entities;

namespace PetCare.Data;

public class PetCareDbContext : IdentityDbContext
{
    public PetCareDbContext(DbContextOptions<PetCareDbContext> options)
        : base(options)
    {
    }

    public DbSet<Owner> Owners => Set<Owner>();

    public DbSet<Pet> Pets => Set<Pet>();

    public DbSet<Veterinarian> Veterinarians => Set<Veterinarian>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();

    public DbSet<Treatment> Treatments => Set<Treatment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MedicalRecord>()
            .HasOne(record => record.Appointment)
            .WithOne()
            .HasForeignKey<MedicalRecord>(record => record.AppointmentId);

        modelBuilder.Entity<Appointment>()
            .HasIndex(appointment => new
            {
                appointment.VeterinarianId,
                appointment.StartsAt
            })
            .IsUnique()
            .HasFilter("[Status] = 0");

        modelBuilder.Entity<Veterinarian>().HasData(
            new Veterinarian
            {
                Id = 1,
                FirstName = "Елена",
                LastName = "Иванова",
                Specialty = "Вътрешни болести"
            },
            new Veterinarian
            {
                Id = 2,
                FirstName = "Николай",
                LastName = "Петров",
                Specialty = "Хирургия"
            });
    }
}