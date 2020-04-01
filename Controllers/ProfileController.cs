using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CollectionApp.Models;
using CollectionApp.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CollectionApp.Controllers
{
    public class ProfileController:Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index(string userName)
        {
            User user = await _userManager.FindByNameAsync(userName);
            User user_2=null;
            if (User.Identity.Name != null)
            {
                user_2 = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user!=null && user_2!=null)
            {
                ProfileControllerViewModel model = new ProfileControllerViewModel
                {
                    UserId = user.Id,
                    UserName=user.UserName,
                    UserEmail = user.Email,
                    RegisterDate = user.RegisterDate,
                    LoginDate = user.LoginDate,
                    Status=user.Status,
                    Role=user.Role,
                    Role_2=user_2.Role,
                    
                };
                return View(model);
            }
            if (user!=null && user_2==null)
            {
                ProfileControllerViewModel model = new ProfileControllerViewModel
                {
                    UserId = user.Id,
                    UserName=user.UserName,
                    UserEmail = user.Email,
                    RegisterDate = user.RegisterDate,
                    LoginDate = user.LoginDate,
                    Status=user.Status,
                    Role=user.Role,
                    Role_2=null,
                    
                    
                };
                return View(model);
            }
            return NotFound();
        }
       
    }
}
