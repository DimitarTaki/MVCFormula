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
    public class FormulaeController : Controller
    {
        private readonly MVCFormulaContext _context;
        private readonly IHostingEnvironment webHostingEnvironment;

        public FormulaeController(MVCFormulaContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            webHostingEnvironment = hostingEnvironment;
        }

        // GET: Formulae
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var mVCFormulaContext = _context.Formula.Include(f => f.Manufacturer).Include(f=>f.Drivers).ThenInclude(f=>f.Driver);
            return View(await mVCFormulaContext.ToListAsync());
        }

        // GET: Formulae/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formula = await _context.Formula
                .Include(f => f.Manufacturer)
                .Include(f=>f.Drivers).ThenInclude(f=>f.Driver).AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formula == null)
            {
                return NotFound();
            }

            return View(formula);
        }

        // GET: Formulae/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ManufacturerId"] = new SelectList(_context.Set<Manufacturer>(), "Id", "Name");
            return View();
        }

        // POST: Formulae/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Model,ModelYear,Tyres,FormulaPicture,ManufacturerId")] Formula formula)
        {
            if (ModelState.IsValid)
            {
                _context.Add(formula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ManufacturerId"] = new SelectList(_context.Set<Manufacturer>(), "Id", "Name", formula.ManufacturerId);
            return View(formula);
        }

        // GET: Formulae/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formula = await _context.Formula.FindAsync(id);
            if (formula == null)
            {
                return NotFound();
            }
            ViewData["ManufacturerId"] = new SelectList(_context.Set<Manufacturer>(), "Id", "Name", formula.ManufacturerId);
            return View(formula);
        }

        // POST: Formulae/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile imageUrl, [Bind("Id,Model,ModelYear,Tyres,FormulaPicture,ManufacturerId")] Formula formula)
        {
            if (id != formula.Id)
            {
                return NotFound();
            }
            FormulaeController uploadImage = new FormulaeController(_context, webHostingEnvironment);
            formula.FormulaPicture = uploadImage.UploadedFile(imageUrl);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormulaExists(formula.Id))
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
            ViewData["ManufacturerId"] = new SelectList(_context.Set<Manufacturer>(), "Id", "Name", formula.ManufacturerId);
            return View(formula);
        }

        // GET: Formulae/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formula = await _context.Formula
                .Include(f => f.Manufacturer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formula == null)
            {
                return NotFound();
            }

            return View(formula);
        }

        // POST: Formulae/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var formula = await _context.Formula.FindAsync(id);
            _context.Formula.Remove(formula);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> DrivingFormula(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var formula = await _context.Formula.Include(f => f.Manufacturer).Include(f => f.Drivers).ThenInclude(f => f.Driver)
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (formula == null)
            {
                return NotFound();
            }

            return View(formula);

        }

        private bool FormulaExists(int id)
        {
            return _context.Formula.Any(e => e.Id == id);
        }

        private string UploadedFile(FormulaViewModel model)
        {
            string uniqueFileName = null;

            if (model.FormulaPicture != null)
            {
                string uploadsFolder = Path.Combine(webHostingEnvironment.WebRootPath, "FormulaImages");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.FormulaPicture.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.FormulaPicture.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        //Oveloaded function UploadedFile for Edit
        public string UploadedFile(IFormFile file)
        {
            string uniqueFileName = null;
            if (file != null)
            {
                string uploadsFolder = Path.Combine(webHostingEnvironment.WebRootPath, "FormulaImages");
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
