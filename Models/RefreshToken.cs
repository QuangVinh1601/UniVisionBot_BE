using MongoDB.Bson;

namespace UniVisionBot.Models
{
    public class RefreshToken
    {
        public ObjectId Id { get; set; }
        public DateTime Expired { get; set; }
        public ObjectId UserId { get; set; }
        public Guid Token { get; set; }
        public bool isRevoked { get; set; }


    }
}
