using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoo.Data;  // Namespace waar ZooContext zich bevindt
using Zoo.Models; // Namespace voor je modellen
using Microsoft.EntityFrameworkCore;

public class AnimalController : Controller
{
    private readonly ZooContext _context;

    public AnimalController(ZooContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var animals = await _context.Animals.Include(a => a.Enclosure).ToListAsync();
        return View(animals);
    }

    public IActionResult Status()
    {
        var animals = _context.Animals
            .Select(a => new AnimalStatusVM
            {
                Id = a.Id,
                Name = a.Name,
                IsActive = a.ActivityPattern == ActivityPattern.Diurnal && DateTime.Now.Hour > 6 && DateTime.Now.Hour < 18,
                IsEating = false // Logica voor voedingstijd
            }).ToList();

        return View(animals);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Animal animal)
    {
        if (ModelState.IsValid)
        {
            _context.Add(animal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(animal);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var animal = await _context.Animals.FindAsync(id);
        if (animal == null) return NotFound();

        return View(animal);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Animal animal)
    {
        if (id != animal.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(animal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(animal);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal != null)
        {
            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
