using AutoMapper;
using UniVisionBot.DTOs.Article;
using UniVisionBot.Models;

namespace UniVisionBot.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleResponse>();
        }
    }
}
