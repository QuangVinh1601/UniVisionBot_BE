using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.DTOs.Chat;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.ChatHub;

namespace UniVisionBot.Repositories.ChatHub
{
    public class ChatHubRepository : IChatHubRepository
    {
        private readonly IMongoCollection<Conversation> _conversationCollection;
        private readonly IMongoCollection<Message> _messageCollection;
        private readonly IOptions<MyDatabase> _options;
        private readonly IMapper _mapper;
        public ChatHubRepository(IOptions<MyDatabase> options, IMapper mapper) 
        {
            _options = options;
            var connnectionString = new MongoClient(_options.Value.ConnectionString);
            var databaseName = connnectionString.GetDatabase(_options.Value.DatabaseName);
            _conversationCollection = databaseName.GetCollection<Conversation>(_options.Value.ConversationCollectionName);
            _messageCollection = databaseName.GetCollection<Message>(_options.Value.MessageCollectionName);
            _mapper = mapper;
        }

        public async Task<MessageResponse> SaveMessage(MessageRequest request)
        {
            var conversation = _conversationCollection.Find(c => c.Id == request.ConversationId).FirstOrDefault();
            if(conversation == null)
            {
                throw new NotFoundException("Cannot find the conversation");
            }
            var messageMap = _mapper.Map<MessageRequest, MessageResponse>(request);
            messageMap.Id = ObjectId.GenerateNewId().ToString();
            messageMap.Created_At = DateTime.UtcNow;
            var messageResponseMap = _mapper.Map<MessageResponse, Message>(messageMap);
            await _messageCollection.InsertOneAsync(messageResponseMap);
            return messageMap;
        }
    }
}
