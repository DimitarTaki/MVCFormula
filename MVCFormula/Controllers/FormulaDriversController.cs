using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCFormula.Data;
using MVCFormula.Models;

namespace MVCFormula.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FormulaDriversController : Controller
    {
        private readonly MVCFormulaContext _context;

        public FormulaDriversController(MVCFormulaContext context)
        {
            _context = context;
        }

        // GET: FormulaDrivers
        public async Task<IActionResult> Index()
        {
            var mVCFormulaContext = _context.FormulaDriver.Include(f => f.Driver).Include(f => f.Formula);
            return View(await mVCFormulaContext.ToListAsync());
        }

        // GET: FormulaDrivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formulaDriver = await _context.FormulaDriver
                .Include(f => f.Driver)
                .Include(f => f.Formula)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formulaDriver == null)
            {
                return NotFound();
            }

            return View(formulaDriver);
        }

        // GET: FormulaDrivers/Create
        public IActionResult Create()
        {
            ViewData["DriverId"] = new SelectList(_context.Driver, "Id", "FullName");
            ViewData["FormulaId"] = new SelectList(_context.Formula, "Id", "Model");
            return View();
        }

        // POST: FormulaDrivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FormulaId,DriverId,Fuel,Chassis,Pts")] FormulaDriver formulaDriver)
        {
            if (ModelState.IsValid)
            {
                _context.Add(formulaDriver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DriverId"] = new SelectList(_context.Driver, "Id", "FirstName", formulaDriver.DriverId);
            ViewData["FormulaId"] = new SelectList(_context.Formula, "Id", "Model", formulaDriver.FormulaId);
            return View(formulaDriver);
        }

        // GET: FormulaDrivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formulaDriver = await _context.FormulaDriver.FindAsync(id);
            if (formulaDriver == null)
            {
                return NotFound();
            }
            ViewData["DriverId"] = new SelectList(_context.Driver, "Id", "FirstName", formulaDriver.DriverId);
            ViewData["FormulaId"] = new SelectList(_context.Formula, "Id", "Model", formulaDriver.FormulaId);
            return View(formulaDriver);
        }

        // POST: FormulaDrivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FormulaId,DriverId,Fuel,Chassis,Pts")] FormulaDriver formulaDriver)
        {
            if (id != formulaDriver.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formulaDriver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormulaDriverExists(formulaDriver.Id))
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
            ViewData["DriverId"] = new SelectList(_context.Driver, "Id", "FirstName", formulaDriver.DriverId);
            ViewData["FormulaId"] = new SelectList(_context.Formula, "Id", "Model", formulaDriver.FormulaId);
            return View(formulaDriver);
        }

        // GET: FormulaDrivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formulaDriver = await _context.FormulaDriver
                .Include(f => f.Driver)
                .Include(f => f.Formula)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formulaDriver == null)
            {
                return NotFound();
            }

            return View(formulaDriver);
        }

        // POST: FormulaDrivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var formulaDriver = await _context.FormulaDriver.FindAsync(id);
            _context.FormulaDriver.Remove(formulaDriver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormulaDriverExists(int id)
        {
            return _context.FormulaDriver.Any(e => e.Id == id);
        }
    }
}
