namespace UniVisionBot.DTOs.Article
{
    public class ArticleResponse
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public Dictionary<string,string> UrlImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
