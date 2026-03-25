using Spring2026_Project3_ncpeinitz.Models;

namespace Spring2026_Project3_ncpeinitz.ViewModels
{
    public class ActorDetails
    {
        public Actor Actor { get; set; } = new();
        public List<string> Movies { get; set; } = new();
        public List<TextSentiment> Tweets { get; set; } = new();
        public string GeneralLabel { get; set; } = "";
        public double AverageScore { get; set; }
    }
}
