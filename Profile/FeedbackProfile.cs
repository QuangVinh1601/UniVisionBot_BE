using AutoMapper;
using UniVisionBot.DTOs.Feedback;
using UniVisionBot.Models;

namespace UniVisionBot.Profiles
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<FeedbackRequest, Feedback>();
            CreateMap<Feedback, FeedbackResponse>();
        }
    }
}
