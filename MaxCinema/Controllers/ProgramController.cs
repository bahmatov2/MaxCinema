using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaxCinema.Data;
using MaxCinema.Models;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MaxCinema.Controllers
{
    public class ProgramController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgramController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Program - показва всички предстоящи прожекции
        public async Task<IActionResult> Index()
        {
            var projections = await _context.Projections
                .Include(p => p.Movie)
                .Include(p => p.Hall)
                .Where(p => p.StartTime > DateTime.Now)
                .OrderBy(p => p.StartTime)
                .ToListAsync();

            return View(projections);
        }

        // GET: Program/Details/5 - детайли за конкретна прожекция
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projection = await _context.Projections
                .Include(p => p.Movie)
                .Include(p => p.Hall)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (projection == null)
            {
                return NotFound();
            }

            return View(projection);
        }
    }
}