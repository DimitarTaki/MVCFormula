using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCFormula.Areas.Identity.Data;
using MVCFormula.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFormula.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<MVCFormulaUser> userManager;

        public HomeController(UserManager<MVCFormulaUser> userMgr)
        {
            userManager = userMgr;
        }

        public async Task<IActionResult> IndexAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                MVCFormulaUser appUser = await userManager.GetUserAsync(User);
                if ((await userManager.IsInRoleAsync(appUser, "Admin")))
                {
                    return RedirectToAction("Index", "Formulae", null);
                }
                if ((await userManager.IsInRoleAsync(appUser, "Driver")))
                {
                    return RedirectToAction("Reviews", "Driver", null);
                }
            }
            else
            {
                return RedirectToAction("Index", "Guest", null);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
