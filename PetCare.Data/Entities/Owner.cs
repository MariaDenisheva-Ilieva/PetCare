namespace PetCare.Data.Entities;

public class Owner
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}