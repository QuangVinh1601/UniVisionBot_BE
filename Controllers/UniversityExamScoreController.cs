﻿using Microsoft.AspNetCore.Http;
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
        [HttpGet("search")]
        public async Task<IActionResult> GetTitleUniversityExamScoreFromSearching([FromQuery] UniversityExamScoreRequest request)
        {
            var result = await _universityExamScoreRepository.GetTiileBySearching(request);
            return Ok(result);
        }
        [HttpGet("getscore")]
        public async Task<IActionResult> GetScoreExamOfEachUniversityByYear([FromQuery] ScoreExamOfEachUniversityByYearRequest request)
        {
            var result = await _universityExamScoreRepository.GetExamScoreByYear(request);
            return Ok(result);
        }
    }
}
