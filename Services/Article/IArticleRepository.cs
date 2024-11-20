﻿using UniVisionBot.DTOs.Article;

namespace UniVisionBot.Services.Article
{
    public interface IArticleRepository
    {
        Task CreateNewArticle(ArticleRequest request, Dictionary<string,string> listUrlImage);
        Task<List<ArticleResponse>> GetAllArticle();
        ArticleResponse GetArticleById(string articleId);
        Task DeleteArticleById(string articleId);
        Task UpdateArticle(string articleId, string publicId, ArticleRequest request);
    }
}