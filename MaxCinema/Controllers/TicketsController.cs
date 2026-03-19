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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MaxCinema.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tickets - само за админ
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var tickets = _context.Tickets
                .Include(t => t.Projection)
                    .ThenInclude(p => p.Movie)
                .Include(t => t.Projection)
                    .ThenInclude(p => p.Hall)
                .Include(t => t.Seat)
                .Include(t => t.User)
                .OrderByDescending(t => t.PurchasedAt);

            return View(await tickets.ToListAsync());
        }

        // GET: Tickets/MyTickets
        [Authorize]
        public async Task<IActionResult> MyTickets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tickets = _context.Tickets
                .Include(t => t.Projection)
                    .ThenInclude(p => p.Movie)
                .Include(t => t.Projection)
                    .ThenInclude(p => p.Hall)
                .Include(t => t.Seat)
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.Projection.StartTime);

            return View(await tickets.ToListAsync());
        }

        // GET: Tickets/Create?projectionId=5
        [Authorize]
        public IActionResult Create(int? projectionId)
        {
            try
            {
                if (projectionId == null)
                {
                    TempData["Error"] = "Не е избрана прожекция.";
                    return RedirectToAction("Index", "Program");
                }

                var projection = _context.Projections
                    .Include(p => p.Movie)
                    .Include(p => p.Hall)
                    .Include(p => p.Tickets)
                    .FirstOrDefault(p => p.Id == projectionId);

                if (projection == null)
                {
                    TempData["Error"] = "Прожекцията не съществува.";
                    return RedirectToAction("Index", "Program");
                }

                if (projection.StartTime <= DateTime.Now)
                {
                    TempData["Error"] = "Не можете да купувате билети за минали прожекции.";
                    return RedirectToAction("Index", "Program");
                }

               
                var allSeats = _context.Seats
                    .Where(s => s.HallId == projection.HallId)
                    .OrderBy(s => s.Row)
                    .ThenBy(s => s.Number)
                    .ToList();

               
                var takenSeatIds = projection.Tickets?.Select(t => t.SeatId).ToList() ?? new List<int>();

               
                var freeSeatIds = allSeats
                    .Where(s => !takenSeatIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToList();

                if (!freeSeatIds.Any())
                {
                    TempData["Error"] = "Няма свободни места за тази прожекция.";
                    return RedirectToAction("Index", "Program");
                }

                ViewBag.Projection = projection;
                ViewBag.AllSeats = allSeats;
                ViewBag.FreeSeatIds = freeSeatIds;

                var ticket = new Ticket
                {
                    ProjectionId = projection.Id,
                    PricePaid = projection.TicketPrice
                };

                return View(ticket);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Възникна грешка: " + ex.Message;
                return RedirectToAction("Index", "Program");
            }
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ProjectionId,SeatId,PricePaid")] Ticket ticket)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ticket.UserId = userId;
                ticket.PurchasedAt = DateTime.Now;

                var seatTaken = _context.Tickets
                    .Any(t => t.ProjectionId == ticket.ProjectionId && t.SeatId == ticket.SeatId);

                if (!seatTaken)
                {
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = " Билетът беше закупен успешно!";
                    return RedirectToAction(nameof(MyTickets));
                }

                ModelState.AddModelError("SeatId", " Това място вече е заето! Моля, изберете друго.");

                var projection = _context.Projections
                    .Include(p => p.Movie)
                    .Include(p => p.Hall)
                    .Include(p => p.Tickets)
                    .FirstOrDefault(p => p.Id == ticket.ProjectionId);

                var takenSeatIds = projection.Tickets.Select(t => t.SeatId).ToList();
                var allSeats = _context.Seats
                    .Where(s => s.HallId == projection.HallId)
                    .OrderBy(s => s.Row)
                    .ThenBy(s => s.Number)
                    .ToList();

                var freeSeatIds = allSeats
                    .Where(s => !takenSeatIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToList();

                ViewBag.Projection = projection;
                ViewBag.AllSeats = allSeats;
                ViewBag.FreeSeatIds = freeSeatIds;

                return View(ticket);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Грешка при запазване: " + ex.Message;
                return RedirectToAction("Index", "Program");
            }
        }

        // GET: Tickets/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Projection).ThenInclude(p => p.Movie)
                .Include(t => t.Projection).ThenInclude(p => p.Hall)
                .Include(t => t.Seat)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ticket.UserId != userId && !User.IsInRole("Admin"))
                return Unauthorized();

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Projection)
                .Include(t => t.Seat)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            ViewData["ProjectionId"] = new SelectList(_context.Projections.Include(p => p.Movie), "Id", "Movie.Title", ticket.ProjectionId);
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Id", ticket.SeatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", ticket.UserId);

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectionId,SeatId,UserId,PricePaid,PurchasedAt")] Ticket ticket)
        {
            if (id != ticket.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectionId"] = new SelectList(_context.Projections.Include(p => p.Movie), "Id", "Movie.Title", ticket.ProjectionId);
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Id", ticket.SeatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", ticket.UserId);

            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Projection).ThenInclude(p => p.Movie)
                .Include(t => t.Projection).ThenInclude(p => p.Hall)
                .Include(t => t.Seat)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}