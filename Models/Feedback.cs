using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class Feedback
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Key]
        public string Id { get; set; }
        [BsonElement("overall_feedback")]
        public string? overallFeedback { set; get; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("user_id")]
        public string userId { get; set; }
        [BsonElement("rating")]
        public Ratings instance { get; set; }
        [BsonElement("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
    public class Ratings
    {
        public int userInterface { set; get; }

        public int functionality { set; get; }

        public int performance { set; get; }

        public int usefulness { set; get; }
    }
}
