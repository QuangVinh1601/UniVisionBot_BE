namespace UniVisionBot.DTOs.Major
{
    public class MajorResponse
    {
        public string Id { get; set; }
        public string FacultyId { get; set; }
        public List<string> CareerIds { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public int Duration { get; set; }
        public string MajorCode { get; set; }
        public List<string> SubjectCombinations { get; set; }

        public Dictionary<string, int> EntryScoreExam { get; set; }
        public Dictionary<string, int> EntryScoreRecord { get; set; }

        public decimal TuitionFee { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool? Success { get; set; }
        public string? Message { get; set; }
    }
}

