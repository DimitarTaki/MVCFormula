using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCFormula.Data;
using MVCFormula.Models;
using MVCFormula.ViewModels;

namespace MVCFormula.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DriversController : Controller
    {
        private readonly MVCFormulaContext _context;
        private readonly IHostingEnvironment webHostingEnvironment;

        public DriversController(MVCFormulaContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            webHostingEnvironment = hostingEnvironment;
        }

        // GET: Drivers
        public async Task<IActionResult> Index()
        {
            var mvcFormulaContext = _context.Driver.Include(d => d.Formulas).ThenInclude(r => r.Formula);
            return View(await _context.Driver.ToListAsync());
        }

        // GET: Drivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Driver
                .Include(d=>d.Formulas).ThenInclude(d=>d.Formula).AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // GET: Drivers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Drivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DriverViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);
                Driver driver = new Driver
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    BirthDate = model.BirthDate,
                    DriverPicture = uniqueFileName,
                    Team=model.Team,
                    Podiums=model.Podiums,
                    Points=model.Points,
                    Country=model.Country
                };
                _context.Add(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Driver.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile imageUrl, [Bind("Id,FirstName,LastName,BirthDate,DriverPicture,Team,Podiums,Points,Country")] Driver driver)
        {
            if (id != driver.Id)
            {
                return NotFound();
            }
            DriversController uploadImage = new DriversController(_context, webHostingEnvironment);
            driver.DriverPicture = uploadImage.UploadedFile(imageUrl);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(driver);
        }

        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Driver
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Driver.FindAsync(id);
            _context.Driver.Remove(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
            return _context.Driver.Any(e => e.Id == id);
        }
        private string UploadedFile(DriverViewModel model)
        {
            string uniqueFileName = null;

            if (model.DriverPicture != null)
            {
                string uploadsFolder = Path.Combine(webHostingEnvironment.WebRootPath, "DriverImages");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.DriverPicture.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.DriverPicture.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        //Overloaded function UploadedFile for Edit
        public string UploadedFile(IFormFile file)
        {
            string uniqueFileName = null;
            if (file != null)
            {
                string uploadsFolder = Path.Combine(webHostingEnvironment.WebRootPath, "DriverImages");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

    }
}
