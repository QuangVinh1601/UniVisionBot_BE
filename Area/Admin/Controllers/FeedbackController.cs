using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniVisionBot.DTOs.Feedback;
using UniVisionBot.Exceptions;
using UniVisionBot.Services.Feedback;

namespace UniVisionBot.Area.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        public FeedbackController(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Create(FeedbackRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new BadInputException("Invalid input");
            }
            await _feedbackRepository.AddNewFeedback(request);
            return Ok(request);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllFeedback()
        {
            var feedbackList = await _feedbackRepository.GetAllFeedback();
            return Ok(feedbackList);
        }
        [HttpGet("{feedbackId}")]
        public async Task<IActionResult> GetFeedbackById(string feedbackId)
        {
            var feedback = await _feedbackRepository.GetFeedbackById(feedbackId);
            return Ok(feedback);
        }
    }
}
