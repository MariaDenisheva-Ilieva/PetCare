using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;

namespace PetCare.Web.Controllers;

public class VeterinariansController : Controller
{
    private readonly PetCareDbContext dbContext;

    public VeterinariansController(PetCareDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var veterinarians = await dbContext.Veterinarians
            .AsNoTracking()
            .OrderBy(veterinarian => veterinarian.LastName)
            .ThenBy(veterinarian => veterinarian.FirstName)
            .ToListAsync();

        return View(veterinarians);
    }
}