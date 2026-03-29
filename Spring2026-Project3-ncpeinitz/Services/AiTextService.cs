using Azure;
using Azure.AI.OpenAI;
using Spring2026_Project3_ncpeinitz.ViewModels;
using Spring2026_Project3_ncpeinitz.Models;
using VaderSharp2;
using OpenAI;
using OpenAI.Chat;

namespace Spring2026_Project3_ncpeinitz.Services
{
    public class AiTextService
    {
        private readonly AzureOpenAIClient _client;
        private readonly IConfiguration _config;
        private readonly SentimentIntensityAnalyzer _analyzer = new();

        public AiTextService(AzureOpenAIClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task<List<TextSentiment>>GenerateMovieReviewsAsync(Movie movie, List<string> actorNames)
        {
            string actorText = actorNames.Count == 0
                ? "No actors listed"
                : string.Join(", ", actorNames);

            string prompt = $"""
                Write exactly 5 short reviews for this movie. Return exactly one review per line. Do not number the lines.
                Movie: {movie.Title}
                Genre: {movie.Genre}
                Year: {movie.Year_Released}
                Actors in Movie: {actorText}
                Keep each review to 1-2 sentences.
                """;

            var lines = await GetAiLinesAsync(prompt);
            return lines.Take(5).Select(ToSentiment).ToList();
        }

        public async Task<List<TextSentiment>> GenerateActorTweetsAsync(Actor actor, List<string> movieTitles)
        {
            string movieText = movieTitles.Count == 0
                ? "No movies listed"
                : string.Join(", ", movieTitles);

            string prompt = $"""
                Write exactly 10 short tweets about this actor. Return exactly one tweet per line. Do not number the lines.
                Actor: {actor.Name}
                Gender: {actor.Gender}
                Age: {actor.Age}
                Known movies: {movieText}
                Keep each review to 220 characters.
                """;

            var lines = await GetAiLinesAsync(prompt);
            return lines.Take(10).Select(ToSentiment).ToList();
        }

        private async Task<List<string>> GetAiLinesAsync(string prompt)
        {
            string deployment = _config["AzureOpenAI:Deployment"]
                ?? throw new InvalidOperationException("Missing AzureOpenAI:Deployment");

            ChatClient chatClient = _client.GetChatClient(deployment);

            List<ChatMessage> messages = new()
            {
                new SystemChatMessage("Follow the requested output formate exactly."),
                new UserChatMessage(prompt)
            };

            ChatCompletionOptions options = new()
            {
                Temperature = 0.7f,
                MaxOutputTokenCount = 700
            };

            ChatCompletion response = await chatClient.CompleteChatAsync(messages, options);

            string content = response.Content[0].Text;

            return content
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(RemovePrefix)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        private TextSentiment ToSentiment(string text)
        {
            var score = _analyzer.PolarityScores(text);

            return new TextSentiment
            {
                Text = text,
                Score = score.Compound,
                Label = ToLabel(score.Compound)
            };
        }

        private static string RemovePrefix(string text)
        {
            text = text.Trim();

            if (text.Length > 2 && char.IsDigit(text[0]) && (text[1] == '.' || text[1] == ')'))
                return text.Substring(2).Trim();

            return text.TrimStart('-', '*', ' ');
        }

        public static double AverageScore(IEnumerable<TextSentiment> items) =>
            items.Any() ? items.Average(i => i.Score) : 0;

        public static string ToLabel(double compound)
        {
            if (compound >= 0.05) return "Positive";
            if (compound <= -0.05) return "Negative";
            return "Neutral";
        }
    }
}
