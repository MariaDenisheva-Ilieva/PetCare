using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetCare.Data;
using PetCare.Web.Models;

namespace PetCare.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly PetCareDbContext dbContext;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        PetCareDbContext dbContext)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.dbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim();
        var user = new IdentityUser
        {
            UserName = email,
            Email = email
        };

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var createResult = await userManager.CreateAsync(user, model.Password);

        if (!createResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,
                "Регистрацията не бе успешна. Проверете имейла и изискванията за паролата.");

            return View(model);
        }

        var roleResult = await userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty,
                "Не успяхме да завършим регистрацията. Опитайте отново.");

            return View(model);
        }

        await transaction.CommitAsync();

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await signInManager.PasswordSignInAsync(
            model.Email.Trim(),
            model.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty,
            "Входът не бе успешен. Проверете имейла и паролата или опитайте по-късно.");

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }
}