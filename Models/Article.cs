using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class Article
    {
        [BsonId]
        [Key]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("author")]
        public string Author { get; set; }
        [BsonElement("content")]
        public string Content { get; set; }
        [BsonElement("urlImage")]
        public Dictionary<string, string> UrlImage { get; set; }
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }  
    }
}
