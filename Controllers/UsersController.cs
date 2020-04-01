using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CollectionApp.Models;
using CollectionApp.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CollectionApp.Controllers
{
    public class UsersController:Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        public UsersController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> Index()
        {
            View(_userManager.Users.ToList());
            User user;
            if (User.Identity.Name != null)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user.Role=="admin" || user.Role=="moderator")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("", "Home");
                }
            }
            else
            {
                return RedirectToAction("", "Home");
            }
            
        }



        [HttpPost]
        public async Task<ActionResult> Functions(List<string> Id, string Delete, string Block, string Unblock,  string ChangeRole)
        {
            User user;
            User currentuser;
            currentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!string.IsNullOrEmpty(Delete))
            {
                for (int i = 0; i < Id.Count; i++)
                {
                    user = await _userManager.FindByIdAsync(Id[i]);
                    if (user != null && user.Role=="user")
                    {
                        await _userManager.DeleteAsync(user);
                    }
                    if (user!=null && currentuser.Role=="admin" && user.Role!="admin")
                    {
                        await _userManager.DeleteAsync(user);
                    }
                }

            }
            if (!string.IsNullOrEmpty(Block))
            {
                for (int i = 0; i < Id.Count; i++)
                {
                    user = await _userManager.FindByIdAsync(Id[i]);
                    if (user != null && user.Role == "user")
                    {
                        user.Block = "true";
                        await _userManager.UpdateAsync(user);
                    }
                    if (user != null && currentuser.Role == "admin" && user.Role != "admin")
                    {
                        user.Block = "true";
                        await _userManager.UpdateAsync(user);
                    }
                }


            }
            if (!string.IsNullOrEmpty(Unblock))
            {
                for (int i = 0; i < Id.Count; i++)
                {
                    user = await _userManager.FindByIdAsync(Id[i]);
                    if (user != null)
                    {
                        user.Block = "false";
                        await _userManager.UpdateAsync(user);
                    }
                    

                }


            }
            if (!string.IsNullOrEmpty(ChangeRole))
            {
                for (int i=0; i<Id.Count;i++)
                {
                    user = await _userManager.FindByIdAsync(Id[i]);
                    if (user!=null && currentuser.Role=="admin")
                    {
                        if (user.Role == "user")
                        {
                            user.Role = "moderator";
                        }
                        else if (user.Role == "moderator")
                        {
                            user.Role = "user";
                        }
                        await _userManager.UpdateAsync(user);
                    }
                }
            }

            return RedirectToAction("Index");

        }
    }
}
