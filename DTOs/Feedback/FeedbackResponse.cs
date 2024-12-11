using MongoDB.Bson.Serialization.Attributes;
using UniVisionBot.Models;

namespace UniVisionBot.DTOs.Feedback
{
    public class FeedbackResponse
    {
        public string Id { get; set; }
        public string overallFeedback { set; get; }
        public double? rating { set; get; }
        public string userId { get; set; }
        public string fullname { get; set; }
        public Ratings instance { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
