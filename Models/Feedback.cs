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
        public string overallFeedback { set; get; }

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
        [Range(1, 5, ErrorMessage = "InterfaceInteraction phải nằm trong khoảng từ 1 đến 5.")]
        public int userInterface { set; get; }

        [Range(1, 5, ErrorMessage = "ResponseSpeed phải nằm trong khoảng từ 1 đến 5.")]
        public int functionality { set; get; }

        [Range(1, 5, ErrorMessage = "InformationRelevance phải nằm trong khoảng từ 1 đến 5.")]
        public int performance { set; get; }

        [Range(1, 5, ErrorMessage = "ContextUnderstanding phải nằm trong khoảng từ 1 đến 5.")]
        public int usefulness { set; get; }
    }
}
