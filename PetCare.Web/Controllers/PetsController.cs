using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Data.Entities;
using PetCare.Web.Models;

namespace PetCare.Web.Controllers;

[Authorize(Roles = "User")]
public class PetsController : Controller
{
    private readonly PetCareDbContext dbContext;
    private readonly UserManager<IdentityUser> userManager;

    public PetsController(
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

        var pets = await dbContext.Pets
            .AsNoTracking()
            .Where(pet => pet.Owner.UserId == userId)
            .OrderBy(pet => pet.Name)
            .ThenBy(pet => pet.Id)
            .ToListAsync();

        return View(pets);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var ownerId = await FindCurrentOwnerIdAsync();

        if (ownerId is null)
        {
            return RedirectToAction("Index", "Profile");
        }

        return View(new PetCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PetCreateViewModel model)
    {
        var ownerId = await FindCurrentOwnerIdAsync();

        if (ownerId is null)
        {
            return RedirectToAction("Index", "Profile");
        }

        if (model.BirthDate > DateOnly.FromDateTime(DateTime.Today))
        {
            ModelState.AddModelError(
                nameof(model.BirthDate),
                "Датата на раждане не може да бъде в бъдещето.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var pet = new Pet
        {
            Name = model.Name.Trim(),
            Species = model.Species.Trim(),
            Breed = string.IsNullOrWhiteSpace(model.Breed) ? null : model.Breed.Trim(),
            BirthDate = model.BirthDate,
            OwnerId = ownerId.Value
        };

        dbContext.Pets.Add(pet);
        await dbContext.SaveChangesAsync();

        TempData["PetSaved"] = "Любимецът е добавен успешно.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<int?> FindCurrentOwnerIdAsync()
    {
        var userId = userManager.GetUserId(User);

        if (userId is null)
        {
            return null;
        }

        var ownerId = await dbContext.Owners
            .Where(owner => owner.UserId == userId)
            .Select(owner => (int?)owner.Id)
            .SingleOrDefaultAsync();

        if (ownerId is null)
        {
            TempData["ProfileRequired"] = "Първо запазете данните си в „Моят профил“.";
        }

        return ownerId;
    }
}