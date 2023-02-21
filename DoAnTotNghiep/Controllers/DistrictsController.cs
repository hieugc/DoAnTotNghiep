using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;

namespace DoAnTotNghiep.Controllers
{
    public class DistrictsController : Controller
    {
        private readonly DoAnTotNghiepContext _context;

        public DistrictsController(DoAnTotNghiepContext context)
        {
            _context = context;
        }

        // GET: Districts
        public async Task<IActionResult> Index()
        {
            var doAnTotNghiepContext = _context.Districts.Include(d => d.Citys);
            return View(await doAnTotNghiepContext.ToListAsync());
        }

        // GET: Districts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Districts == null)
            {
                return NotFound();
            }

            var district = await _context.Districts
                .Include(d => d.Citys)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (district == null)
            {
                return NotFound();
            }

            return View(district);
        }

        // GET: Districts/Create
        public IActionResult Create()
        {
            ViewData["IdCity"] = new SelectList(_context.Cities, "Id", "Name");
            return View();
        }

        // POST: Districts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Lat,Lng,Count,IdCity")] District district)
        {
            if (ModelState.IsValid)
            {
                _context.Add(district);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCity"] = new SelectList(_context.Cities, "Id", "Name", district.IdCity);
            return View(district);
        }

        // GET: Districts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Districts == null)
            {
                return NotFound();
            }

            var district = await _context.Districts.FindAsync(id);
            if (district == null)
            {
                return NotFound();
            }
            ViewData["IdCity"] = new SelectList(_context.Cities, "Id", "Name", district.IdCity);
            return View(district);
        }

        // POST: Districts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Lat,Lng,Count,IdCity")] District district)
        {
            if (id != district.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(district);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DistrictExists(district.Id))
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
            ViewData["IdCity"] = new SelectList(_context.Cities, "Id", "Name", district.IdCity);
            return View(district);
        }

        // GET: Districts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Districts == null)
            {
                return NotFound();
            }

            var district = await _context.Districts
                .Include(d => d.Citys)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (district == null)
            {
                return NotFound();
            }

            return View(district);
        }

        // POST: Districts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Districts == null)
            {
                return Problem("Entity set 'DoAnTotNghiepContext.Districts'  is null.");
            }
            var district = await _context.Districts.FindAsync(id);
            if (district != null)
            {
                _context.Districts.Remove(district);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DistrictExists(int id)
        {
          return _context.Districts.Any(e => e.Id == id);
        }
    }
}
