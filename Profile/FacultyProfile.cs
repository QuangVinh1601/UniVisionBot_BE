using AutoMapper;
using UniVisionBot.DTOs.Faculty;
using UniVisionBot.Models;

namespace UniVisionBot.Profiles
{
    public class FacultyProfile : Profile
    {
        public FacultyProfile()
        {
            CreateMap<Faculty, FacultyResponse>();
            CreateMap<FacultyRequest, Faculty>();
        }
    }
}
