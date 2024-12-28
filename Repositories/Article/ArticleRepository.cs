using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.DTOs.Article;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.Article;
using UniVisionBot.Services.Image;
namespace UniVisionBot.Repositories.Articles
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IMongoCollection<Article> _articleCollection;
        private readonly IOptions<MyDatabase> _options;
        private readonly IMapper _mapper;
        private readonly IImageRepository _imageRepository;
        public ArticleRepository(IOptions<MyDatabase> options, IMapper mapper, IImageRepository imageRepository)
        {
            _mapper = mapper;
            _options = options;
            var connectionString = new MongoClient(_options.Value.ConnectionString);
            var database = connectionString.GetDatabase(_options.Value.DatabaseName);
            _articleCollection = database.GetCollection<Article>(_options.Value.ArticleCollectionName);
            _imageRepository = imageRepository;
        }

        public async Task CreateNewArticle(ArticleRequest request, Dictionary<string, string>? listUrlImage)
        {
            try
            {
                var article = new Article
                {
                    Content = request.Content,
                    Author = request.Author,
                    Title = request.Title,
                    UrlImage = listUrlImage,
                    CreatedAt = DateTime.Now,
                };
                await _articleCollection.InsertOneAsync(article);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteArticleById(string articleId)
        {
            if(!ObjectId.TryParse(articleId, out ObjectId objectArticleId))
            {
                throw new BadInputException("Invalid format");
            }
            var filter = Builders<Article>.Filter.Eq(a => a.Id, articleId);
            await _articleCollection.DeleteOneAsync(filter);
        }

        public async Task<List<ArticleResponse>> GetAllArticle()
        {
            var articleList = await _articleCollection.Find(_ => true).ToListAsync();
            var articleListMap = _mapper.Map<List<Article>, List<ArticleResponse>>(articleList);
            return articleListMap;
        }
        public ArticleResponse GetArticleById(string articleId)
        {
            try
            {
                if (!ObjectId.TryParse(articleId, out ObjectId objectarticleId))
                {
                    throw new BadInputException("Invalid format");
                }
                var article = _articleCollection.AsQueryable().Where(a => a.Id == articleId).FirstOrDefault();
                var articleMap = _mapper.Map<Article, ArticleResponse>(article);
                return articleMap;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateArticle(string articleId, UpdateArticleWithImageRequest request )
        {
            try
            {
                if (!ObjectId.TryParse(articleId, out ObjectId objectArticleId))
                {
                    throw new BadInputException("Invalid format");
                }
                var article = await _articleCollection.Find(a => a.Id == articleId).FirstOrDefaultAsync();
                if (article == null)
                {
                    throw new NotFoundException("Cannot find article");
                }
                if (!article.UrlImage.ContainsKey(request.PublicId))
                {
                    throw new NotFoundException("Cannot find key");
                }
                await _imageRepository.DeleteImageById(request.PublicId);
                var articleReplace = new Article
                {
                    Id = article.Id,
                    Author = request.Author,
                    Content = request.Content,
                    Title = request.Title,
                    UrlImage = await _imageRepository.GetUrlImage(request.ImageFile),
                };
                var replaceResult = await _articleCollection.ReplaceOneAsync(Builders<Article>.Filter.Eq(a => a.Id, article.Id), articleReplace);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }

        public async Task UpdateArticleWithoutImage(string articleId, UpdateArticleWithoutImageRequest request)
        {
            try
            {
                if (!ObjectId.TryParse(articleId, out ObjectId objectArticleId))
                {
                    throw new BadInputException("Invalid format");
                }
                var article = await _articleCollection.Find(a => a.Id == articleId).FirstOrDefaultAsync();
                if (article == null)
                {
                    throw new NotFoundException("Cannot find article");
                }
                if (!article.UrlImage.ContainsKey(request.PublicId))
                {
                    throw new NotFoundException("Cannot find key");
                }
                var articleReplace = new Article
                {
                    Id = article.Id,
                    Author = request.Author,
                    Content = request.Content,
                    Title = request.Title,
                    UrlImage = article.UrlImage
                };
                var replaceResult = await _articleCollection.ReplaceOneAsync(Builders<Article>.Filter.Eq(a => a.Id, article.Id), articleReplace);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
