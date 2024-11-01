using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class UserPreference
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? UserId { get; set; } // Reference to User

        public string? PreferredFieldOfStudy { get; set; }
        public string? PreferredLocation { get; set; }
        public decimal? AcademicScore { get; set; }
        public decimal? Budget { get; set; }
        public int? PreferredUniversityRanking { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
