using UniVisionBot.DTOs.Faculty;

namespace UniVisionBot.Services.Faculty
{
    public interface IFacultyRepository
    {
        Task<FacultyResponse> CreateAsync(FacultyRequest request, string universityId);
        Task<FacultyResponse> UpdateAsync(FacultyRequest request, string universityId, string facultyId);
        Task DeleteAsync(string universityId, string facultyId);
        Task<List<FacultyResponse>> GetAllFacultyOfUniversity(string universityId, int? page);
    }
}
