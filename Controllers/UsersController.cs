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
using Microsoft.EntityFrameworkCore;

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

        public async Task<ActionResult> Index(UserSort sortOrder = UserSort.NameAsc)
        {
            IQueryable<User> users = _userManager.Users;
            ViewData["NameSort"] = sortOrder == UserSort.NameAsc ? UserSort.NameDesc : UserSort.NameAsc;
            ViewData["StatusSort"] = sortOrder == UserSort.StatusAsc ? UserSort.StatusDesc : UserSort.StatusAsc;
            ViewData["RoleSort"] = sortOrder == UserSort.RoleAsc ? UserSort.RoleDesc : UserSort.RoleAsc;
            ViewData["BlockSort"] = sortOrder == UserSort.BlockAsc ? UserSort.BlockDesc : UserSort.BlockAsc;

            users = sortOrder switch
            {
                UserSort.NameDesc => users.OrderByDescending(s => s.UserName),
                UserSort.StatusAsc => users.OrderBy(s => s.Status),
                UserSort.StatusDesc => users.OrderByDescending(s => s.Status),
                UserSort.RoleAsc => users.OrderBy(s => s.Role),
                UserSort.RoleDesc => users.OrderByDescending(s => s.Role),
                UserSort.BlockAsc => users.OrderBy(s => s.Block),
                UserSort.BlockDesc => users.OrderByDescending(s => s.Block),
                _ => users.OrderBy(s => s.UserName),
            };
            string role = null;
            if (User.Identity.Name!=null)
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                role = user.Role;
            }
            UsersViewModel model = new UsersViewModel
            {
                Users = await users.AsNoTracking().ToListAsync(),
                Role = role,
            };
            return View(model); 
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
