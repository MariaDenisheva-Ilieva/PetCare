using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Data.Entities;
using PetCare.Web.Models;

namespace PetCare.Web.Controllers;

[Authorize(Roles = "User")]
public class AppointmentsController : Controller
{
    private static readonly TimeZoneInfo ClinicTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Sofia");

    private readonly PetCareDbContext dbContext;
    private readonly UserManager<IdentityUser> userManager;

    public AppointmentsController(
        PetCareDbContext dbContext,
        UserManager<IdentityUser> userManager)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int veterinarianId)
    {
        var userId = userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var model = new AppointmentCreateViewModel
        {
            VeterinarianId = veterinarianId
        };

        if (!await LoadFormAsync(model, userId))
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentCreateViewModel model)
    {
        var userId = userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        if (!await LoadFormAsync(model, userId))
        {
            return NotFound();
        }

        var ownsPet = await dbContext.Pets.AnyAsync(
            pet => pet.Id == model.PetId && pet.Owner.UserId == userId);

        if (!ownsPet)
        {
            ModelState.AddModelError(nameof(model.PetId), "Изберете свой любимец.");
        }

        DateTimeOffset? startsAt = null;

        if (model.LocalStart is DateTime selectedStart)
        {
            var localStart = DateTime.SpecifyKind(selectedStart, DateTimeKind.Unspecified);

            if (localStart.TimeOfDay.Ticks % TimeSpan.FromMinutes(30).Ticks != 0)
            {
                ModelState.AddModelError(nameof(model.LocalStart),
                    "Изберете час, който започва на :00 или :30.");
            }
            else if (ClinicTimeZone.IsInvalidTime(localStart) ||
                     ClinicTimeZone.IsAmbiguousTime(localStart))
            {
                ModelState.AddModelError(nameof(model.LocalStart),
                    "Изберете друг час заради смяната на часовото време.");
            }
            else
            {
                startsAt = new DateTimeOffset(
                    TimeZoneInfo.ConvertTimeToUtc(localStart, ClinicTimeZone));

                if (startsAt.Value <= DateTimeOffset.UtcNow)
                {
                    ModelState.AddModelError(nameof(model.LocalStart),
                        "Изберете дата и час в бъдещето.");
                }
            }
        }

        if (!ModelState.IsValid || startsAt is null)
        {
            return View(model);
        }

        var appointment = new Appointment
        {
            PetId = model.PetId,
            VeterinarianId = model.VeterinarianId,
            StartsAt = startsAt.Value,
            Reason = model.Reason.Trim(),
            Status = AppointmentStatus.Scheduled
        };

        dbContext.Appointments.Add(appointment);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            ModelState.AddModelError(nameof(model.LocalStart),
                "Този час вече е зает. Изберете друг час.");

            return View(model);
        }

        TempData["AppointmentSaved"] = "Часът е записан успешно.";

        return RedirectToAction("Index", "Veterinarians");
    }

    private async Task<bool> LoadFormAsync(
        AppointmentCreateViewModel model,
        string userId)
    {
        var veterinarian = await dbContext.Veterinarians
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == model.VeterinarianId);

        if (veterinarian is null)
        {
            return false;
        }

        model.VeterinarianName = $"{veterinarian.FirstName} {veterinarian.LastName}";

        model.Pets = await dbContext.Pets
            .AsNoTracking()
            .Where(pet => pet.Owner.UserId == userId)
            .OrderBy(pet => pet.Name)
            .ThenBy(pet => pet.Id)
            .Select(pet => new SelectListItem
            {
                Value = pet.Id.ToString(),
                Text = pet.Name
            })
            .ToListAsync();

        return true;
    }
}