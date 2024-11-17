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
        }
      
        public async Task SaveMessage(MessageRequest request)
        {
            var conversation = _conversationCollection.Find(c => c.Id == request.ConversationId).FirstOrDefault();
            if(conversation == null)
            {
                throw new NotFoundException("Cannot find the conversation");
            }
            var messageMap = _mapper.Map<MessageRequest, Message>(request);
            await _messageCollection.InsertOneAsync(messageMap);
        }
    }
}
