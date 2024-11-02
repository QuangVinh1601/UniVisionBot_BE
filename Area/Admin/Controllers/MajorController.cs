using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using UniVisionBot.DTOs.Major;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.Major;

namespace UniVisionBot.Area.Admin.Controllers
{
    [Route("api/faculties/{facultyId}/major")]
    [ApiController]
    public class MajorController : ControllerBase
    {
        private readonly IMajorRepository _majorRepository;
        public MajorController(IMajorRepository majorRepository)
        {
            _majorRepository = majorRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MajorRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new BadInputException("Invalid input");
            }
            var result = await _majorRepository.CreateNewMajor(request);
            return Ok(result);


        }
    }
}
