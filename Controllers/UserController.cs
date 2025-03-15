﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EventManager.Models;
using EventManager.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;

namespace EventManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private UserManager<AppUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<AppUser> userMngr, RoleManager<IdentityRole> roleMngr)
        {
            userManager = userMngr;
            roleManager = roleMngr;
        }

        public IActionResult Index()
        {
            List<AppUser> users = new List<AppUser>();
            users = userManager.Users.ToList();

            foreach (AppUser user in users)
            {
                var task = userManager.GetRolesAsync(user);
                task.Wait();
                user.RoleNames = task.Result;
            }

            UserViewModel model = new UserViewModel
            {
                Users = users,
                Roles = roleManager.Roles
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Username };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                try
                {
                    IdentityResult result = await userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        string errorMessage = "";
                        foreach (IdentityError error in result.Errors)
                        {
                            errorMessage += error.Description + " | ";
                        }
                        TempData["message"] = errorMessage;
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "User deleted successfully.";
                    }
                }
                catch (DbUpdateException ex)
                {
                    if(ex.InnerException != null && ex.InnerException.Message.Contains("foreign key"))
                    {
                        TempData["ErrorMessage"] = "User cannot be deleted, as they are referenced in other records.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An unexpected error occured while deleing the user.";
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddToAdmin(string id)
        {
            IdentityRole adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                TempData["message"] = "Admin role does not exist. " + "Click 'Create Admin Role' button to create it.";
            }
            else
            {
                AppUser user = await userManager.FindByIdAsync(id);
                await userManager.AddToRoleAsync(user, adminRole.Name);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromAdmin(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            await userManager.RemoveFromRoleAsync(user, "Admin");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdminRole()
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            await roleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }
    }
}
