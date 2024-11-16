using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class Conversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("consultant_Id")]
        public string ConsultantId { get; set; }
        [BsonElement("user_Id")]
        public string UserId { get; set; }
        [BsonElement("created_at")]
        public DateTime CreateAt { get; set; }

    }
}
