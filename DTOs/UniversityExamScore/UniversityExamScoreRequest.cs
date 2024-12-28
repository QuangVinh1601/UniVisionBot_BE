namespace UniVisionBot.DTOs.UniversityExamScore
{
    public class UniversityExamScoreRequest
    {
        public string? SearchTerm { get; set; }
        public string? Year { get; set; }
    }
    public class ScoreExamOfEachUniversityByYearRequest
    {
        public string Year { get; set; }
        public string UniversityCode { get; set; }
    }
}
