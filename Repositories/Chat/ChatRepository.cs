using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.DTOs.Chat;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.Chat;
using UniVisionBot.Services.ChatHub;

namespace UniVisionBot.Repositories.Chat
{
    public class ChatRepository: IChatRepository
    {
        private readonly IMongoCollection<Message> _messageCollection;
        private readonly IMongoCollection<Conversation> _conversationCollection;
        private readonly IMongoCollection<PendingConversation> _pendingConversationCollection;
        private readonly IMongoCollection<AppUser> _appUserCollection;
        private readonly IOptions<MyDatabase> _options;
        private readonly IMapper _mapper;
        public ChatRepository(IOptions<MyDatabase> options, IMapper mapper)
        {
            _options = options;
            var connnectionString = new MongoClient(_options.Value.ConnectionString);
            var databaseName = connnectionString.GetDatabase(_options.Value.DatabaseName);
            _messageCollection = databaseName.GetCollection<Message>(_options.Value.MessageCollectionName);
            _conversationCollection = databaseName.GetCollection<Conversation>(_options.Value.ConversationCollectionName);
            _appUserCollection = databaseName.GetCollection<AppUser>(_options.Value.AppUsersCollectionName);
            _pendingConversationCollection = databaseName.GetCollection<PendingConversation>(_options.Value.PendingConversationCollectionName);
            _mapper = mapper;
        }
        public async Task<List<ConversationResponse>> GetAllConversationForConsultant(string consultantId)
        {
            if (!ObjectId.TryParse(consultantId, out ObjectId objectConsultantId))
            {
                throw new BadInputException("Invalid format");
            }
            //var filter = Builders<Conversation>.Filter.Eq(c => c.ConsultantId, consultantId);
            //var conversationlist = await _conversationCollection.Find(filter).ToListAsync();
            var pendingConversationList = await _pendingConversationCollection.Find(_ => true).ToListAsync();
            var conversationList = await _conversationCollection.Find(_ => true).ToListAsync();
            var conversationListExcludePending = conversationList.Where(c => !pendingConversationList.Any(pc => pc.conversationId == c.Id)).ToList();
            var conversationListResponse = new List<ConversationResponse>();
            foreach (var conversation in conversationListExcludePending)
            {
                ObjectId.TryParse(conversation.UserId, out ObjectId objectUserId);
                var filterUser = Builders<AppUser>.Filter.Eq("_id", objectUserId);
                var user = await _appUserCollection.Find(filterUser).FirstOrDefaultAsync();

                var messageList = await _messageCollection.Find(m => m.ConversationId == conversation.Id).ToListAsync();
                var latestMessage = messageList.OrderByDescending(m => m.Created_At).FirstOrDefault();
                var conversationmap = _mapper.Map<Conversation, ConversationResponse>(conversation);
                var messageMap = _mapper.Map<List<Message>, List<MessageResponse>>(messageList);
                var userMap = _mapper.Map<AppUser, UserResponse>(user);
                conversationmap.LastMessage = latestMessage?.Content;
                conversationmap.LastMessageTime = latestMessage?.Created_At;
                conversationmap.User = userMap;
                conversationmap.Messages = messageMap;
                conversationListResponse.Add(conversationmap);
                }
            return conversationListResponse;
        }
        //User side
        public async Task<ConversationResponse> GetConversationForCurrentUser(string currentUserId, string consultantId)
        {
            if (!ObjectId.TryParse(currentUserId, out ObjectId objectCurrentUserId))
            {
                throw new BadInputException("Invalid format");
            }
            var conversation = _conversationCollection.Find(c => c.UserId == currentUserId && c.ConsultantId == consultantId).FirstOrDefault();
            if (conversation == null)
            {
                throw new NotFoundException("Conversation is not existed");
            }
            var messageList = await _messageCollection.Find(m => m.ConversationId == conversation.Id).ToListAsync();
            if (messageList == null)
            {
                throw new NotFoundException("Message list is not found");
            }
            var messageListMap = _mapper.Map<List<Message>, List<MessageResponse>>(messageList);
            var conversationmap = _mapper.Map<Conversation, ConversationResponse>(conversation);
            conversationmap.Messages = messageListMap;
            return conversationmap;
        }

        // Consultant side
        public async Task<ConversationResponse> GetHistoryMessage(string conversationId)
        {
            if (!ObjectId.TryParse(conversationId, out ObjectId objectConversationId))
            {
                throw new BadInputException("Invalid format");
            }
            var conversation = _conversationCollection.Find(c => c.Id == conversationId).FirstOrDefault();
            if (conversation == null)
            {
                throw new NotFoundException("Cannot find conversation");
            }
            var user = _appUserCollection.Find(u => u.Id == ObjectId.Parse(conversation.UserId)).FirstOrDefault();
            if (user == null)
            {
                throw new NotFoundException("Cannot find user");
            }
            var messageList = await _messageCollection.Find(m => m.ConversationId == conversationId).ToListAsync();
            if (messageList == null)
            {
                throw new NotFoundException("Cannot find any message for current conversation");
            }
            var userMap = _mapper.Map<AppUser, UserResponse>(user);

            var messageMap = _mapper.Map<List<Message>, List<MessageResponse>>(messageList);
            var conversationMap = _mapper.Map<Conversation, ConversationResponse>(conversation);
            conversationMap.Messages = messageMap;
            conversationMap.User = userMap;
            conversationMap.LastMessage = messageMap.OrderByDescending(m => m.Created_At).FirstOrDefault()?.Content;
            return conversationMap;
        }
        public async Task<string> CreateConversation(ConversationRequest request)
        {
            if (!ObjectId.TryParse(request.ConsultantId, out var consultantId))
            {
                throw new BadInputException("Invalid format");
            }
            if (!ObjectId.TryParse(request.UserId, out ObjectId userId))
            {
                throw new BadInputException("Invalid format");
            }
            var conversation = _conversationCollection.Find(c => c.UserId == request.UserId && c.ConsultantId == request.ConsultantId).FirstOrDefault();
            if (conversation == null)
            {
                var newConversation = new Conversation
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserId = request.UserId,
                    ConsultantId = request.ConsultantId,
                    Created_At = DateTime.UtcNow,
                };
                await _conversationCollection.InsertOneAsync(newConversation);
                return newConversation.Id;
            }
            return conversation.Id;
        }

        public List<PendingConversationResponse> GetAllPendingConversation()
        {
            var pendingConversationList = _pendingConversationCollection.Find(_ => true).ToList().Select(pc => new PendingConversationResponse
            {
                Id = pc.Id,
                ConversationId = pc.conversationId,
                Status = pc.Status,
                Fullname = pc.Fullname,
                CreatedAt = pc.CreatedAt
            }).ToList();
            return pendingConversationList;
        }

        public async Task DeleteConversation(string conversationId)
        {
            await _conversationCollection.DeleteOneAsync(c => c.Id == conversationId);
            await _pendingConversationCollection.DeleteOneAsync(c => c.conversationId == conversationId);
        }

        public PendingConversationResponse GetPendingConversation(PendingConversationRequest request)
        {
            var pendingConversation = _pendingConversationCollection.Find(pc => pc.conversationId == request.ConversationId).FirstOrDefault();
            var pendingConvesationResponse = new PendingConversationResponse
            {
                Id = pendingConversation.Id,
                Fullname = pendingConversation.Fullname,
                ConversationId = pendingConversation.conversationId,
                Status = pendingConversation.Status,
                CreatedAt = pendingConversation.CreatedAt
            };
            return pendingConvesationResponse;

        }
    }
}
