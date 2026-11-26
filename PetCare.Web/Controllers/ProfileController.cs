using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Data.Entities;
using PetCare.Web.Models;

namespace PetCare.Web.Controllers;

[Authorize(Roles = "User")]
public class ProfileController : Controller
{
    private readonly PetCareDbContext dbContext;
    private readonly UserManager<IdentityUser> userManager;

    public ProfileController(
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

        var model = await dbContext.Owners
            .AsNoTracking()
            .Where(owner => owner.UserId == userId)
            .Select(owner => new OwnerProfileViewModel
            {
                FirstName = owner.FirstName,
                LastName = owner.LastName,
                PhoneNumber = owner.PhoneNumber
            })
            .SingleOrDefaultAsync();

        return View(model ?? new OwnerProfileViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(OwnerProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var owner = await dbContext.Owners
            .SingleOrDefaultAsync(owner => owner.UserId == userId);

        if (owner is null)
        {
            owner = new Owner { UserId = userId };
            dbContext.Owners.Add(owner);
        }

        owner.FirstName = model.FirstName.Trim();
        owner.LastName = model.LastName.Trim();
        owner.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber)
            ? null
            : model.PhoneNumber.Trim();

        await dbContext.SaveChangesAsync();

        TempData["ProfileSaved"] = "Профилът е запазен успешно.";

        return RedirectToAction(nameof(Index));
    }
}