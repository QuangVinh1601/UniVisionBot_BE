using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("user_id")]
        public ObjectId UserId { get; set; } // Reference to User

        [BsonElement("university_id")]
        public ObjectId UniversityId { get; set; } // Reference to University

        [BsonElement("faculty_id")]
        public ObjectId FacultyId { get; set; } // Reference to Faculty

        [BsonElement("major_id")]
        public ObjectId MajorId { get; set; } // Reference to Major

        [BsonElement("content")]
        public string Content { get; set; } // Nội dung đánh giá

        [BsonElement("rating")]
        public int Rating { get; set; } // Đánh giá (ví dụ: từ 1 đến 5)

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
