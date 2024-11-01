using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class QuizResult
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? UserId { get; set; } // Reference to User

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId QuizId { get; set; } // Reference to Quiz

        public BsonDocument? ResultData { get; set; }
        public int? Score { get; set; }
        public DateTime ? CreatedAt { get; set; }
    }
}
