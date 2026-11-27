using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Data.Entities;
using PetCare.Web.Models;

namespace PetCare.Web.Controllers;

[Authorize(Roles = "User")]
public class MyAppointmentsController : Controller
{
    private const int PageSize = 5;

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
    public async Task<IActionResult> Index(
        AppointmentStatus? status,
        int page = 1)
    {
        var userId = userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        if (status.HasValue && !Enum.IsDefined(status.Value))
        {
            status = null;
        }

        var query = dbContext.Appointments
            .AsNoTracking()
            .Where(appointment => appointment.Pet.Owner.UserId == userId);

        if (status is AppointmentStatus selectedStatus)
        {
            query = query.Where(appointment => appointment.Status == selectedStatus);
        }

        var totalCount = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var appointments = await query
            .Include(appointment => appointment.Pet)
            .Include(appointment => appointment.Veterinarian)
            .OrderByDescending(appointment => appointment.StartsAt)
            .ThenByDescending(appointment => appointment.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return View(new MyAppointmentsViewModel
        {
            Appointments = appointments,
            Status = status,
            Page = page,
            TotalPages = totalPages
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(
        int id,
        AppointmentStatus? status,
        int page = 1)
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

        return RedirectToAction(nameof(Index), new { status, page });
    }
}