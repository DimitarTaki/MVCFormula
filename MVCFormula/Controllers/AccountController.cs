using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCFormula.Areas.Identity.Data;
using MVCFormula.Data;
using MVCFormula.Models;
using MVCFormula.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFormula.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<MVCFormulaUser> userManager;
        private SignInManager<MVCFormulaUser> signInManager;
        private IPasswordHasher<MVCFormulaUser> passwordHasher;
        private IPasswordValidator<MVCFormulaUser> passwordValidator;
        private IUserValidator<MVCFormulaUser> userValidator;
        private readonly MVCFormulaContext _context;

        public AccountController(UserManager<MVCFormulaUser> userMgr, SignInManager<MVCFormulaUser> signinMgr, IPasswordHasher<MVCFormulaUser> passwordHash, IPasswordValidator<MVCFormulaUser> passwordVal, IUserValidator<MVCFormulaUser>
            userValid, MVCFormulaContext context)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            passwordHasher = passwordHash;
            passwordValidator = passwordVal;
            userValidator = userValid;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            LoginViewModel login = new LoginViewModel();
            login.ReturnUrl = returnUrl;
            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                MVCFormulaUser appUser = await userManager.FindByEmailAsync(login.Email);
                if (appUser != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, false);
                    if (result.Succeeded)
                    {
                        if ((await userManager.IsInRoleAsync(appUser, "Admin")))
                        {
                            //return Redirect(login.ReturnUrl ?? "/");
                            return RedirectToAction("Index", "Formulae", null);
                        }
                        if ((await userManager.IsInRoleAsync(appUser, "Driver")))
                        {
                            return RedirectToAction("Reviews", "Driver", new { id = appUser.DriverId });
                        }
                    }
                }
                ModelState.AddModelError(nameof(login.Email), "Login Failed: Invalid Email or password");
            }
            return View(login);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", null);
        }

        [Authorize]
        public async Task<IActionResult> UserInfo()
        {
            MVCFormulaUser curruser = await userManager.GetUserAsync(User);
            string userDetails = curruser.UserName;
            string role = "Admin";
            if (curruser.DriverId != null)
            {
                Driver driver = await _context.Driver.FindAsync(curruser.DriverId);
                userDetails = driver.FullName;
                role = "Driver";
            }
            UserInfoViewModel userInfoViewModel = new UserInfoViewModel
            {
                UserDetails = userDetails,
                Role = role,
                Id = curruser.Id,
                PasswordHash = curruser.PasswordHash,
                Email = curruser.Email
            };
            return View(userInfoViewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserInfo(UserInfoViewModel entry)
        {
            MVCFormulaUser user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(entry.NewPassword))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, user, entry.NewPassword);
                    if (validPass.Succeeded)
                        user.PasswordHash = passwordHasher.HashPassword(user, entry.NewPassword);
                    else
                        Errors(validPass);
                }
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(entry.NewPassword) && validPass.Succeeded)
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(UserInfo));
                    else
                        Errors(result);
                }
            }
            return View(user);
        }



        public IActionResult AccessDenied()
        {
            return View();
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}

