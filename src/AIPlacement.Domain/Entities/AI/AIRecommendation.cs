namespace AIPlacement.Domain.Entities.AI
{
    public class AIRecommendation
    {
        public int RecommendationId { get; set; }

        public int StudentId { get; set; }

        public int JobDriveId { get; set; }

        public int MatchId { get; set; }

        public string RecommendationText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}