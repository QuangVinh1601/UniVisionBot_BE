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
        public ChatHub(IChatHubRepository chatHubRepository, IOptions<MyDatabase> options)
        {
            _chatHubRepository = chatHubRepository;
            _options = options;
            var connnectionString = new MongoClient(_options.Value.ConnectionString);
            var databaseName = connnectionString.GetDatabase(_options.Value.DatabaseName);
            _conversationCollection = databaseName.GetCollection<Conversation>(_options.Value.ConversationCollectionName);
        }
        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }
        public async Task SendMessage(MessageRequest request)
        {
            await _chatHubRepository.SaveMessage(request);
            await Clients.Group(request.ConversationId).SendAsync("ReceiveMessage", request);
        }
    }
}
