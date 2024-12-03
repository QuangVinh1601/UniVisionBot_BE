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
            var result = await _majorRepository.CreateNewMajorAsync(request);
            return Ok(result);
        }
        [HttpPut("{majorId}")]
        public async Task<IActionResult> UpdateMajor([FromBody] MajorRequest request, string majorId)
        {
            if (!ModelState.IsValid)
            {
                throw new BadInputException("Invalid input");
            }
            var result = await _majorRepository.UpdateAsync(request, majorId);
            return Ok(result);
        }
        [HttpDelete("{majorId}")]
        public async Task<IActionResult> DeleteMajor(string majorId, string facultyId)
        {
            await _majorRepository.DeleteAsync(majorId, facultyId);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMajorOfFaculty(string facultyId, [FromQuery] int? page)
        {
            var result = await _majorRepository.GetMajorsbyFacultyIdAsync(facultyId, page);
            return Ok(result);
        }
        [HttpGet("{majorId}")]
        public IActionResult GetParticularMajor(string majorId)
        {
            var major =  _majorRepository.GetMajorById(majorId);
            return Ok(major);
        }
        


    }
}
