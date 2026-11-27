using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetCare.Web.Models;

public class AppointmentCreateViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Изберете ветеринар.")]
    public int VeterinarianId { get; set; }

    public string VeterinarianName { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Изберете любимец.")]
    [Display(Name = "Домашен любимец")]
    public int PetId { get; set; }

    [Required(ErrorMessage = "Изберете дата и час.")]
    [Display(Name = "Дата и час")]
    public DateTime? LocalStart { get; set; }

    [Required(ErrorMessage = "Въведете причина за посещението.")]
    [StringLength(300, ErrorMessage = "Причината може да съдържа до 300 знака.")]
    [Display(Name = "Причина за посещението")]
    public string Reason { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> Pets { get; set; } = [];
}