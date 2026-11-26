using System.ComponentModel.DataAnnotations;

namespace PetCare.Web.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Въведете имейл.")]
    [EmailAddress(ErrorMessage = "Въведете валиден имейл.")]
    [StringLength(256, ErrorMessage = "Имейлът може да съдържа до 256 знака.")]
    [Display(Name = "Имейл")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Въведете парола.")]
    [StringLength(128, MinimumLength = 6,
        ErrorMessage = "Паролата трябва да съдържа от 6 до 128 знака.")]
    [DataType(DataType.Password)]
    [Display(Name = "Парола")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Повторете паролата.")]
    [Compare(nameof(Password), ErrorMessage = "Двете пароли трябва да съвпадат.")]
    [DataType(DataType.Password)]
    [Display(Name = "Повторете паролата")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Въведете имейл.")]
    [EmailAddress(ErrorMessage = "Въведете валиден имейл.")]
    [StringLength(256, ErrorMessage = "Имейлът може да съдържа до 256 знака.")]
    [Display(Name = "Имейл")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Въведете парола.")]
    [StringLength(128, ErrorMessage = "Паролата може да съдържа до 128 знака.")]
    [DataType(DataType.Password)]
    [Display(Name = "Парола")]
    public string Password { get; set; } = string.Empty;
}