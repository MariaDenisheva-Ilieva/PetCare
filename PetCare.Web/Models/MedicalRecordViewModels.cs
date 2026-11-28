using System.ComponentModel.DataAnnotations;

namespace PetCare.Web.Models;

public class MedicalRecordCreateViewModel
{
    public int AppointmentId { get; set; }

    [Required(ErrorMessage = "Въведете диагноза.")]
    [StringLength(200, ErrorMessage = "Диагнозата може да съдържа до 200 знака.")]
    [Display(Name = "Диагноза")]
    public string Diagnosis { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Бележките могат да съдържат до 1000 знака.")]
    [Display(Name = "Бележки")]
    public string? Notes { get; set; }
}

public class MedicalRecordVisitViewModel
{
    public int AppointmentId { get; init; }

    public DateTimeOffset StartsAt { get; init; }

    public string PetName { get; init; } = string.Empty;

    public string VeterinarianName { get; init; } = string.Empty;

    public string? Diagnosis { get; init; }

    public string? Notes { get; init; }
}