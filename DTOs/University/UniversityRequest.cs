using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.DTOs.University
{
    public class UniversityRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(100, ErrorMessage = "Location can't be longer than 100 characters")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "University code is required")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
        public string UniversityCode { get; set; }

        [Required(ErrorMessage = "Scholarships availability status is required")]
        public bool ScholarshipsAvailable { get; set; }

        [Required(ErrorMessage = "Creation date is required")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

  
    }
}
