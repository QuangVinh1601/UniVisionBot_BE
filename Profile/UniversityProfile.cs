using AutoMapper;
using UniVisionBot.DTOs.University;
using UniVisionBot.Models;

namespace UniVisionBot.Profiles
{
    public class UniversityProfile : Profile
    {
        public UniversityProfile()
        {
            CreateMap<University, UniversityResponse>();
            CreateMap<UniversityRequest, University>();
        }
    }
}
