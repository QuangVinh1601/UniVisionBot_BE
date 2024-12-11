using System.ComponentModel.DataAnnotations;

namespace UniVisionBot.DTOs.Article
{
    public class ArticleRequest
    {
        [Required(ErrorMessage = "Content is required.")]
        [MinLength(50, ErrorMessage = "Content must be at least 50 characters long.")]
        [MaxLength(20000, ErrorMessage = "Content cannot exceed 20000 characters.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "An image file is required.")]
        [DataType(DataType.Upload)]
        public IFormFile ImageFile { get; set; }

        public string PublicId { get; set; }

        public string Title { get; set; }

        [Required(ErrorMessage = "Author name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Author name must be between 3 and 100 characters.")]
        public string Author { get; set; }

    }
}
