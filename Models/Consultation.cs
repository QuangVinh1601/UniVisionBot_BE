using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class Consultation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? ConsultantId { get; set; } // Reference to Consultant

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? UserId { get; set; } // Reference to User

        public DateTime? ScheduledAt { get; set; }
        public string? Status { get; set; } // e.g., Pending, Completed, Cancelled
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
