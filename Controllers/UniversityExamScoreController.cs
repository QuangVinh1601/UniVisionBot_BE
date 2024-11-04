using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniVisionBot.DTOs.UniversityExamScore;
using UniVisionBot.Services.IUniversityExamScore;

namespace UniVisionBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityExamScoreController : ControllerBase
    {
        private readonly IUniversityExamScoreRepository _universityExamScoreRepository;
        public UniversityExamScoreController(IUniversityExamScoreRepository universityExamScoreRepository)
        {
            _universityExamScoreRepository = universityExamScoreRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetTitleUniversityExamScore()
        {
            var result =  await _universityExamScoreRepository.GetTitle();
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> GetTitleUniversityExamScoreFromSearching([FromBody] UniversityExamScoreRequest request)
        {
            var result = await _universityExamScoreRepository.GetTiileBySearching(request);
            return Ok(result);
        }
    }
}
