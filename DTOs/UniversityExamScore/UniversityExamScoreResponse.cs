namespace UniVisionBot.DTOs.UniversityExamScore
{
    public class UniversityExamScoreResponse
    {
        public string Title { get; set; }
        public string UniversityName { get; set; }
        public string UniversityCode { get; set; }
        public string Year { get; set; }
    }
    public class ScoreExamOfEachUniversityByYearResponse
    {
        public string Title { get; set; }
        public string UniversityCode { get; set; }
        public string UniversityName { get; set; }
        public string Year { get; set; }
        public List<MajorInfo> Majors { get; set; }
    }
    public class MajorInfo
    {
        public string MajorName { get; set; }
        public string MajorCode { get; set; }
        public List<string> SubjectCombinations { get; set; }
        public string? EntryScoreExam { get; set; }
        public string? EntryScoreRecord { get; set; }
        public string Notes { get; set; }
    }
}
