using AutoMapper;
using UniVisionBot.DTOs.Major;
using UniVisionBot.Models;

namespace UniVisionBot.Profiles
{
    public class MajorProfile : Profile
    {
        public MajorProfile()
        {
            CreateMap<MajorRequest,Major>();
            CreateMap<Major,MajorResponse>();
        }
    }
}
