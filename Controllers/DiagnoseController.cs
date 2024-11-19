using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.Models;

namespace UniVisionBot.Controllers
{
    [Route("api/diagnostics")]
    [ApiController]
    public class DiagnoseController : ControllerBase
    {
        private readonly IMongoDatabase _database;
        private readonly IOptions<MyDatabase> _options;

        public DiagnoseController(IOptions<MyDatabase> options)
        {
            _options = options;
            var client = new MongoClient(_options.Value.ConnectionString);
            _database = client.GetDatabase(_options.Value.DatabaseName);
        }

        [HttpGet("chat-status")]
        public IActionResult CheckChatStatus()
        {
            try
            {
                // Test MongoDB connection
                var collection = _database.GetCollection<Conversation>(_options.Value.ConversationCollectionName);
                collection.AsQueryable().FirstOrDefault();

                // Test SignalR endpoint availability
                var hubUrl = $"{Request.Scheme}://{Request.Host}/chatHub";

                return Ok(new
                {
                    mongoDbStatus = "Connected",
                    signalREndpoint = hubUrl,
                    serverTime = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    mongoDbConnectionString = _options.Value.ConnectionString.Substring(0, 20) + "..." // Only show beginning for security
                });
            }
        }
    }
}
