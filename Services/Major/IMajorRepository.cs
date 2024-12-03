using UniVisionBot.DTOs.Major;

namespace UniVisionBot.Services.Major
{
    public interface IMajorRepository
    {
        Task<MajorResponse> CreateNewMajorAsync(MajorCreateRequest request);
        Task<MajorResponse> UpdateAsync(MajorRequest request, string majorId);
        Task DeleteAsync(string majorId, string facultyId);
        Task<List<MajorResponse>> GetMajorsbyFacultyIdAsync(string facultyId, int? page);
        MajorResponse GetMajorById(string majorId);
    }

}
