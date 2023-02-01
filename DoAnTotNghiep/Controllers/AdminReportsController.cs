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
    public class AdminReportsController : Controller
    {
        private readonly DoAnTotNghiepContext _context;

        public AdminReportsController(DoAnTotNghiepContext context)
        {
            _context = context;
        }

        // GET: AdminReports
        public async Task<IActionResult> Index()
        {
            var doAnTotNghiepContext = _context.AdminReports.Include(a => a.Houses).Include(a => a.Users);
            return View(await doAnTotNghiepContext.ToListAsync());
        }

        // GET: AdminReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AdminReports == null)
            {
                return NotFound();
            }

            var adminReport = await _context.AdminReports
                .Include(a => a.Houses)
                .Include(a => a.Users)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminReport == null)
            {
                return NotFound();
            }

            return View(adminReport);
        }

        // GET: AdminReports/Create
        public IActionResult Create()
        {
            ViewData["IdHouse"] = new SelectList(_context.Set<House>(), "Id", "Description");
            ViewData["IdUser"] = new SelectList(_context.Set<User>(), "Id", "Email");
            return View();
        }

        // POST: AdminReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,DeadlineDate,Status,IdUser,IdHouse")] AdminReport adminReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdHouse"] = new SelectList(_context.Set<House>(), "Id", "Description", adminReport.IdHouse);
            ViewData["IdUser"] = new SelectList(_context.Set<User>(), "Id", "Email", adminReport.IdUser);
            return View(adminReport);
        }

        // GET: AdminReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AdminReports == null)
            {
                return NotFound();
            }

            var adminReport = await _context.AdminReports.FindAsync(id);
            if (adminReport == null)
            {
                return NotFound();
            }
            ViewData["IdHouse"] = new SelectList(_context.Set<House>(), "Id", "Description", adminReport.IdHouse);
            ViewData["IdUser"] = new SelectList(_context.Set<User>(), "Id", "Email", adminReport.IdUser);
            return View(adminReport);
        }

        // POST: AdminReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,DeadlineDate,Status,IdUser,IdHouse")] AdminReport adminReport)
        {
            if (id != adminReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminReportExists(adminReport.Id))
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
            ViewData["IdHouse"] = new SelectList(_context.Set<House>(), "Id", "Description", adminReport.IdHouse);
            ViewData["IdUser"] = new SelectList(_context.Set<User>(), "Id", "Email", adminReport.IdUser);
            return View(adminReport);
        }

        // GET: AdminReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AdminReports == null)
            {
                return NotFound();
            }

            var adminReport = await _context.AdminReports
                .Include(a => a.Houses)
                .Include(a => a.Users)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminReport == null)
            {
                return NotFound();
            }

            return View(adminReport);
        }

        // POST: AdminReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AdminReports == null)
            {
                return Problem("Entity set 'DoAnTotNghiepContext.AdminReport'  is null.");
            }
            var adminReport = await _context.AdminReports.FindAsync(id);
            if (adminReport != null)
            {
                _context.AdminReports.Remove(adminReport);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminReportExists(int id)
        {
          return _context.AdminReports.Any(e => e.Id == id);
        }
    }
}
