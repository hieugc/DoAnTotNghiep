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
    public class CircleExchangeHousesController : Controller
    {
        private readonly DoAnTotNghiepContext _context;

        public CircleExchangeHousesController(DoAnTotNghiepContext context)
        {
            _context = context;
        }

        // GET: CircleExchangeHouses
        public async Task<IActionResult> Index()
        {
              return View(await _context.CircleExchangeRooms.ToListAsync());
        }

        // GET: CircleExchangeHouses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CircleExchangeRooms == null)
            {
                return NotFound();
            }

            var circleExchangeHouse = await _context.CircleExchangeRooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (circleExchangeHouse == null)
            {
                return NotFound();
            }

            return View(circleExchangeHouse);
        }

        // GET: CircleExchangeHouses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CircleExchangeHouses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status")] CircleExchangeHouse circleExchangeHouse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(circleExchangeHouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(circleExchangeHouse);
        }

        // GET: CircleExchangeHouses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CircleExchangeRooms == null)
            {
                return NotFound();
            }

            var circleExchangeHouse = await _context.CircleExchangeRooms.FindAsync(id);
            if (circleExchangeHouse == null)
            {
                return NotFound();
            }
            return View(circleExchangeHouse);
        }

        // POST: CircleExchangeHouses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] CircleExchangeHouse circleExchangeHouse)
        {
            if (id != circleExchangeHouse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(circleExchangeHouse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CircleExchangeHouseExists(circleExchangeHouse.Id))
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
            return View(circleExchangeHouse);
        }

        // GET: CircleExchangeHouses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CircleExchangeRooms == null)
            {
                return NotFound();
            }

            var circleExchangeHouse = await _context.CircleExchangeRooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (circleExchangeHouse == null)
            {
                return NotFound();
            }

            return View(circleExchangeHouse);
        }

        // POST: CircleExchangeHouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CircleExchangeRooms == null)
            {
                return Problem("Entity set 'DoAnTotNghiepContext.CircleExchangeRooms'  is null.");
            }
            var circleExchangeHouse = await _context.CircleExchangeRooms.FindAsync(id);
            if (circleExchangeHouse != null)
            {
                _context.CircleExchangeRooms.Remove(circleExchangeHouse);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CircleExchangeHouseExists(int id)
        {
          return _context.CircleExchangeRooms.Any(e => e.Id == id);
        }
    }
}
