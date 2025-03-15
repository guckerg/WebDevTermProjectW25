using Microsoft.AspNetCore.Mvc;
using EventManager.Data;

namespace EventManager.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        //query for list of events
        var events = _context.Events.ToList();
        return View(events);
    }
}
