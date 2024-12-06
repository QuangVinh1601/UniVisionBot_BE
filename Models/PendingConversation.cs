using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class PendingConversation
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [Key]
        
        public string Id { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [BsonElement("conversationId")]
        public string conversationId { get; set; }
        [BsonElement("fullName")]
        public string Fullname { get; set; }
        [BsonElement("status")]
        public string Status {  get; set; }
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
