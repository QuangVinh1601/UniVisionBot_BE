using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class University
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("university_code")]
        public string UniversityCode { get; set; }


        [BsonElement("scholarships_available")]
        public bool ScholarshipsAvailable { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}
