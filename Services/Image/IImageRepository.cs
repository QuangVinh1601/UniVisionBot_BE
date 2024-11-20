using CloudinaryDotNet.Actions;
using MongoDB.Driver;

namespace UniVisionBot.Services.Image
{
    public interface IImageRepository
    {
        Task<Dictionary<string, string>> GetUrlImage(IFormFile file);
        Task<DeletionResult> DeleteImageById(string publicId);
    }
}
