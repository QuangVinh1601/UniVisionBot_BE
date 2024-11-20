
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Text;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.Configurations.Options;
using UniVisionBot.Models;
using UniVisionBot.Repositories.Universities;
using UniVisionBot.Services.Universities;
using UniVisionBot.Services.Login;
using UniVisionBot.Repositories.Login;
using UniVisionBot.Middlewares;
using UniVisionBot.Services.Faculty;
using UniVisionBot.Repositories.Faculties;
using UniVisionBot.Services.Major;
using UniVisionBot.Repositories.MajorRepository;
using UniVisionBot.Services.IUniversityExamScore;
using UniVisionBot.Repositories.UniversityExamScore;
using UniVisionBot.Services.Chat;
using UniVisionBot.Repositories.Chat;
using UniVisionBot.Services.ChatHub;
using UniVisionBot.Repositories.ChatHub;
using UniVisionBot.Hubs;
using System.Text.Json.Serialization;
using UniVisionBot.Services.Image;
using UniVisionBot.Repositories.Image;
using UniVisionBot.Services.Article;
using UniVisionBot.Repositories.Articles;
using UniVisionBot.Configurations.CloudinaryConfig;

namespace UniVisionBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            BsonSerializer.RegisterSerializer(new ObjectIdSerializer(MongoDB.Bson.BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

            var configuration = builder.Configuration;
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("SignalRPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .SetIsOriginAllowed(origin => true) // Trong production nên specify domain cụ thể
                          .AllowCredentials();
                });
            });
            builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            builder.Services.Configure<MyDatabase>(configuration.GetSection("MyDatabase"));

            builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
            builder.Services.AddScoped<ILoginRepository, LoginRepository>();
            builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
            builder.Services.AddScoped<IMajorRepository, MajorRepository>();
            builder.Services.AddScoped<IUniversityExamScoreRepository, UniversityExamScoreRepository>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IChatHubRepository, ChatHubRepository>();
            builder.Services.AddScoped<IImageRepository, ImageRepository>();
            builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
            builder.Services.Configure<CloudinaryConfig>(configuration.GetSection("Cloudinary"));
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddAutoMapper(typeof(Program));

            var secretKey = configuration["AppSettings:SecretKey"];

            var mongoConnectionString = configuration["MyDatabase:ConnectionString"];
            var databaseName = configuration["MyDatabase:DatabaseName"];

            var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
            {
                MongoDbSettings = new MongoDbSettings
                {
                    ConnectionString = mongoConnectionString,
                    DatabaseName = databaseName
                },
                IdentityOptionsAction = options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireLowercase = false;

                    //lockout
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    options.Lockout.MaxFailedAccessAttempts = 5;

                    options.User.RequireUniqueEmail = true;
                }
            };

            builder.Services.ConfigureMongoDbIdentity<AppUser,AppRole,ObjectId>(mongoDbIdentityConfig)
            .AddUserManager<UserManager<AppUser>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddDefaultTokenProviders();    

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = "https://localhost:7230",
                    ValidAudience = "https://localhost:7230",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero

                };
            });
            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            }).AddJsonProtocol(options =>
            {
                // Giữ nguyên casing của property names
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                // Cấu hình DateTime serialization
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
          

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseCors("SignalRPolicy");

            app.UseExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
