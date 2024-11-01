namespace UniVisionBot.DTOs.Faculty
{
    public class FacultyResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UniversityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Success { get; set; }
        public string Message {  get; set; } 
    }
}
