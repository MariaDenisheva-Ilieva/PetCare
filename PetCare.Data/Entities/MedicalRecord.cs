namespace PetCare.Data.Entities;

public class MedicalRecord
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public Appointment Appointment { get; set; } = null!;

    public string Diagnosis { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}