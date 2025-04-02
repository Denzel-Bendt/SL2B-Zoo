using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zoo.Data;
using Zoo.Models;

public class AnimalController : Controller
{
    private readonly ZooContext _context;

    public AnimalController(ZooContext context)
    {
        _context = context;
    }

    // GET: Animal
    public async Task<IActionResult> Index()
    {
        var animals = await _context.Animals
            .Include(a => a.Enclosure)
            .Include(a => a.Category)  // Added category inclusion
            .AsNoTracking()  // Better for read-only operations
            .ToListAsync();

        return View(animals);
    }

    // GET: Animal/Status
    public async Task<IActionResult> Status()
    {
        var currentHour = DateTime.Now.Hour;
        var isDaytime = currentHour > 6 && currentHour < 18;

        var animals = await _context.Animals
            .Select(a => new AnimalStatusVM
            {
                Id = a.Id,
                Name = a.Name,
                IsActive = a.ActivityPattern == ActivityPattern.Diurnal && isDaytime,
                IsEating = ShouldBeEating(currentHour, a.FeedingSchedule) // Implement this method
            })
            .AsNoTracking()
            .ToListAsync();

        return View(animals);
    }

    // GET: Animal/Create
    public IActionResult Create()
    {
        PopulateEnclosuresDropDownList();
        PopulateCategoriesDropDownList();
        return View();
    }

    // POST: Animal/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Species,Age,EnclosureId,CategoryId")] Animal animal)
    {
        if (ModelState.IsValid)
        {
            _context.Add(animal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateEnclosuresDropDownList(animal.EnclosureId);
        PopulateCategoriesDropDownList(animal.CategoryId);
        return View(animal);
    }

    // GET: Animal/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var animal = await _context.Animals.FindAsync(id);
        if (animal == null) return NotFound();

        PopulateEnclosuresDropDownList(animal.EnclosureId);
        PopulateCategoriesDropDownList(animal.CategoryId);
        return View(animal);
    }

    // POST: Animal/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Species,Age,EnclosureId,CategoryId")] Animal animal)
    {
        if (id != animal.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(animal);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(animal.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        PopulateEnclosuresDropDownList(animal.EnclosureId);
        PopulateCategoriesDropDownList(animal.CategoryId);
        return View(animal);
    }

    // GET: Animal/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var animal = await _context.Animals
            .Include(a => a.Enclosure)
            .Include(a => a.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (animal == null) return NotFound();

        return View(animal);
    }

    // POST: Animal/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        _context.Animals.Remove(animal);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AnimalExists(int id)
    {
        return _context.Animals.Any(e => e.Id == id);
    }

    private void PopulateEnclosuresDropDownList(object selectedEnclosure = null)
    {
        var enclosuresQuery = _context.Enclosures
            .OrderBy(e => e.Name)
            .AsNoTracking();

        ViewBag.EnclosureId = new SelectList(enclosuresQuery, "Id", "Name", selectedEnclosure);
    }

    private void PopulateCategoriesDropDownList(object selectedCategory = null)
    {
        var categoriesQuery = _context.Categories
            .OrderBy(c => c.Name)
            .AsNoTracking();

        ViewBag.CategoryId = new SelectList(categoriesQuery, "Id", "Name", selectedCategory);
    }

    private bool ShouldBeEating(int currentHour, string feedingSchedule)
    {
        // Implement your feeding schedule logic here
        // Example: return true if currentHour matches feeding times
        return false;
    }
}