using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.DTOs.Major
{
    public class MajorRequest
    {

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Level is required")]
        [StringLength(50, ErrorMessage = "Level cannot exceed 50 characters")]
        public string Level { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 10, ErrorMessage = "Duration must be between 1 and 10 years")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "MajorCode is required")]
        public string MajorCode { get; set; }
     
        [Required(ErrorMessage = "SubjectCombinations are required")]
        public List<string> SubjectCombinations { get; set; }

        [Required(ErrorMessage = "EntryScoreExam is required")]
        public Dictionary<string, double> EntryScoreExam { get; set; }

        [Required(ErrorMessage = "EntryScoreRecord is required")]
        public Dictionary<string, double> EntryScoreRecord { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "TuitionFee must be a positive number")]
        public decimal TuitionFee { get; set; }

        [StringLength(200, ErrorMessage = "Notes cannot exceed 200 characters")]
        public string Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public class MajorCreateRequest
    {
        [Required]
        public string FacultyId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Level is required")]
        [StringLength(50, ErrorMessage = "Level cannot exceed 50 characters")]
        public string Level { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 10, ErrorMessage = "Duration must be between 1 and 10 years")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "MajorCode is required")]
        public string MajorCode { get; set; }

        [Required(ErrorMessage = "SubjectCombinations are required")]
        public List<string> SubjectCombinations { get; set; }

        [Required(ErrorMessage = "EntryScoreExam is required")]
        public Dictionary<string, double> EntryScoreExam { get; set; }

        [Required(ErrorMessage = "EntryScoreRecord is required")]
        public Dictionary<string, double> EntryScoreRecord { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "TuitionFee must be a positive number")]
        public decimal TuitionFee { get; set; }

        [StringLength(200, ErrorMessage = "Notes cannot exceed 200 characters")]
        public string Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
