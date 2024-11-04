﻿using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.DTOs.UniversityExamScore;
using UniVisionBot.Models;
using UniVisionBot.Services.IUniversityExamScore;
using UniVisionBot.Services.Universities;
namespace UniVisionBot.Repositories.UniversityExamScore
{
    public class UniversityExamScoreRepository : IUniversityExamScoreRepository
    {
        private readonly IMongoCollection<Major> _majorCollection;
        private readonly IOptions<MyDatabase> _options;
        public UniversityExamScoreRepository(IOptions<MyDatabase> options)
        {
            _options = options;
            var connectionString = new MongoClient(_options.Value.ConnectionString);
            var databaseName = connectionString.GetDatabase(_options.Value.DatabaseName);
            _majorCollection = databaseName.GetCollection<Major>(_options.Value.MajorCollectionName);
        }

        public async Task<List<UniversityExamScoreResponse>> GetTiileBySearching(UniversityExamScoreRequest request)
        {
           
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("$or", new BsonArray()
                {
                    new BsonDocument($"entry_score_exam.{request.Year}", new BsonDocument("$exists", true) ),
                    new BsonDocument($"entry_score_record.{request.Year}", new BsonDocument("$exists", true))
                })),
                new BsonDocument("$lookup", new BsonDocument
                {
                    {"from", "Faculty" },
                    {"localField", "faculty_id"},
                    {"foreignField","_id" },
                    {"as", "faculty_info" }
                }),
                new BsonDocument("$unwind", "$faculty_info"),
                new BsonDocument("$lookup", new BsonDocument
                {
                    {"from", "University" },
                    {"localField", "faculty_info.university_id" },
                    {"foreignField", "_id" },
                    {"as", "university_info" }
                }),
                new BsonDocument("$unwind", "$university_info"),
                new BsonDocument("$match", new BsonDocument("$or", new BsonArray(){
                    new BsonDocument("university_info.university_code", new BsonDocument("$regex", new BsonRegularExpression(request.SearchTerm,"i"))),
                    new BsonDocument("university_info.name", new BsonDocument("$regex", new BsonRegularExpression(request.SearchTerm,"i")))
                })),
                new BsonDocument("$project", new BsonDocument()
                {
                    {"year", request.Year },
                    {"university_code", "$university_info.university_code" },
                    {"university_name", "$university_info.name"},
                    {"title", new BsonDocument("$concat", new BsonArray(){
                        "Điểm chuẩn năm " ,request.Year.ToString(), " - ", "$university_info.university_code", " - ", "$university_info.name"
                    })}
                })
            };

            var result = await _majorCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            var response = result.Select(r => new UniversityExamScoreResponse
            {
                Title = r.Contains("title") ? r["title"].AsString : string.Empty,
                UniversityCode = r.Contains("university_code") ? r["university_code"].AsString : string.Empty,
                UniversityName = r.Contains("university_name") ? r["university_name"].AsString : string.Empty,
                Year = r.Contains("year") ? r["year"].AsString : string.Empty
            }).GroupBy(r => r.UniversityName).Select(g => g.First()).ToList();
            return response;
        }
        public async Task<List<UniversityExamScoreResponse>> GetTitle()
        {
            var pipeline = new BsonDocument[]
           {
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Faculty" },
                    { "localField", "faculty_id" },
                    { "foreignField", "_id" },
                    { "as", "faculty_info" }
                }),
                new BsonDocument("$unwind", "$faculty_info"),

                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "University" },
                    { "localField", "faculty_info.university_id" },
                    { "foreignField", "_id" },
                    { "as", "university_info" }
                }),
                new BsonDocument("$unwind", "$university_info"), 

                new BsonDocument("$project", new BsonDocument
                {
                    { "university_name", "$university_info.name" },
                    { "university_code", "$university_info.university_code" },
                    { "title", new BsonDocument("$concat", new BsonArray
                        {
                            "Điểm chuẩn năm 2023 - ",
                            "$university_info.university_code"," - ",
                            "$university_info.name"
                        })
                    }
                })
           };

                    var result = await _majorCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

                   
                    var response = result.Select(r => new UniversityExamScoreResponse
                    {
                        Title =  r["title"].AsString ,
                        Year = "2023",
                        UniversityName = r["university_name"].AsString,
                        UniversityCode = r["university_code"].AsString
                    }).GroupBy(r => r.UniversityName).Select(g => g.First()).ToList();
                    return response;
        }
    }
}
    
