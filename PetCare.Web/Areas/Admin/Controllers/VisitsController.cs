using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Data.Entities;

namespace PetCare.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class VisitsController : Controller
{
    private readonly PetCareDbContext dbContext;

    public VisitsController(PetCareDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var appointments = await dbContext.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Pet)
            .ThenInclude(pet => pet.Owner)
            .Include(appointment => appointment.Veterinarian)
            .OrderByDescending(appointment => appointment.StartsAt)
            .ThenByDescending(appointment => appointment.Id)
            .ToListAsync();

        return View(appointments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var now = DateTimeOffset.UtcNow;

        var updatedRows = await dbContext.Appointments
            .Where(appointment =>
                appointment.Id == id &&
                appointment.Status == AppointmentStatus.Scheduled &&
                appointment.StartsAt <= now)
            .ExecuteUpdateAsync(update => update.SetProperty(
                appointment => appointment.Status,
                AppointmentStatus.Completed));

        TempData["VisitMessage"] = updatedRows == 1
            ? "Прегледът е отбелязан като проведен."
            : "Този преглед не може да бъде отбелязан като проведен.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var now = DateTimeOffset.UtcNow;

        var updatedRows = await dbContext.Appointments
            .Where(appointment =>
                appointment.Id == id &&
                appointment.Status == AppointmentStatus.Scheduled &&
                appointment.StartsAt > now)
            .ExecuteUpdateAsync(update => update.SetProperty(
                appointment => appointment.Status,
                AppointmentStatus.Cancelled));

        TempData["VisitMessage"] = updatedRows == 1
            ? "Часът е отменен успешно."
            : "Този час не може да бъде отменен.";

        return RedirectToAction(nameof(Index));
    }
}