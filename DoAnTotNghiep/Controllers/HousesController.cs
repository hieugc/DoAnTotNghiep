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
    public class HousesController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;

        public HousesController(DoAnTotNghiepContext context): base(context)
        {
            _context = context;
        }

        // GET: Houses
        public async Task<IActionResult> Index()
        {
            var doAnTotNghiepContext = _context.Houses.Include(h => h.Citys).Include(h => h.Districts).Include(h => h.Users).Include(h => h.Wards);
            return View(await doAnTotNghiepContext.ToListAsync());
        }

        // GET: Houses/Details/5
        public IActionResult Details(int? id)
        {
            //if (id == null || _context.Houses == null)
            //{
            //    return NotFound();
            //}

            //var house = await _context.Houses
            //    .Include(h => h.Citys)
            //    .Include(h => h.Districts)
            //    .Include(h => h.Users)
            //    .Include(h => h.Wards)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (house == null)
            //{
            //    return NotFound();
            //}

            return View();
        }

        // GET: Houses/Create
        public IActionResult Create()
        {
            ViewData["IdCity"] = new SelectList(_context.Cities, "Id", "Name");
            ViewData["IdDistrict"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["IdWard"] = new SelectList(_context.Wards, "Id", "Name");
            return View();
        }

        // POST: Houses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,Price,People,BedRoom,BathRoom,Description,Area,NumberOfRating,Rating,Lat,Lng,IdUser,IdCity,IdDistrict,IdWard")] House house)
        {
            if (ModelState.IsValid)
            {
                _context.Add(house);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCity"] = new SelectList(_context.Cities, "Id", "Name", house.IdCity);
            ViewData["IdDistrict"] = new SelectList(_context.Districts, "Id", "Name", house.IdDistrict);
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Email", house.IdUser);
            ViewData["IdWard"] = new SelectList(_context.Wards, "Id", "Name", house.IdWard);
            return View(house);
        }

        // GET: Houses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Houses == null)
            {
                return NotFound();
            }

            var house = await _context.Houses.FindAsync(id);
            if (house == null)
            {
                return NotFound();
            }
            ViewData["IdCity"] = new SelectList(_context.Cities, "Id", "Name", house.IdCity);
            ViewData["IdDistrict"] = new SelectList(_context.Districts, "Id", "Name", house.IdDistrict);
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Email", house.IdUser);
            ViewData["IdWard"] = new SelectList(_context.Wards, "Id", "Name", house.IdWard);
            return View(house);
        }

        // POST: Houses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status,Price,People,BedRoom,BathRoom,Description,Area,NumberOfRating,Rating,Lat,Lng,IdUser,IdCity,IdDistrict,IdWard")] House house)
        {
            if (id != house.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(house);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HouseExists(house.Id))
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
            ViewData["IdCity"] = new SelectList(_context.Cities, "Id", "Name", house.IdCity);
            ViewData["IdDistrict"] = new SelectList(_context.Districts, "Id", "Name", house.IdDistrict);
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Email", house.IdUser);
            ViewData["IdWard"] = new SelectList(_context.Wards, "Id", "Name", house.IdWard);
            return View(house);
        }

        // GET: Houses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Houses == null)
            {
                return NotFound();
            }

            var house = await _context.Houses
                .Include(h => h.Citys)
                .Include(h => h.Districts)
                .Include(h => h.Users)
                .Include(h => h.Wards)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (house == null)
            {
                return NotFound();
            }

            return View(house);
        }

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Houses == null)
            {
                return Problem("Entity set 'DoAnTotNghiepContext.Houses'  is null.");
            }
            var house = await _context.Houses.FindAsync(id);
            if (house != null)
            {
                _context.Houses.Remove(house);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HouseExists(int id)
        {
          return _context.Houses.Any(e => e.Id == id);
        }
    }
}
