using Spring2026_Project3_ncpeinitz.Models;

namespace Spring2026_Project3_ncpeinitz.ViewModels
{
    public class MovieDetails
    {
        public Movie Movie { get; set; } = new();
        public List<string> Actors { get; set; } = new();
        public List<TextSentiment> Reviews { get; set;  } = new();
        public string GeneralLabel { get; set; } = "";
        public double AverageScore { get; set; }
    }
}
