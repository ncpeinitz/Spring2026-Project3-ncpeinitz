using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spring2026_Project3_ncpeinitz.Data;
using Spring2026_Project3_ncpeinitz.Models;
using Spring2026_Project3_ncpeinitz.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spring2026_Project3_ncpeinitz.Services;

namespace Spring2026_Project3_ncpeinitz.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AiTextService _aiTextService;

        public ActorsController(ApplicationDbContext context, AiTextService aiTextService)
        {
            _context = context;
            _aiTextService = aiTextService;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actor.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .Include(mov => mov.ActorMovies)
                .ThenInclude(act_mov => act_mov.Movie)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            var movieTitles = actor.ActorMovies
                .Where(act_mov => act_mov.Movie != null)
                .Select(act_mov => act_mov.Movie!.Title)
                .ToList();

            var tweets = await _aiTextService.GenerateActorTweetsAsync(actor, movieTitles);

            double average = AiTextService.AverageScore(tweets);

            var view = new ActorDetails
            {
                Actor = actor,
                Movies = movieTitles,
                Tweets = tweets,
                AverageScore = average,
                GeneralLabel = AiTextService.ToLabel(average)
            };

            return View(view);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Age,Gender,IMDbUrl")] Actor actor, IFormFile? photoFile)
        {
            if (photoFile != null && photoFile.Length > 0)
            {
                using var memory_stream = new MemoryStream();
                await photoFile.CopyToAsync(memory_stream);
                actor.Photo = memory_stream.ToArray();
            }
            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Age,Gender,IMDbUrl")] Actor actor, IFormFile? photoFile)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            var dbActor = await _context.Actor.FindAsync(id);
            if (dbActor == null) return NotFound();

            if (ModelState.IsValid)
            {
                dbActor.Name = actor.Name;
                dbActor.Gender = actor.Gender;
                dbActor.Age = actor.Age;
                dbActor.IMDbUrl = actor.IMDbUrl;

                if (photoFile != null && photoFile.Length > 0)
                {
                    using var memory_stream1 = new MemoryStream();
                    await photoFile.CopyToAsync(memory_stream1);
                    dbActor.Photo = memory_stream1.ToArray();
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.Id == id);
        }
    }
}
