using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PetCare.Data.Entities;

namespace PetCare.Web.Models;

public class TreatmentViewModel
{
    public int AppointmentId { get; set; }

    [Required(ErrorMessage = "Въведете наименование на лечението.")]
    [StringLength(120, ErrorMessage = "Наименованието може да съдържа до 120 знака.")]
    [Display(Name = "Наименование на лечението")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Указанията могат да съдържат до 1000 знака.")]
    [Display(Name = "Указания")]
    public string? Instructions { get; set; }

    [BindNever]
    public string PetName { get; set; } = string.Empty;

    [BindNever]
    public IReadOnlyList<Treatment> Treatments { get; set; } = [];
}