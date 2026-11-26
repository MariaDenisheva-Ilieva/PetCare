using System.ComponentModel.DataAnnotations;

namespace PetCare.Web.Models;

public class OwnerProfileViewModel
{
    [Required(ErrorMessage = "Въведете име.")]
    [StringLength(80, ErrorMessage = "Името може да съдържа до 80 знака.")]
    [Display(Name = "Име")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Въведете фамилия.")]
    [StringLength(80, ErrorMessage = "Фамилията може да съдържа до 80 знака.")]
    [Display(Name = "Фамилия")]
    public string LastName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Въведете валиден телефонен номер.")]
    [StringLength(30, ErrorMessage = "Телефонът може да съдържа до 30 знака.")]
    [Display(Name = "Телефон")]
    public string? PhoneNumber { get; set; }
}