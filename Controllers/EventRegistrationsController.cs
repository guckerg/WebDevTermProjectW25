using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManager.Data;
using EventManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventManager.Data;
using EventManager.Models;

namespace EventManager.Controllers
{
    public class EventRegistrationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventRegistrationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EventRegistrations.Include(e => e.Event).Include(e => e.User);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EventRegistrations == null)
            {
                return NotFound();
            }

            var eventRegistration = await _context.EventRegistrations
                .Include(e => e.Event)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.RegistrationID == id);
            if (eventRegistration == null)
            {
                return NotFound();
            }

            return View(eventRegistration);
        }

        public IActionResult Create()
        {
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventTitle");
            ViewData["UserID"] = new SelectList(_context.Set<AppUser>(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationID,UserID,EventID")] EventRegistration eventRegistration)
        {
            if (eventRegistration.UserID != null && eventRegistration.EventID > 0)
            {
                eventRegistration.User = await _context.Set<AppUser>()
                    .FirstOrDefaultAsync(u => u.Id == eventRegistration.UserID);
                eventRegistration.Event = await _context.Events
                    .FirstOrDefaultAsync(e => e.EventID == eventRegistration.EventID);

                if (eventRegistration.User == null || eventRegistration.Event == null)
                {
                    // Handle the case where the User or Event doesn't exist
                    ModelState.AddModelError("", "Invalid User or Event selected.");
                }
                else
                {
                    _context.Add(eventRegistration);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventTitle", eventRegistration.EventID);
            ViewData["UserID"] = new SelectList(_context.Set<AppUser>(), "Id", "Name", eventRegistration.UserID);

            return View(eventRegistration);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EventRegistrations == null)
            {
                return NotFound();
            }

            var eventRegistration = await _context.EventRegistrations.FindAsync(id);
            if (eventRegistration == null)
            {
                return NotFound();
            }
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventID", eventRegistration.EventID);
            ViewData["UserID"] = new SelectList(_context.Set<AppUser>(), "Id", "Id", eventRegistration.UserID);
            return View(eventRegistration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistrationID,UserID,EventID")] EventRegistration eventRegistration)
        {
            if (id != eventRegistration.RegistrationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventRegistration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventRegistrationExists(eventRegistration.RegistrationID))
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
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventID", eventRegistration.EventID);
            ViewData["UserID"] = new SelectList(_context.Set<AppUser>(), "Id", "Id", eventRegistration.UserID);
            return View(eventRegistration);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EventRegistrations == null)
            {
                return NotFound();
            }

            var eventRegistration = await _context.EventRegistrations
                .Include(e => e.Event)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.RegistrationID == id);
            if (eventRegistration == null)
            {
                return NotFound();
            }

            return View(eventRegistration);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EventRegistrations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EventRegistrations'  is null.");
            }
            var eventRegistration = await _context.EventRegistrations.FindAsync(id);
            if (eventRegistration != null)
            {
                _context.EventRegistrations.Remove(eventRegistration);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventRegistrationExists(int id)
        {
            return (_context.EventRegistrations?.Any(e => e.RegistrationID == id)).GetValueOrDefault();
        }
    }
}
