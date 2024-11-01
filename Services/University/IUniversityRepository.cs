using UniVisionBot.DTOs.University;
using UniVisionBot.Models;

namespace UniVisionBot.Services.Universities
{
    public interface IUniversityRepository 
    {
        Task CreateAsync(University request);
        UniversityResponse GetById(string id);

        List<UniversityResponse> GetAllUniversity(int? page);

        Task<UniversityResponse> UpdateAsync(UniversityRequest request, string id);
        Task DeleteAsync(string id);
    }
}
