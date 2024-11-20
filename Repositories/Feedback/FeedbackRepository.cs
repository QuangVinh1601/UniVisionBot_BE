using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.DTOs.Feedback;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.Feedback;
namespace UniVisionBot.Repositories.Feedbacks
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IMongoCollection<Feedback> _feedbackCollection;
        private readonly IOptions<MyDatabase> _options;
        private readonly IMapper _mapper; 
        public FeedbackRepository(IOptions<MyDatabase> options, IMapper mapper)
        {
            _options = options;
            var connectionString = new MongoClient(_options.Value.ConnectionString);
            var databaseName = connectionString.GetDatabase(_options.Value.DatabaseName);
            _feedbackCollection = databaseName.GetCollection<Feedback>(_options.Value.FeedbackCollectionName);
            _mapper = mapper;
        }

        public async Task AddNewFeedback(FeedbackRequest request)
        {
            if(!ObjectId.TryParse(request.UserId, out var objectUserId))
            {
                throw new BadInputException("Invalid input");
            }
            var feedbackMap = _mapper.Map<FeedbackRequest, Feedback>(request);    
            feedbackMap.CreatedAt = DateTime.Now;
            await _feedbackCollection.InsertOneAsync(feedbackMap);
        }
        public async Task<List<FeedbackResponse>> GetAllFeedback()
        {
            var feedbackList = await _feedbackCollection.Find(_ => true).ToListAsync();
            var feedbackMap = _mapper.Map<List<Feedback>, List<FeedbackResponse>>(feedbackList);
            return feedbackMap;
        }

        public async Task<FeedbackResponse> GetFeedbackById(string feedbackId)
        {
            if(!ObjectId.TryParse(feedbackId, out ObjectId objectFeedbackId))
            {
                throw new BadInputException("Invalid format");
            }
            var feedback = await _feedbackCollection.Find(f => f.Id == feedbackId).FirstOrDefaultAsync();
            if(feedback == null)
            {
                throw new NotFoundException("Cannot find feedback");
            }
            var feedbackMap = _mapper.Map<Feedback, FeedbackResponse>(feedback);
            return feedbackMap;
        }
    }
}
