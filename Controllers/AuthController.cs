using System.Security.Claims;
using AspnetCoreMvcFull.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

public class AuthController(UserRepository repo) : Controller
{
  public IActionResult ForgotPasswordBasic() => View();
  public IActionResult LoginBasic() => View();
  public IActionResult RegisterBasic() => View();

  [HttpPost]
  public async Task<IActionResult> LoginBasic(
    string username,
    string password)
  {
    try
    {
      var user = await repo.GetByUsername(username);

      if (user == null)
      {
        ViewBag.Error = "Sai tài khoản";

        return View();
      }

      if (user.PasswordHash != password)
      {
        ViewBag.Error = "Sai mật khẩu";

        return View();
      }

      var claims = new List<Claim>
      {
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Role, user.RoleName)
      };

      var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      var principal = new ClaimsPrincipal(identity);

      await HttpContext
        .SignInAsync(
          CookieAuthenticationDefaults
            .AuthenticationScheme,
          principal
        );

      return RedirectToAction("Index","Dashboards");
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      ViewBag.Error = e.Message;
      throw;
    }
  }

  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    return RedirectToAction(nameof(LoginBasic));
  }
}
