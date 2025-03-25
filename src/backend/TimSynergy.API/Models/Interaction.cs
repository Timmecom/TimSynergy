using Newtonsoft.Json;

namespace TimSynergy.API.Models
{
    public class Interaction
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string CustomerId { get; set; } = string.Empty;
        
        public DateTime InteractionDate { get; set; } = DateTime.UtcNow;
        
        public InteractionType Type { get; set; } = InteractionType.Other;
        
        public string Subject { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string CreatedBy { get; set; } = string.Empty;
        
        // AI-generated summary of the interaction
        public string Summary { get; set; } = string.Empty;
        
        // AI-generated sentiment score (-1.0 to 1.0)
        public double? SentimentScore { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum InteractionType
    {
        Email,
        Call,
        Meeting,
        SocialMedia,
        Support,
        Sales,
        Other
    }
}
