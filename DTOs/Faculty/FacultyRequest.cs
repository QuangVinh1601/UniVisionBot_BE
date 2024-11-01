using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.DTOs.Faculty
{
    public class FacultyRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Creation date is required")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Update date is required")]
        [DataType(DataType.Date)]
        public DateTime UpdatedAt { get; set; }
    }   
}
