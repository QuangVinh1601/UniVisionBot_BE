using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.Models;

namespace UniVisionBot.Helpers.SeedData
{
    public class SeedData
    {
        private readonly IMongoCollection<AppUser> _userCollection;
        private readonly IMongoCollection<AppRole> _roleCollection;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager; 
        private readonly IOptions<MyDatabase> _options;
        public SeedData(IOptions<MyDatabase> options, RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _options = options;
            _roleManager = roleManager;
            _userManager = userManager;
            var connectionString = new MongoClient(_options.Value.ConnectionString);
            var database = connectionString.GetDatabase(_options.Value.DatabaseName);
            _userCollection = database.GetCollection<AppUser>(_options.Value.AppUsersCollectionName);
            _roleCollection = database.GetCollection<AppRole>(_options.Value.AppRolesCollectionName);

        }
        public async Task InitialData()
        {
            if((await _roleCollection.CountDocumentsAsync(FilterDefinition<AppRole>.Empty)) == 0)
            {
                var userRole = new AppRole()
                {
                    Name = "USER"
                };
                await _roleManager.CreateAsync(userRole);
                var adminRole = new AppRole()
                {
                    Name = "ADMIN"
                };
                await _roleManager.CreateAsync(adminRole);
                var consultantRole = new AppRole()
                {
                    Name = "CONSULTANT"
                };
                await _roleManager.CreateAsync(consultantRole);
            }
            if((await _userCollection.CountDocumentsAsync(FilterDefinition<AppUser>.Empty)) == 0)
            {
                var consultant = new AppUser()
                {
                    Email = "consultant@gmail.com",
                    UserName = "consultant@gmail.com",
                    FullName = "Hải Nhi",
                    EmailConfirmed = true,
                };
                await _userManager.CreateAsync(consultant, "User123#");
                await _userManager.AddToRoleAsync(consultant, "CONSULTANT");
                var admin = new AppUser()
                {
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    FullName = "Quang Trung",
                    EmailConfirmed = true,
                };
                await _userManager.CreateAsync(admin, "User123#");
                await _userManager.AddToRoleAsync(admin, "ADMIN");
                var user = new AppUser()
                {
                    Email = "quangvinh5@gmail.com",
                    UserName = "quangvinh5@gmail.com",
                    FullName = "Quang Vinh",
                    EmailConfirmed = true,
                };
                await _userManager.CreateAsync(user, "User123#");
                await _userManager.AddToRoleAsync(user, "USER");

                var fakerUser = new Faker<AppUser>()
               .RuleFor(u => u.UserName, f => f.Internet.Email())
               .RuleFor(u => u.Email, f => f.Internet.Email())
               .RuleFor(u => u.EmailConfirmed, f => true)
               .RuleFor(u => u.CreatedOn, f => new DateTime(2024, f.PickRandom(new List<int> { 9, 10, 11, 12 }), f.Random.Int(1, 30)));
                
                var userFakers = fakerUser.Generate(200);
                foreach (var userFaker in userFakers)
                {
                    await _userManager.CreateAsync(userFaker, "User123#");
                    await _userManager.AddToRoleAsync(userFaker, "USER");
                }
            }
        }
    }
}
