using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniVisionBot.DTOs.Faculty;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.Faculty;

namespace UniVisionBot.Area.Admin.Controllers
{
    [ApiController]
    [Route("api/universities/{universityId}/faculties")]
    [Authorize(Roles ="ADMIN")]
    public class FacultyController : Controller
    {
        private readonly IFacultyRepository _facultyRepository;
        public FacultyController(IFacultyRepository facultyRepository)
        {
            _facultyRepository = facultyRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateFaculty([FromBody] FacultyRequest request, string universityId)
        {
            if (!ModelState.IsValid)
            {
                throw new BadInputException("Invalid input");
            }
            var result = await _facultyRepository.CreateAsync(request, universityId);
            return result.Success ? Ok(result) : BadRequest("Failed to created");
        }
        [HttpPut("{facultyId}")]
        public async Task<IActionResult> UpdateFaculty([FromBody] FacultyRequest request, string universityId, string facultyId)
        {
            if (!ModelState.IsValid)
            {
                throw new BadInputException("Invalid input");
            }
            var result = await _facultyRepository.UpdateAsync(request, universityId, facultyId);
            return result.Success ? Ok(result) : BadRequest("Failed to update");
        }
        [HttpDelete("{facultyId}")]
        public async Task<IActionResult> DeleteFaculty(string universityId, string facultyId)
        {
            await _facultyRepository.DeleteAsync(universityId, facultyId);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetFacultyByUniversityId(string universityId, [FromQuery] int? page)
        {
            var result = await _facultyRepository.GetAllFacultyOfUniversity(universityId, page);
            return Ok(result);
        }
    }
}
