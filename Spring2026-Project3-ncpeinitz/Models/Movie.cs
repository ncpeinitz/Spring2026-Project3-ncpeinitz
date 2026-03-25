using System.ComponentModel.DataAnnotations;

namespace Spring2026_Project3_ncpeinitz.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = "";

        [Required, StringLength(50)]
        public string Genre { get; set; } = "";

        [Display(Name = "Year Released")]
        [Range(1800, 2100)]
        public int Year_Released { get; set; }

        [Required, Url, Display(Name = "IMDb Link")]
        public string IMDbUrl { get; set; } = "";

        public byte[]? Poster { get; set; }
        public List<ActorMovie> ActorMovies { get; set; } = new();
    }
}
