using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.DTOs.Chat;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.ChatHub;

namespace UniVisionBot.Hubs
{
    public class ChatHub : Hub
    {
        //Create 2 function: 1. Add current connection in group, because I think user and consultant will be joined each group to exchange message eachother 
        // 2. Send message in user in that room
        private readonly IChatHubRepository _chatHubRepository;
        private readonly IMongoCollection<Conversation> _conversationCollection;
        private readonly IOptions<MyDatabase> _options;
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(IChatHubRepository chatHubRepository, IOptions<MyDatabase> options, ILogger<ChatHub> logger)
        {
            _chatHubRepository = chatHubRepository;
            _options = options;
            var connnectionString = new MongoClient(_options.Value.ConnectionString);
            var databaseName = connnectionString.GetDatabase(_options.Value.DatabaseName);
            _conversationCollection = databaseName.GetCollection<Conversation>(_options.Value.ConversationCollectionName);
            _logger = logger;
        }
        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }
        public async Task SendMessage(MessageRequest request)
        {
            try
            {
                _logger.LogInformation($"Received message request: {System.Text.Json.JsonSerializer.Serialize(request)}");

                // Validate request
                if (string.IsNullOrEmpty(request.ConversationId) ||
                    string.IsNullOrEmpty(request.SenderId) ||
                    string.IsNullOrEmpty(request.Content))
                {
                    _logger.LogError("Invalid message request - missing required fields");
                    throw new ArgumentException("Invalid message request");
                }
                // Ensure Created_At is set if not provided

                var response = await _chatHubRepository.SaveMessage(request);
                await Clients.Group(request.ConversationId).SendAsync("ReceiveMessage", response);

                _logger.LogInformation($"Message sent successfully to conversation {request.ConversationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendMessage: {ex}");
                throw;
            }
        }
        
    }
}
