using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManager.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventManager.Data;
using EventManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace EventManager.Controllers
{
    public class MatchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Matches
                .Include(m => m.Event)
                .Include(m => m.Player1)
                .Include(m => m.Player2);

            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Matches == null)
            {
                return NotFound();
            }

            var match = await _context.Matches
                .Include(m => m.Event)
                .FirstOrDefaultAsync(m => m.MatchID == id);
            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            //show events that havent started yet only
            ViewData["EventID"] = new SelectList(_context.Events.Where(e => !e.IsLive), "EventID", "EventTitle");

            //users with registrations only
            var registeredUsers = _context.EventRegistrations
                .Select(er => er.User)
                .Distinct(); // avoid duplicates

            ViewData["PlayerID"] = new SelectList(registeredUsers, "Id", "Name");

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MatchID,EventID")] Match match, string Player1ID, string Player2ID)
        {
            //did data come back correcty
            if (match.EventID > 0 && !string.IsNullOrEmpty(Player1ID) && !string.IsNullOrEmpty(Player2ID))
            {
                //find cooresponding AppUsers
                var player1 = await _context.Set<AppUser>().FirstOrDefaultAsync(u => u.Id == Player1ID);
                var player2 = await _context.Set<AppUser>().FirstOrDefaultAsync(u => u.Id == Player2ID);

                //Did they exist?
                if (player1 == null || player2 == null)
                {
                    ModelState.AddModelError("", "One or both players do not exist.");
                }
                //No duplicates
                if (Player1ID == Player2ID)
                {
                    ModelState.AddModelError("", "A player cannot play against themselves.");
                }
                //Populate match
                match.Player1 = player1;
                match.Player2 = player2;
                match.Event = await _context.Events.FirstOrDefaultAsync(e => e.EventID == match.EventID);

                //add to DB
                _context.Add(match);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Repopulate dropdowns on validation failure
            ViewData["EventID"] = new SelectList(_context.Events.Where(e => !e.IsLive), "EventID", "EventTitle", match.EventID);
            ViewData["PlayerID"] = new SelectList(_context.EventRegistrations.Select(er => er.User).Distinct(), "Id", "Name");
            return View(match);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Matches == null)
            {
                return NotFound();
            }

            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventID", match.EventID);
            return View(match);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MatchID,EventID")] Match match)
        {
            if (id != match.MatchID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(match);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchExists(match.MatchID))
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
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventID", match.EventID);
            return View(match);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Include(m => m.Event)
                .FirstOrDefaultAsync(m => m.MatchID == id);

            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Matches == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Matches'  is null.");
            }
            var match = await _context.Matches.FindAsync(id);
            if (match != null)
            {
                _context.Matches.Remove(match);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatchExists(int id)
        {
            return (_context.Matches?.Any(e => e.MatchID == id)).GetValueOrDefault();
        }
    }
}
