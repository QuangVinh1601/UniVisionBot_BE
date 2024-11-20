using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using UniVisionBot.Models;

namespace UniVisionBot.DTOs.Feedback
{
    public class FeedbackRequest
    {
        [Required(ErrorMessage = "OverallFeedback không được để trống.")]
        [StringLength(500, ErrorMessage = "OverallFeedback không được vượt quá 500 ký tự.")]
        public string OverallFeedback { set; get; }
        [StringLength(50, ErrorMessage = "UserId không được vượt quá 50 ký tự.")]
        public string UserId { get; set; }
        public Ratings Rating { get; set; }
    }
}
