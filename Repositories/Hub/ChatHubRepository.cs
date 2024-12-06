using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
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
        private readonly IMongoCollection<AppUser> _appUsersCollection;
        private readonly IOptions<MyDatabase> _options;
        private readonly IMapper _mapper;
        public ChatHubRepository(IOptions<MyDatabase> options, IMapper mapper) 
        {
            _options = options;
            var connnectionString = new MongoClient(_options.Value.ConnectionString);
            var databaseName = connnectionString.GetDatabase(_options.Value.DatabaseName);
            _conversationCollection = databaseName.GetCollection<Conversation>(_options.Value.ConversationCollectionName);
            _messageCollection = databaseName.GetCollection<Message>(_options.Value.MessageCollectionName);
            _appUsersCollection = databaseName.GetCollection<AppUser>(_options.Value.AppUsersCollectionName);
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
        public async Task<ConversationResponse> GetConversation(string conversationId)
        {
            var conversation = await _conversationCollection.Find(c => c.Id == conversationId).FirstOrDefaultAsync();

            if(!ObjectId.TryParse(conversation.UserId, out ObjectId objectuserId))
            {
                throw new Exception("Invalid value");
            }
            var user = await _appUsersCollection.Find(u => u.Id == objectuserId).FirstOrDefaultAsync();
            var userMap = _mapper.Map<AppUser, UserResponse>(user);
            var messageList = await _messageCollection.Find(m => m.ConversationId == conversationId).ToListAsync();
            var lastMessage = messageList.OrderByDescending(m => m.Created_At).FirstOrDefault()?.Content;
            var messageMap = _mapper.Map<List<Message>, List<MessageResponse>>(messageList);
            var conversationResponse = _mapper.Map<Conversation, ConversationResponse>(conversation);
            conversationResponse.Messages = messageMap;
            conversationResponse.LastMessage = lastMessage;
            conversationResponse.User = userMap;
            return conversationResponse;
        }
    }
}
