using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCFormula.Areas.Identity.Data;
using MVCFormula.Data;
using MVCFormula.Models;
using MVCFormula.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFormula.Controllers
{
    [Authorize(Roles = "Driver")]
    public class DriverController : Controller
    {

        private readonly MVCFormulaContext _context;
        private readonly UserManager<MVCFormulaUser> userManager;
        private readonly IHostingEnvironment webHostEnvironment;

        public DriverController(MVCFormulaContext context, IHostingEnvironment hostEnvironment, UserManager<MVCFormulaUser> userMgr)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
            userManager = userMgr;
        }

        // GET: Driver/Reviews/5
        public async Task<IActionResult> Reviews(int? id)
        {
            if (id == null)
            {
                MVCFormulaUser formulauser = await userManager.GetUserAsync(User);
                if (formulauser.DriverId != null)
                    return RedirectToAction(nameof(Reviews), new { id = formulauser.DriverId });
                else
                    return NotFound();
            }

            Driver driver = await _context.Driver
                .Include(d => d.Formulas).ThenInclude(r => r.Formula)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (driver == null)
            {
                return NotFound();
            }

            MVCFormulaUser user = await userManager.GetUserAsync(User);
            if (driver.Id != user.DriverId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            return View(driver);
        }


        // GET: Driver/EditReview/5
        public async Task<IActionResult> EditReview(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.FormulaDriver
                .Include(r => r.Formula)
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            MVCFormulaUser user = await userManager.GetUserAsync(User);
            if (review.DriverId != user.DriverId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            var memory = new MemoryStream();

            var reviewEditViewModel = new ReviewEditViewModel
            {
                Review = review
            };

            return View(reviewEditViewModel);
        }


        // POST: Driver/EditReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(int id, ReviewEditViewModel entry)
        {
            if (entry.Review.Id != id)
            {
                return NotFound();
            }

            var review = await _context.FormulaDriver
                .Include(r => r.Formula)
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            MVCFormulaUser user = await userManager.GetUserAsync(User);
            if (review.DriverId != user.DriverId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    review.Fuel = entry.Review.Fuel;
                    review.Chassis = entry.Review.Chassis;
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(EditReview));
        }

        // GET: Driver/CreateReview
        public async Task<IActionResult> CreateReview(/*int driverId*/)
        {
            ViewData["CarId"] = new SelectList(_context.Formula, "Id", "Model");
            ViewData["DriverId"] = new SelectList(_context.Driver, "Id", "FullName");
            return View();
        }


        // POST: Driver/EditReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(/*int driverId, */ReviewCreateViewModel entry)
        {
            if (ModelState.IsValid)
            {
                FormulaDriver review = new FormulaDriver
                {
                    Id = entry.Review.Id,
                    DriverId = entry.Review.DriverId,
                    FormulaId = entry.Review.FormulaId,
                    Fuel = entry.Review.Fuel,
                    Chassis = entry.Review.Chassis,
                    Pts = entry.Review.Pts

                };

                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("ReviewDetails", new { Id = review.Id });
            }
            return View();
        }




        // GET: Driver/ReviewDetails/5
        public async Task<IActionResult> ReviewDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.FormulaDriver
                .Include(r => r.Formula)
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            MVCFormulaUser user = await userManager.GetUserAsync(User);
            if (review.DriverId != user.DriverId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            return View(review);
        }


        // GET: Driver/Manufacturers
        public async Task<IActionResult> Manufacturers(string sortOrder, string searchString)
        {
            ViewData["CountrySortParm"] = String.IsNullOrEmpty(sortOrder) ? "type_desc" : "";
            ViewData["CurrentFilter"] = searchString;
            var manufacturers = from m in _context.Manufacturer select m;
            manufacturers = manufacturers.Include(m => m.Formulas);

            if (!String.IsNullOrEmpty(searchString))
            {
                manufacturers = manufacturers.Where(m => m.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "type_desc":
                    manufacturers = manufacturers.OrderBy(m => m.Country);
                    break;
                default:
                    manufacturers = manufacturers.OrderBy(m => m.Name);
                    break;
            }

            return View(await manufacturers.AsNoTracking().ToListAsync());
        }


        // GET: Driver/ManufacturerDetails/5
        public async Task<IActionResult> ManufacturerDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manufacturer = await _context.Manufacturer
                .Include(m => m.Formulas)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            return View(manufacturer);
        }

        // GET: Driver/Cars
        public async Task<IActionResult> Cars(string sortOrder, string searchString)
        {
            ViewData["TypeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "type_desc" : "";
            ViewData["CurrentFilter"] = searchString;
            var MVCFormulaContext = from c in _context.Formula select c;

            MVCFormulaContext = _context.Formula.Include(c => c.Manufacturer).Include(c => c.Drivers).
                ThenInclude(r => r.Driver);

            if (!String.IsNullOrEmpty(searchString))
            {
                MVCFormulaContext = MVCFormulaContext.Where(c => c.Model.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "type_desc":
                    MVCFormulaContext = MVCFormulaContext.OrderBy(c => c.Model);
                    break;
                default:
                    MVCFormulaContext = MVCFormulaContext.OrderBy(c => c.Manufacturer.Name);
                    break;
            }

            return View(await MVCFormulaContext.AsNoTracking().ToListAsync());
        }

        // GET: Driver/CarDetails/5
        public async Task<IActionResult> CarDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Formula
                .Include(c => c.Manufacturer)
                .Include(c => c.Drivers).ThenInclude(r => r.Driver)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }


        // GET: Driver/ReviewList
        public async Task<IActionResult> ReviewList()
        {
            var MVCFormulaContext = _context.FormulaDriver.Include(r => r.Formula).Include(r => r.Driver);
            return View(await MVCFormulaContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> OthersReview(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.FormulaDriver
                .Include(r => r.Driver)
                .Include(r => r.Formula)
                .ThenInclude(c => c.Manufacturer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }



        private bool ReviewExists(int id)
        {
            return _context.FormulaDriver.Any(e => e.Id == id);
        }
    }
}


