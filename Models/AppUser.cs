using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniVisionBot.Models
{
    public class AppUser : MongoIdentityUser<ObjectId>
    {

        public string? FullName { get; set; }   
        public string? UrlImage { get; set; }
        public DateTime? Created {  get; set; }
    }
}
    