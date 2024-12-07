using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.Models
{
    public class Major
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("faculty_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FacultyId { get; set; } // Reference to Faculty

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("level")]
        public string Level { get; set; } // e.g., Bachelor's, Master's

        [BsonElement("duration")]
        public int Duration { get; set; } // Duration in years

        [BsonElement("major_code")]
        public string MajorCode { get; set; } // Mã ngành

        [BsonElement("subject_combinations")]
        public List<string> SubjectCombinations { get; set; } // Danh sách mã tổ hợp môn

        [BsonElement("entry_score_exam")]
        public Dictionary<int, string> EntryScoreExam { get; set; } // Điểm thi đại học theo từng năm

        [BsonElement("entry_score_record")]
        public Dictionary<int, string> EntryScoreRecord { get; set; } // Điểm học bạ theo từng năm

        [BsonElement("tuition_fee")]
        public decimal TuitionFee { get; set; } // Học phí theo từng năm

        [BsonElement("notes")]
        public string Notes { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}
