using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Data.Entities;
using PetCare.Web.Models;

namespace PetCare.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class TreatmentsController : Controller
{
    private readonly PetCareDbContext dbContext;

    public TreatmentsController(PetCareDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int appointmentId)
    {
        var model = new TreatmentViewModel { AppointmentId = appointmentId };
        var recordId = await LoadRecordAsync(model);

        if (recordId is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(TreatmentViewModel model)
    {
        var recordId = await LoadRecordAsync(model);

        if (recordId is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        dbContext.Treatments.Add(new Treatment
        {
            MedicalRecordId = recordId.Value,
            Name = model.Name.Trim(),
            Instructions = string.IsNullOrWhiteSpace(model.Instructions)
                ? null
                : model.Instructions.Trim()
        });

        await dbContext.SaveChangesAsync();

        TempData["TreatmentSaved"] = "Лечението е добавено успешно.";

        return RedirectToAction(nameof(Index), new { appointmentId = model.AppointmentId });
    }

    private async Task<int?> LoadRecordAsync(TreatmentViewModel model)
    {
        var record = await dbContext.MedicalRecords
            .AsNoTracking()
            .Where(record =>
                record.AppointmentId == model.AppointmentId &&
                record.Appointment.Status == AppointmentStatus.Completed)
            .Select(record => new
            {
                record.Id,
                PetName = record.Appointment.Pet.Name
            })
            .SingleOrDefaultAsync();

        if (record is null)
        {
            return null;
        }

        model.PetName = record.PetName;
        model.Treatments = await dbContext.Treatments
            .AsNoTracking()
            .Where(treatment => treatment.MedicalRecordId == record.Id)
            .OrderBy(treatment => treatment.Id)
            .ToListAsync();

        return record.Id;
    }
}