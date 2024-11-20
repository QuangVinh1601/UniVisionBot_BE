using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using UniVisionBot.Configurations.CloudinaryConfig;
using UniVisionBot.Services.Image;
namespace UniVisionBot.Repositories.Image
{
    public class ImageRepository : IImageRepository
    {
        private readonly IOptions<CloudinaryConfig> _options;
        public Account account;
        public ImageRepository(IOptions<CloudinaryConfig> options)
        {
            _options = options;
            account = new Account()
            {
                ApiKey = _options.Value.ApiKey,
                ApiSecret = _options.Value.SecretKey,
                Cloud = _options.Value.CloudName
            };
        }

        public async Task<DeletionResult> DeleteImageById(string publicId)
        {
            try
            {
                var cloudinary = new Cloudinary(account);
                var deleteResult = await cloudinary.DestroyAsync(new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image
                });
                if (deleteResult.Result != "ok")
                {
                    throw new Exception($"Xoá ảnh thất bại {deleteResult.Error?.Message}");
                }
                return deleteResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Dictionary<string, string>> GetUrlImage(IFormFile file)
        {
            Dictionary<string, string> listImageUrl = new Dictionary<string, string>();
            var cloudinary = new Cloudinary(account);
            var fileName = file.FileName;
            var path = Path.Combine("Uploads", fileName);
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(stream);
            }
            var imageParams = new ImageUploadParams
            {
                File = new FileDescription(path),
                UseFilename = true,
                UniqueFilename = true,
                Folder = "Article"
            };
            var uploadResult = await cloudinary.UploadAsync(imageParams);
            listImageUrl.Add($"{uploadResult.PublicId}", uploadResult.Url.ToString());
            return listImageUrl;

        }
    }
}

