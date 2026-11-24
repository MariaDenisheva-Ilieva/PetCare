namespace PetCare.Data.Entities;

public class Appointment
{
    public int Id { get; set; }

    public DateTimeOffset StartsAt { get; set; }

    public string Reason { get; set; } = string.Empty;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public int PetId { get; set; }

    public Pet Pet { get; set; } = null!;

    public int VeterinarianId { get; set; }

    public Veterinarian Veterinarian { get; set; } = null!;
}