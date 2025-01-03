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

        public async Task<ScoreExamOfEachUniversityByYearResponse> GetExamScoreByYear(ScoreExamOfEachUniversityByYearRequest request)
        {
            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$match", new BsonDocument("$or", new BsonArray()
        {
            new BsonDocument($"entry_score_exam.{request.Year}", new BsonDocument("$exists", true)),
            new BsonDocument($"entry_score_record.{request.Year}", new BsonDocument("$exists", true))
        })),
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
        new BsonDocument("$match", new BsonDocument(
            "university_info.university_code", request.UniversityCode
        )),
        new BsonDocument("$project", new BsonDocument
        {
            { "year", request.Year },
            { "university_code", "$university_info.university_code" },
            { "university_name", "$university_info.name" },
            { "majors", new BsonDocument("$map", new BsonDocument
                {
                    { "input", new BsonArray { "$$ROOT" } },
                    { "as", "major" },
                    { "in", new BsonDocument
                        {
                            { "name", "$name" },
                            { "major_code", "$major_code" },
                            { "subject_combinations", "$subject_combinations" },
                            { "entryScoreExam", new BsonDocument("$ifNull", new BsonArray
                                {
                                    new BsonDocument("$getField", new BsonDocument
                                    {
                                        { "field", request.Year },
                                        { "input", "$entry_score_exam" }
                                    }),
                                    ""
                                })
                            },
                            { "entryScoreRecord", new BsonDocument("$ifNull", new BsonArray
                                {
                                    new BsonDocument("$getField", new BsonDocument
                                    {
                                        { "field", request.Year},
                                        { "input", "$entry_score_record" }
                                    }),
                                    ""
                                })
                            },
                            { "notes", "$notes" }
                        }
                    }
                })
            }
        })
            };

            var result = await _majorCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            if (!result.Any())
                return null;

            var response = new ScoreExamOfEachUniversityByYearResponse
            {
                Title = $"Điểm chuẩn năm {request.Year} - {result.First()["university_code"].AsString} - {result.First()["university_name"].AsString}",
                UniversityCode = result.First()["university_code"].AsString,
                UniversityName = result.First()["university_name"].AsString,
                Year = request.Year,
                Majors = result.SelectMany(x => x["majors"].AsBsonArray.Select(m => new MajorInfo
                {
                    MajorName = m["name"].AsString,
                    MajorCode = m["major_code"].AsString,
                    SubjectCombinations = m["subject_combinations"].AsBsonArray.Select(s => s.AsString).ToList(),
                    EntryScoreExam = m["entryScoreExam"].IsBsonNull ? null : m["entryScoreExam"].AsString,
                    EntryScoreRecord = m["entryScoreRecord"].IsBsonNull ? null : m["entryScoreRecord"].AsString,
                    Notes = m["notes"].AsString
                })).ToList()
            };

            return response;
        }
        public async Task<List<UniversityExamScoreResponse>> GetTiileBySearching(UniversityExamScoreRequest request)
        {
            var pipeline = new List<BsonDocument>();

            // Only add initial match if year is provided
            if (!string.IsNullOrEmpty(request.Year))
            {
                pipeline.Add(new BsonDocument("$match", new BsonDocument("$or", new BsonArray()
        {
            new BsonDocument($"entry_score_exam.{request.Year}", new BsonDocument("$exists", true)),
            new BsonDocument($"entry_score_record.{request.Year}", new BsonDocument("$exists", true))
        })));
            }

            // Lookup stages remain the same
            pipeline.AddRange(new[]
            {
        new BsonDocument("$lookup", new BsonDocument
        {
            {"from", "Faculty"},
            {"localField", "faculty_id"},
            {"foreignField","_id"},
            {"as", "faculty_info"}
        }),
        new BsonDocument("$unwind", "$faculty_info"),

        new BsonDocument("$lookup", new BsonDocument
        {
            {"from", "University"},
            {"localField", "faculty_info.university_id"},
            {"foreignField", "_id"},
            {"as", "university_info"}
        }),
        new BsonDocument("$unwind", "$university_info")
    });

            // Add search/filter stage if searchTerm is provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchConditions = new BsonArray
        {
            new BsonDocument("university_info.name",
                new BsonRegularExpression(request.SearchTerm, "i")),
            new BsonDocument("university_info.university_code",
                new BsonRegularExpression(request.SearchTerm, "i"))
        };

                pipeline.Add(new BsonDocument("$match",
                    new BsonDocument("$or", searchConditions)));
            }

            // Get current year for default value
            var currentYear = DateTime.Now.Year;
            var yearValue = request.Year ?? currentYear.ToString() ;

            // Project stage with null checking
            pipeline.Add(new BsonDocument("$project", new BsonDocument
    {
        {"year", yearValue.ToString()},
        {"university_code", "$university_info.university_code"},
        {"university_name", "$university_info.name"},
        {"title", new BsonDocument("$concat", new BsonArray
        {
            "Điểm chuẩn năm ",
            yearValue.ToString(),
            " - ",
            "$university_info.university_code",
            " - ",
            "$university_info.name"
        })}
    }));

            var result = await _majorCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var response = result
                .Select(r => new UniversityExamScoreResponse
                {
                    Title = r.GetValue("title", string.Empty).AsString,
                    UniversityCode = r.GetValue("university_code", string.Empty).AsString,
                    UniversityName = r.GetValue("university_name", string.Empty).AsString,
                    Year = yearValue
                })
                .GroupBy(r => r.UniversityName)
                .Select(g => g.First())
                .ToList();

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
                            "Điểm chuẩn năm 2024 - ",
                            "$university_info.university_code"," - ",
                            "$university_info.name"
                        })
                    }
                })
           };

            var result = await _majorCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();


            var response = result.Select(r => new UniversityExamScoreResponse
            {
                Title = r["title"].AsString,
                Year = "2024",
                UniversityName = r["university_name"].AsString,
                UniversityCode = r["university_code"].AsString
            }).GroupBy(r => r.UniversityName).Select(g => g.First()).ToList();
            return response;
        }
    }
}

