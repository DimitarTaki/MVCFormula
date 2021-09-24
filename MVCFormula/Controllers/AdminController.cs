using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCFormula.Areas.Identity.Data;
using MVCFormula.Data;
using MVCFormula.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFormula.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private UserManager<MVCFormulaUser> userManager;
        private IPasswordHasher<MVCFormulaUser> passwordHasher;
        private IPasswordValidator<MVCFormulaUser> passwordValidator;
        private IUserValidator<MVCFormulaUser> userValidator;
        private readonly MVCFormulaContext _context;

        public AdminController(UserManager<MVCFormulaUser> usrMgr, IPasswordHasher<MVCFormulaUser> passwordHash, IPasswordValidator<MVCFormulaUser> passwordVal, IUserValidator<MVCFormulaUser>
            userValid, MVCFormulaContext context)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
            passwordValidator = passwordVal;
            userValidator = userValid;
            _context = context;
        }


        public IActionResult Index()
        {
            IQueryable<MVCFormulaUser> users = userManager.Users.OrderBy(u => u.Email);
            return View(users);
        }

        public IActionResult DriverProfile(int driverId)
        {
            //AppUser user = await userManager.FindByIdAsync(id);
            MVCFormulaUser user = userManager.Users.FirstOrDefault(u => u.DriverId == driverId);
            Driver driver = _context.Driver.Where(s => s.Id == driverId).FirstOrDefault();
            if (driver != null)
            {
                ViewData["FullName"] = driver.FullName;
                ViewData["DriverId"] = driver.Id;
            }
            if (user != null)
                return View(user);
            else
                return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> DriverProfile(int driverId, string email, string password)
        {
            //AppUser user = await userManager.FindByIdAsync(id);
            MVCFormulaUser user = userManager.Users.FirstOrDefault(u => u.DriverId == driverId);
            if (user != null)
            {
                IdentityResult validUser = null;
                IdentityResult validPass = null;

                user.Email = email;
                user.UserName = email;

                if (string.IsNullOrEmpty(email))
                    ModelState.AddModelError("", "Email cannot be empty");

                validUser = await userValidator.ValidateAsync(userManager, user);
                if (!validUser.Succeeded)
                    Errors(validUser);

                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, user, password);
                    if (validPass.Succeeded)
                        user.PasswordHash = passwordHasher.HashPassword(user, password);
                    else
                        Errors(validPass);
                }

                if (validUser != null && validUser.Succeeded && (string.IsNullOrEmpty(password) || validPass.Succeeded))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(DriverProfile), new { driverId });
                    else
                        Errors(result);
                }
            }
            else
            {
                MVCFormulaUser newuser = new MVCFormulaUser();
                IdentityResult validUser = null;
                IdentityResult validPass = null;

                newuser.Email = email;
                newuser.UserName = email;

                newuser.DriverId = driverId;
                newuser.Role = "Driver";

                if (string.IsNullOrEmpty(email))
                    ModelState.AddModelError("", "Email cannot be empty");

                validUser = await userValidator.ValidateAsync(userManager, newuser);
                if (!validUser.Succeeded)
                    Errors(validUser);

                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, newuser, password);
                    if (validPass.Succeeded)
                        newuser.PasswordHash = passwordHasher.HashPassword(newuser, password);
                    else
                        Errors(validPass);
                }
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (validUser != null && validUser.Succeeded && validPass != null && validPass.Succeeded)
                {
                    IdentityResult result = await userManager.CreateAsync(newuser, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newuser, "Driver");
                        return RedirectToAction(nameof(DriverProfile), new { driverId });
                    }
                    else
                        Errors(result);
                }
                user = newuser;
            }
            Driver driver = _context.Driver.Where(s => s.Id == driverId).FirstOrDefault();
            if (driver != null)
            {
                ViewData["FullName"] = driver.FullName;
                ViewData["DriverId"] = driver.Id;
            }
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            MVCFormulaUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", userManager.Users);
        }




        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

    }
}