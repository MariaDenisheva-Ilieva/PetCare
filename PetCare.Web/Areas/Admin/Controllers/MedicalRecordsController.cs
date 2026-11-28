using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Data.Entities;
using PetCare.Web.Models;

namespace PetCare.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class MedicalRecordsController : Controller
{
    private readonly PetCareDbContext dbContext;

    public MedicalRecordsController(PetCareDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var visits = await (
            from appointment in dbContext.Appointments.AsNoTracking()
            where appointment.Status == AppointmentStatus.Completed
            join record in dbContext.MedicalRecords.AsNoTracking()
                on appointment.Id equals record.AppointmentId into records
            from record in records.DefaultIfEmpty()
            orderby appointment.StartsAt descending, appointment.Id descending
            select new MedicalRecordVisitViewModel
            {
                AppointmentId = appointment.Id,
                StartsAt = appointment.StartsAt,
                PetName = appointment.Pet.Name,
                VeterinarianName = appointment.Veterinarian.FirstName + " " +
                    appointment.Veterinarian.LastName,
                Diagnosis = record == null ? null : record.Diagnosis,
                Notes = record == null ? null : record.Notes
            })
            .ToListAsync();

        return View(visits);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int appointmentId)
    {
        if (!await IsCompletedVisitAsync(appointmentId))
        {
            return NotFound();
        }

        if (await RecordExistsAsync(appointmentId))
        {
            TempData["RecordMessage"] = "За този преглед вече има медицински запис.";
            return RedirectToAction(nameof(Index));
        }

        return View(new MedicalRecordCreateViewModel
        {
            AppointmentId = appointmentId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MedicalRecordCreateViewModel model)
    {
        if (!await IsCompletedVisitAsync(model.AppointmentId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await RecordExistsAsync(model.AppointmentId))
        {
            TempData["RecordMessage"] = "За този преглед вече има медицински запис.";
            return RedirectToAction(nameof(Index));
        }

        dbContext.MedicalRecords.Add(new MedicalRecord
        {
            AppointmentId = model.AppointmentId,
            Diagnosis = model.Diagnosis.Trim(),
            Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
        });

        try
        {
            await dbContext.SaveChangesAsync();
            TempData["RecordMessage"] = "Медицинският запис е запазен.";
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            TempData["RecordMessage"] = "За този преглед вече има медицински запис.";
        }

        return RedirectToAction(nameof(Index));
    }

    private Task<bool> IsCompletedVisitAsync(int appointmentId)
    {
        return dbContext.Appointments.AnyAsync(appointment =>
            appointment.Id == appointmentId &&
            appointment.Status == AppointmentStatus.Completed);
    }

    private Task<bool> RecordExistsAsync(int appointmentId)
    {
        return dbContext.MedicalRecords.AnyAsync(record =>
            record.AppointmentId == appointmentId);
    }
}