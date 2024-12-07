namespace UniVisionBot.DTOs.UniversityExamScore
{
    public class UniversityExamScoreRequest
    {
        public string SearchTerm { get; set; }
        public int Year { get; set; }
    }
    public class ScoreExamOfEachUniversityByYearRequest
    {
        public int Year { get; set; }
        public string UniversityCode { get; set; }
    }
}
