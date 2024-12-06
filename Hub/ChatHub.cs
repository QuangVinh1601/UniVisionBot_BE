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
        private readonly IChatHubRepository _chatHubRepository;
        private readonly IMongoCollection<Conversation> _conversationCollection;
        private readonly IOptions<MyDatabase> _options;
        private readonly ILogger<ChatHub> _logger;
        public static Dictionary<string, string> ConsultantConnection = new Dictionary<string, string>();
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
                var conversation = await _chatHubRepository.GetConversation(request.ConversationId);
                await Clients.Group(request.ConversationId).SendAsync("ReceiveMessage", response);
                //if (request.SenderId != ConsultantConnection[GetConsultantConnection(request.ReceiverId)])
                //{
                //    await Clients.Client(GetConsultantConnection(request.ReceiverId)).SendAsync("NotifyNewConversation", conversation);
                //}
                _logger.LogInformation($"Message sent successfully to conversation {request.ConversationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendMessage: {ex}");
                throw;
            }
        }
        public async Task NotifyNewConversation(string conversationId)
        {
             var conversation = await _chatHubRepository.GetConversation(conversationId);
             foreach ( var consultant in ConsultantConnection)
             {
                 await Clients.Client(consultant.Value).SendAsync("NotifyNewConversation", conversation);
             }
        }
        public async Task SetConsultantConnection(string consultantId)
        {
            try
            {
                if (string.IsNullOrEmpty(consultantId))
                {
                    throw new ArgumentException("ConsultantId cannot be null or empty");
                }

                ConsultantConnection[consultantId] = Context.ConnectionId;
                _logger.LogInformation($"Consultant {consultantId} connected with ID {Context.ConnectionId}");

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting consultant connection for {consultantId}");
                throw;
            }
        }
        public static string GetConsultantConnection(string consultantId)
        {
            foreach (var connection in ConsultantConnection)
            {
                if(connection.Key == consultantId)
                {
                    return connection.Value;
                }
            }
            return null;
        }
        private void ValidateMessageRequest(MessageRequest request)
        {
            if (string.IsNullOrEmpty(request.ConversationId))
                throw new ArgumentException("ConversationId is required");

            if (string.IsNullOrEmpty(request.SenderId))
                throw new ArgumentException("SenderId is required");

            if (string.IsNullOrEmpty(request.Content))
                throw new ArgumentException("Content is required");
        }

    }
}
