using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spring2026_Project3_ncpeinitz.Models;

namespace Spring2026_Project3_ncpeinitz.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Movie> Movie { get; set; } = default!;
        public DbSet<Actor> Actor { get; set; } = default!;
        public DbSet<ActorMovie> ActorMovie { get; set;  } = default!;

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            // Removing a Movie -> removes its ActorMovie row
            modelbuilder.Entity<ActorMovie>()
                .HasOne(actor_movie => actor_movie.Movie)
                .WithMany(movie => movie.ActorMovies)
                .HasForeignKey(actor_movie => actor_movie.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
            //Removing an Actor -> removes its ActorMovie rows
            modelbuilder.Entity<ActorMovie>()
                .HasOne(actor_movie => actor_movie.Actor)
                .WithMany(actor => actor.ActorMovies)
                .HasForeignKey(actor_movie => actor_movie.ActorId)
                .OnDelete(DeleteBehavior.Cascade);
            //Prevent duplicate pairs
            modelbuilder.Entity<ActorMovie>()
                .HasIndex(actor_movie => new { actor_movie.ActorId, actor_movie.MovieId })
                .IsUnique();
        }
    }
}
