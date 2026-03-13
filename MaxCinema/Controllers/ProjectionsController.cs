using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaxCinema.Data;
using MaxCinema.Models;
using Microsoft.AspNetCore.Authorization;

namespace MaxCinema.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProjectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projections - ПОПРАВЕНО: вече включва Movie и Hall
        public async Task<IActionResult> Index()
        {
            var projections = _context.Projections
                .Include(p => p.Movie)      // <- ТОВА Е ВАЖНОТО! Зарежда името на филма
                .Include(p => p.Hall)       // <- ТОВА Е ВАЖНОТО! Зарежда името на залата
                .ToListAsync();

            return View(await projections);
        }

        // GET: Projections/Details/5 - ПОПРАВЕНО: вече включва Movie и Hall
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projection = await _context.Projections
                .Include(p => p.Movie)      // <- ТОВА Е ВАЖНОТО!
                .Include(p => p.Hall)       // <- ТОВА Е ВАЖНОТО!
                .FirstOrDefaultAsync(m => m.Id == id);

            if (projection == null)
            {
                return NotFound();
            }

            return View(projection);
        }

        // GET: Projections/Create
        public IActionResult Create()
        {
            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name");      // <- Name, не Id
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");   // <- Title, не Id
            return View();
        }

        // POST: Projections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieId,HallId,StartTime,TicketPrice")] Projection projection)
        {
            // Проста валидация
            if (projection.MovieId > 0 && projection.HallId > 0 && projection.TicketPrice > 0)
            {
                _context.Add(projection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", projection.HallId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", projection.MovieId);
            return View(projection);
        }

        // GET: Projections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projection = await _context.Projections.FindAsync(id);
            if (projection == null)
            {
                return NotFound();
            }

            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", projection.HallId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", projection.MovieId);
            return View(projection);
        }

        // POST: Projections/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,HallId,StartTime,TicketPrice")] Projection projection)
        {
            if (id != projection.Id)
            {
                return NotFound();
            }

            if (projection.MovieId > 0 && projection.HallId > 0 && projection.TicketPrice > 0)
            {
                try
                {
                    _context.Update(projection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectionExists(projection.Id))
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

            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", projection.HallId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", projection.MovieId);
            return View(projection);
        }

        // GET: Projections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projection = await _context.Projections
                .Include(p => p.Hall)
                .Include(p => p.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (projection == null)
            {
                return NotFound();
            }

            return View(projection);
        }

        // POST: Projections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projection = await _context.Projections.FindAsync(id);
            if (projection != null)
            {
                _context.Projections.Remove(projection);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectionExists(int id)
        {
            return _context.Projections.Any(e => e.Id == id);
        }
    }
}