using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCFormula.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFormula.Controllers
{
    public class GuestController : Controller
    {
        private readonly MVCFormulaContext _context;
        private readonly IHostingEnvironment webHostingEnvironment;

        public GuestController(MVCFormulaContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            webHostingEnvironment = hostingEnvironment;
        }


        public IActionResult Index()
        {
            return View();
        }

        // GET: Guest/Manufacturers
        public async Task<IActionResult> Manufacturers(string searchString)
        {
           
            ViewData["CurrentFilter"] = searchString;
            var manufacturers = from m in _context.Manufacturer select m;
            manufacturers = manufacturers.Include(m => m.Formulas);

            if (!String.IsNullOrEmpty(searchString))
            {
                manufacturers = manufacturers.Where(m => m.Name.Contains(searchString));
            }

           

            return View(await manufacturers.AsNoTracking().ToListAsync());
        }


        // GET: Guest/ManufacturerDetails/5
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

        // GET: Guest/Cars
        public async Task<IActionResult> Formulas(string sortOrder, string searchString)
        {
          
            ViewData["CurrentFilter"] = searchString;
            var mvcFormulaContext = from c in _context.Formula select c;

            mvcFormulaContext = _context.Formula.Include(c => c.Manufacturer).Include(c => c.Drivers).
                ThenInclude(r => r.Driver);

            if (!String.IsNullOrEmpty(searchString))
            {
                mvcFormulaContext = mvcFormulaContext.Where(f => f.Model.Contains(searchString));
            }

          

            return View(await mvcFormulaContext.AsNoTracking().ToListAsync());
        }

        // GET: Guest/CarDetails/5
        public async Task<IActionResult> FormulaDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formula = await _context.Formula
                .Include(c => c.Manufacturer)
                .Include(c => c.Drivers).ThenInclude(r => r.Driver)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formula == null)
            {
                return NotFound();
            }

            return View(formula);
        }
    }
}


