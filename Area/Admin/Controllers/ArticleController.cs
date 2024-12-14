using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using UniVisionBot.DTOs.Article;
using UniVisionBot.Exceptions;
using UniVisionBot.Services.Article;
using UniVisionBot.Services.Image;

namespace UniVisionBot.Area.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IImageRepository _imageRepository;
        public ArticleController(IArticleRepository articleRepository, IImageRepository imageRepository)
        {
            _articleRepository = articleRepository;
            _imageRepository = imageRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ArticleRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new BadInputException("Invalid input");
            }
            var urlImage = await _imageRepository.GetUrlImage(request.ImageFile);
            if (urlImage == null)
            {
                throw new Exception("Has any error adding image");
            }
            await _articleRepository.CreateNewArticle(request, urlImage);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllArticle()
        {
            var articleList = await _articleRepository.GetAllArticle();
            return Ok(articleList);
        }
        [HttpGet("{articleId}")]
        public IActionResult GetArticleById(string articleId)
        {
            var article = _articleRepository.GetArticleById(articleId);
            return Ok(article);
        }
        [HttpDelete("{articleId}/{publicId}")]
        //Truyền vào 1 list publicId
        public async Task<IActionResult> DeleteArticleById(string articleId, string publicId)
        {
             var decodepublicId = HttpUtility.UrlDecode(publicId);  
             var result =  await _imageRepository.DeleteImageById(decodepublicId);
             await _articleRepository.DeleteArticleById(articleId);
             return Ok(result);
        }
        [HttpPut("{articleId}")]
        public async Task<IActionResult> UpdateArticle(string articleId, [FromForm] ArticleRequest request)
        {
            if(!ModelState.IsValid)
            {
                throw new BadInputException("Invalid input");
            }
            await _articleRepository.UpdateArticle(articleId,request);
            return Ok();
        } 
    }
}
