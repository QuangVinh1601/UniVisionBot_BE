using UniVisionBot.DTOs.Major;

namespace UniVisionBot.Services.Major
{
    public interface IMajorRepository
    {
        Task<MajorResponse> CreateNewMajor(MajorRequest request);
    }
}
