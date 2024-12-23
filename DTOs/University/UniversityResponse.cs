using MongoDB.Bson;

namespace UniVisionBot.DTOs.University
{
    public class UniversityResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string UniversityCode { get; set; }
        public bool ScholarshipsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
