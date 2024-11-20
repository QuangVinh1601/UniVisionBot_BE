using MongoDB.Bson.Serialization.Attributes;
using UniVisionBot.Models;

namespace UniVisionBot.DTOs.Feedback
{
    public class FeedbackResponse
    {
        public string Id { get; set; }
        public string OverallFeedback { set; get; }
        public string UserId { get; set; }
        public Ratings Rating { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
