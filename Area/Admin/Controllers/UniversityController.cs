using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using UniVisionBot.DTOs.University;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.Universities;

namespace UniVisionBot.Area.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityController : ControllerBase
    {
        private readonly IUniversityRepository _universityRepository;
        public UniversityController(IUniversityRepository universityRepository)
        {
            _universityRepository = universityRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewUniversity(UniversityRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var university = new University()
                {
                    Description = model.Description,
                    Name = model.Name,
                    Location = model.Location,
                    UniversityCode = model.UniversityCode,
                    ScholarshipsAvailable = model.ScholarshipsAvailable,
                };
                await _universityRepository.CreateAsync(university);
                return Ok(university);
            }
            catch(BadInputException ex)
            {
                return BadRequest("Existed university");
            }
        }
        [HttpGet("id")]
        public IActionResult GetUniversityById(string id)
        {
            var resultUniversity = _universityRepository.GetById(id);
            return Ok(resultUniversity);
        }
        [HttpGet]
        public IActionResult GetAllUniversity([FromQuery] int? page)
        {
            var university = _universityRepository.GetAllUniversity(page);
            return Ok(university);
        }
        [HttpPut("id")]
        public async Task<IActionResult> UpdateUniversity([FromBody] UniversityRequest request, string id)
        {
            try
            {
                var university = await _universityRepository.UpdateAsync(request, id);
                return Ok(university);
            }
            catch(BadInputException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteUniversity(string id)
        {
            await _universityRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
