using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using UniVisionBot.Enum;

namespace UniVisionBot.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("message")]

        public string Content {  get; set; }

        [BsonElement("conversation_id")]
        public string ConversationId { get; set; }
        [BsonElement("sender")]
        public string SenderId { get; set; }
        [BsonElement("receiver")]
        public string ReceiverId { get; set; }

        [BsonElement("status")]
        public StatusChatEnum Status = StatusChatEnum.SENT;
        [BsonElement("create_at")]

        public DateTime Created_At { get; set; }

    }
}
