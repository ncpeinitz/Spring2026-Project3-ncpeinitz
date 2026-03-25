using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spring2026_Project3_ncpeinitz.Data;
using Spring2026_Project3_ncpeinitz.Models;
using Spring2026_Project3_ncpeinitz.ViewModels;

namespace Spring2026_Project3_ncpeinitz.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AiTextService _aiTextService;

        public MoviesController(ApplicationDbContext context, AiTextService aiTextService)
        {
            _context = context;
            _aiTextService = aiTextService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(mov => mov.ActorMovies)
                .ThenInclude(act_mov => act_mov.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            var actorNames = movie.ActorMovies
                .Where(act_mov => act_mov.Actor != null)
                .Select(act_mov => act_mov.Actor!.Name)
                .ToList();

            var reviews = await _aiTextService.GenerateMovieReviewsAsync(movie, actorNames);

            double average = AiTextService.AverageCompound(reviews);

            var view = new MovieDetails
            {
                Movie = movie,
                Actors = actorNames,
                Reviews = reviews,
                AverageCompound = average,
                OverallLabel = AiTextService.ToLabel(average)
            };

            return View(view);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Year_Released,IMDbUrl")] Movie movie, IFormFile? posterFile)
        {
            if (posterFile != null && posterFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await posterFile.CopyToAsync(ms);
                movie.Poster = ms.ToArray();
            }

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,Year_Released,IMDbUrl")] Movie movie, IFormFile? posterFile)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            var dbMovie = await _context.Movie.FindAsync(id);
            if (dbMovie == null) return NotFound();

            if (ModelState.IsValid)
            {
                dbMovie.Title = movie.Title;
                dbMovie.IMDbUrl = movie.IMDbUrl;
                dbMovie.Genre = movie.Genre;
                dbMovie.Year_Released = movie.Year_Released;

                if (posterFile != null && posterFile.Length > 0)
                {
                    using var memory_stream1 = new MemoryStream();
                    await posterFile.CopyToAsync(memory_stream1);
                    dbMovie.Poster = memory_stream1.ToArray();
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
