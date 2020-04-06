using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CollectionApp.ViewModels;
using CollectionApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace CollectionApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private ApplicationContext db;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.Name != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                if (model.UserName == "Kroutis")
                {
                    model.Role = "admin";
                }
                else
                {
                    model.Role = "user";
                }
                User user = new User { Email = model.Email, UserName = model.UserName, RegisterDate = model.RegisterDate, LoginDate = model.LoginDate, Status = model.Status, Block = model.Block, Role=model.Role };
                // добавляем пользователя

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                    
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.Name != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(new LoginViewModel { });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    User user = await _userManager.FindByNameAsync(model.Username);



                    if (user.Block != "false")
                    {
                        ModelState.AddModelError("", "Banned");
                        await _signInManager.SignOutAsync();
                        return View(model);
                    }
                    else
                    {
                        user.LoginDate = model.LoginDate;
                        user.Status = model.Status;
                        await _userManager.UpdateAsync(user);

                    }

                    /*if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else*/
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Wrong login or password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            var Name = User.Identity.Name;
            User user = await _userManager.FindByNameAsync(Name);
            if (user != null)
            {
                user.Status = model.Status_2;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
