using BestReg.Data;
using BestReg.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestReg.Controllers
{
    [Authorize(Roles = "Admin, SystemAdmin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Index action - shared by both Admin and SystemAdmin roles
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userViewModel = new AdminUserViewModel
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList()
                };
                model.Add(userViewModel);
            }

            return View(model);
        }

    
        public IActionResult CreateUser()
        {
            ViewData["Roles"] = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync(model.Role))
                    {
                        var roleAssignResult = await _userManager.AddToRoleAsync(user, model.Role);
                        if (roleAssignResult.Succeeded)
                        {
                            // Redirect to the Identity Register page
                            return RedirectToPage("/Account/Register", new { area = "Identity" });
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Failed to assign role: {string.Join(", ", roleAssignResult.Errors.Select(e => e.Description))}");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Role does not exist.");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            ViewData["Roles"] = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }



    }
}
