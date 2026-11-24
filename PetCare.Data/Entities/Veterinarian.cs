namespace PetCare.Data.Entities;

public class Veterinarian
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Specialty { get; set; }
}