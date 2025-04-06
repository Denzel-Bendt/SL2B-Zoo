using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zoo.Data;
using Zoo.Models;

namespace Zoo.Controllers
{
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
                .Include(a => a.Category)
                .AsNoTracking()
                .OrderBy(a => a.Name)
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
                    IsEating = CheckFeedingTime(currentHour, a.FeedingSchedule)
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
        public async Task<IActionResult> Create([Bind("Id,Name,Species,Age,ActivityPattern,EnclosureId,CategoryId,FeedingSchedule")] Animal animal)
        {
            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Add(animal);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            LogModelErrors();
            PopulateEnclosuresDropDownList(animal.EnclosureId);
            PopulateCategoriesDropDownList(animal.CategoryId);
            return View(animal);
        }

        // GET: Animal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            PopulateEnclosuresDropDownList(animal.EnclosureId);
            PopulateCategoriesDropDownList(animal.CategoryId);
            return View(animal);
        }

        // POST: Animal/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Species,Age,ActivityPattern,EnclosureId,CategoryId,FeedingSchedule")] Animal animal)
        {
            if (id != animal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAnimal = await _context.Animals.FindAsync(id);
                    if (existingAnimal == null)
                    {
                        return NotFound();
                    }

                    existingAnimal.Name = animal.Name;
                    existingAnimal.Species = animal.Species;
                    existingAnimal.Age = animal.Age;
                    existingAnimal.ActivityPattern = animal.ActivityPattern;
                    existingAnimal.EnclosureId = animal.EnclosureId;
                    existingAnimal.CategoryId = animal.CategoryId;
                    existingAnimal.FeedingSchedule = animal.FeedingSchedule;

                    _context.Entry(existingAnimal).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AnimalExistsAsync(animal.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Concurrentiewijziging gedetecteerd. Herlaad de pagina en probeer opnieuw.");
                      
                    }
                }
            }

            LogModelErrors();
            PopulateEnclosuresDropDownList(animal.EnclosureId);
            PopulateCategoriesDropDownList(animal.CategoryId);
            return View(animal);
        }

        // GET: Animal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

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

        private async Task<bool> AnimalExistsAsync(int id)
        {
            return await _context.Animals
                .AsNoTracking()
                .AnyAsync(e => e.Id == id);
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

        private bool CheckFeedingTime(int currentHour, string feedingSchedule)
        {
            // Implementeer je eigen logica voor voedertijden
            return false;
        }

        private void LogModelErrors()
        {
            foreach (var entry in ModelState)
            {
                if (entry.Value.Errors.Count > 0)
                {
                    Console.WriteLine($"Model error: {entry.Key} - {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }
        }
    }
}