namespace PetCare.Data.Entities;

public class Treatment
{
    public int Id { get; set; }

    public int MedicalRecordId { get; set; }

    public MedicalRecord MedicalRecord { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string? Instructions { get; set; }
}