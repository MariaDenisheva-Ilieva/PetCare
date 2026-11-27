using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Data.Entities;

namespace PetCare.Web.Controllers;

[Authorize(Roles = "User")]
public class MyAppointmentsController : Controller
{
    private readonly PetCareDbContext dbContext;
    private readonly UserManager<IdentityUser> userManager;

    public MyAppointmentsController(
        PetCareDbContext dbContext,
        UserManager<IdentityUser> userManager)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var appointments = await dbContext.Appointments
            .AsNoTracking()
            .Where(appointment => appointment.Pet.Owner.UserId == userId)
            .Include(appointment => appointment.Pet)
            .Include(appointment => appointment.Veterinarian)
            .OrderByDescending(appointment => appointment.StartsAt)
            .ThenByDescending(appointment => appointment.Id)
            .ToListAsync();

        return View(appointments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var now = DateTimeOffset.UtcNow;

        var updatedRows = await dbContext.Appointments
            .Where(appointment =>
                appointment.Id == id &&
                appointment.Pet.Owner.UserId == userId &&
                appointment.Status == AppointmentStatus.Scheduled &&
                appointment.StartsAt > now)
            .ExecuteUpdateAsync(update => update.SetProperty(
                appointment => appointment.Status,
                AppointmentStatus.Cancelled));

        if (updatedRows == 1)
        {
            TempData["AppointmentCancelled"] = "Часът е отменен успешно.";
        }
        else
        {
            TempData["AppointmentCancelError"] = "Този час не може да бъде отменен.";
        }

        return RedirectToAction(nameof(Index));
    }
}