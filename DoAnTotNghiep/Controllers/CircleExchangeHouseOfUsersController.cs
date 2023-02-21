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
    public class CircleExchangeHouseOfUsersController : Controller
    {
        private readonly DoAnTotNghiepContext _context;

        public CircleExchangeHouseOfUsersController(DoAnTotNghiepContext context)
        {
            _context = context;
        }

        // GET: CircleExchangeHouseOfUsers
        public async Task<IActionResult> Index()
        {
            var doAnTotNghiepContext = _context.CircleExchangeHouseOfUsers.Include(c => c.CircleExchangeHouse).Include(c => c.Users);
            return View(await doAnTotNghiepContext.ToListAsync());
        }

        // GET: CircleExchangeHouseOfUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CircleExchangeHouseOfUsers == null)
            {
                return NotFound();
            }

            var circleExchangeHouseOfUser = await _context.CircleExchangeHouseOfUsers
                .Include(c => c.CircleExchangeHouse)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (circleExchangeHouseOfUser == null)
            {
                return NotFound();
            }

            return View(circleExchangeHouseOfUser);
        }

        // GET: CircleExchangeHouseOfUsers/Create
        public IActionResult Create()
        {
            ViewData["IdCircleExchangeHouse"] = new SelectList(_context.CircleExchangeRooms, "Id", "Id");
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: CircleExchangeHouseOfUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCircleExchangeHouse,IdUser")] CircleExchangeHouseOfUser circleExchangeHouseOfUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(circleExchangeHouseOfUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCircleExchangeHouse"] = new SelectList(_context.CircleExchangeRooms, "Id", "Id", circleExchangeHouseOfUser.IdCircleExchangeHouse);
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Email", circleExchangeHouseOfUser.IdUser);
            return View(circleExchangeHouseOfUser);
        }

        // GET: CircleExchangeHouseOfUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CircleExchangeHouseOfUsers == null)
            {
                return NotFound();
            }

            var circleExchangeHouseOfUser = await _context.CircleExchangeHouseOfUsers.FindAsync(id);
            if (circleExchangeHouseOfUser == null)
            {
                return NotFound();
            }
            ViewData["IdCircleExchangeHouse"] = new SelectList(_context.CircleExchangeRooms, "Id", "Id", circleExchangeHouseOfUser.IdCircleExchangeHouse);
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Email", circleExchangeHouseOfUser.IdUser);
            return View(circleExchangeHouseOfUser);
        }

        // POST: CircleExchangeHouseOfUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCircleExchangeHouse,IdUser")] CircleExchangeHouseOfUser circleExchangeHouseOfUser)
        {
            if (id != circleExchangeHouseOfUser.IdUser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(circleExchangeHouseOfUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CircleExchangeHouseOfUserExists(circleExchangeHouseOfUser.IdUser))
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
            ViewData["IdCircleExchangeHouse"] = new SelectList(_context.CircleExchangeRooms, "Id", "Id", circleExchangeHouseOfUser.IdCircleExchangeHouse);
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Email", circleExchangeHouseOfUser.IdUser);
            return View(circleExchangeHouseOfUser);
        }

        // GET: CircleExchangeHouseOfUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CircleExchangeHouseOfUsers == null)
            {
                return NotFound();
            }

            var circleExchangeHouseOfUser = await _context.CircleExchangeHouseOfUsers
                .Include(c => c.CircleExchangeHouse)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (circleExchangeHouseOfUser == null)
            {
                return NotFound();
            }

            return View(circleExchangeHouseOfUser);
        }

        // POST: CircleExchangeHouseOfUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CircleExchangeHouseOfUsers == null)
            {
                return Problem("Entity set 'DoAnTotNghiepContext.CircleExchangeHouseOfUsers'  is null.");
            }
            var circleExchangeHouseOfUser = await _context.CircleExchangeHouseOfUsers.FindAsync(id);
            if (circleExchangeHouseOfUser != null)
            {
                _context.CircleExchangeHouseOfUsers.Remove(circleExchangeHouseOfUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CircleExchangeHouseOfUserExists(int id)
        {
          return _context.CircleExchangeHouseOfUsers.Any(e => e.IdUser == id);
        }
    }
}
