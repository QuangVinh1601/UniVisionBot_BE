using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class SuggestedUniversities
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? UserId { get; set; } // Reference to User

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? UniversityId { get; set; } // Reference to University

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? FacultyId { get; set; } // Reference to Faculty

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? MajorId { get; set; } // Reference to Major

        public decimal? MatchedScore { get; set; } // Indicates how well the university matches user preferences
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
