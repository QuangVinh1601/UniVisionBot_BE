using MongoDB.Bson;
using UniVisionBot.DTOs.UniversityExamScore;

namespace UniVisionBot.Services.IUniversityExamScore
{
    public interface IUniversityExamScoreRepository
    {
        Task<List<UniversityExamScoreResponse>> GetTitle();
        Task<List<UniversityExamScoreResponse>> GetTiileBySearching(UniversityExamScoreRequest request);
    }
}
