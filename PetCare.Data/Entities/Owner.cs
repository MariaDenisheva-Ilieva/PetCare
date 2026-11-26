using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PetCare.Data.Entities;

[Index(nameof(UserId), IsUnique = true)]
public class Owner
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? UserId { get; set; }

    public IdentityUser? User { get; set; }

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}