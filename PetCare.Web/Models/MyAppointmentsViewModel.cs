using PetCare.Data.Entities;

namespace PetCare.Web.Models;

public class MyAppointmentsViewModel
{
    public IReadOnlyList<Appointment> Appointments { get; init; } = [];

    public AppointmentStatus? Status { get; init; }

    public int Page { get; init; }

    public int TotalPages { get; init; }
}