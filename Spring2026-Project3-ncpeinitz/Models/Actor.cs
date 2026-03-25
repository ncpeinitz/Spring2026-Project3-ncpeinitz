using System.ComponentModel.DataAnnotations;

namespace Spring2026_Project3_ncpeinitz.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        [Range(0, 150)]
        public int Age { get; set; }

        [Required, StringLength(10)]
        public string Gender { get; set; } = "";

        [Required, Url, Display(Name = "IMDb Link")]
        public string IMDbUrl { get; set; } = "";

        public byte[]? Photo { get; set; }
        public List<ActorMovie> ActorMovies { get; set; } = new();


    }
}
