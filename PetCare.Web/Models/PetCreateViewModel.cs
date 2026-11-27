using System.ComponentModel.DataAnnotations;

namespace PetCare.Web.Models;

public class PetCreateViewModel
{
    [Required(ErrorMessage = "Въведете име на любимеца.")]
    [StringLength(80, ErrorMessage = "Името може да съдържа до 80 знака.")]
    [Display(Name = "Име")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Въведете вид на животното.")]
    [StringLength(50, ErrorMessage = "Видът може да съдържа до 50 знака.")]
    [Display(Name = "Вид")]
    public string Species { get; set; } = string.Empty;

    [StringLength(80, ErrorMessage = "Породата може да съдържа до 80 знака.")]
    [Display(Name = "Порода")]
    public string? Breed { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Дата на раждане")]
    public DateOnly? BirthDate { get; set; }
}