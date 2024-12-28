using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.DTOs.User;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.User;

namespace UniVisionBot.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<AppUser> _userCollection;
        private readonly IOptions<MyDatabase> _options;
        private readonly IMongoCollection<AppRole> _rolesCollection;
        public UserRepository(IOptions<MyDatabase> options)
        {
            _options = options; 
            var connectionString = new MongoClient(_options.Value.ConnectionString);
            var database = connectionString.GetDatabase(_options.Value.DatabaseName);
            _userCollection = database.GetCollection<AppUser>(_options.Value.AppUsersCollectionName);
            _rolesCollection = database.GetCollection<AppRole>(_options.Value.AppRolesCollectionName);
        }
        public async Task DeleteUser(string userId)
        {
            ObjectId.TryParse(userId, out ObjectId objectUserId);
            var filter = Builders<AppUser>.Filter.Eq(u => u.Id, objectUserId);
            await _userCollection.DeleteOneAsync(filter);
        }

        public async Task<List<UserResponse>> GetAllUser()
        {
            var response = await Task.WhenAll((await _userCollection.Find(_ => true).ToListAsync()).Select(async u => new UserResponse
            {
                Id = u.Id.ToString(),
                Email = u.Email,
                UserName = u.UserName,
                FullName = u.FullName,
                RoleUser = (await _rolesCollection.Find(r => r.Id == u.Roles.FirstOrDefault()).FirstOrDefaultAsync()).Name ?? "USER"
            }));
            return response.ToList();
        }

        public async Task<UserResponse> GetUserById(string id)
        {
            var user =  await _userCollection.Find(u => u.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
            return new UserResponse
            {
                Email = user.Email,
                UserName = user.UserName,
                Id = user.Id.ToString(),
                FullName = user.FullName,
                RoleUser = (await _rolesCollection.Find(r => r.Id == user.Roles.FirstOrDefault()).FirstOrDefaultAsync()).Name ?? "USER"
            };
        }

        public async Task UpdateUser(string userId, UserRequest request)
        {
            var users = (await _userCollection.Find(u => u.Id != ObjectId.Parse(userId)).ToListAsync());
            foreach (var user in users)
            {   
                if (user.UserName == request.UserName)
                {
                    throw new BadInputException("Username already exists");
                }
                else if(user.Email == request.Email)
                {
                    throw new BadInputException("Email already exists");
                }
            }
             await _userCollection.UpdateOneAsync(Builders<AppUser>.Filter.Eq(u => u.Id, ObjectId.Parse(userId)),
                                                                  Builders<AppUser>.Update.Set(u => u.Email, request.Email)
                                                                  .Set(u => u.UserName, request.UserName)
                                                                  .Set(u => u.FullName, request.FullName));
        }
    }
}
