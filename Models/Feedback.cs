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
        public string OverallFeedback { set; get; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("user_id")]
        public string UserId { get; set; }
        [BsonElement("rating")]
        public Ratings Rating { get; set; }
        [BsonElement("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
    public class Ratings
    {
        [Range(1, 5, ErrorMessage = "InterfaceInteraction phải nằm trong khoảng từ 1 đến 5.")]
        public int InterfaceInteraction { set; get; }

        [Range(1, 5, ErrorMessage = "ResponseSpeed phải nằm trong khoảng từ 1 đến 5.")]
        public int ResponseSpeed { set; get; }

        [Range(1, 5, ErrorMessage = "InformationRelevance phải nằm trong khoảng từ 1 đến 5.")]
        public int InformationRelevance { set; get; }

        [Range(1, 5, ErrorMessage = "ContextUnderstanding phải nằm trong khoảng từ 1 đến 5.")]
        public int ContextUnderstanding { set; get; }
    }
}
