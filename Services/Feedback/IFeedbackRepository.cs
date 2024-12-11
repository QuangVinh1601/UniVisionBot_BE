using UniVisionBot.DTOs.Feedback;

namespace UniVisionBot.Services.Feedback
{
    public interface IFeedbackRepository
    {
        Task AddNewFeedback(FeedbackRequest request);
        Task<List<FeedbackResponse>> GetAllFeedback(int page);
        Task<FeedbackResponse> GetFeedbackById(string feedbackId);

    }
}
