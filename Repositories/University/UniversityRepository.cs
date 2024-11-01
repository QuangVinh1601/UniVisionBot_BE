using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.DTOs.University;
using UniVisionBot.Models;
using UniVisionBot.Services.Universities;
using UniVisionBot.Exceptions;
using System.Linq.Expressions;
using UniVisionBot.Helpers.Pagination;

namespace UniVisionBot.Repositories.Universities
{
    public class UniversityRepository : IUniversityRepository
    {
        private readonly IMongoCollection<University> _universityCollection;
        private readonly IOptions<MyDatabase> _options;
        private readonly IMapper _mapper;
        public UniversityRepository(IOptions<MyDatabase> options, IMapper mapper)
        {
            _options = options;
            var mongoClient = new MongoClient(_options.Value.ConnectionString);
            var database =  mongoClient.GetDatabase(_options.Value.DatabaseName);
            _universityCollection = database.GetCollection<University>(_options.Value.UniversityCollectionName);
            _mapper = mapper;
        }

        public async Task CreateAsync(University request)
        {
           await  _universityCollection.InsertOneAsync(request);
        }

        public async Task DeleteAsync(string id)
        {
            if(!ObjectId.TryParse(id, out ObjectId objectId))
            {
                throw new BadInputException("Invalid ObjectId format");
            }

            var deleteResult = await _universityCollection.DeleteOneAsync(u => u.Id == id);
            if(deleteResult.DeletedCount == 0)
            {
                throw new NotFoundException("University", id);
            }
        }

        public List<UniversityResponse> GetAllUniversity(int? page)
        {
            var universityList = _universityCollection.Find(_ => true).ToList();
            var universityResponse = _mapper.Map<List<University>, List<UniversityResponse>>(universityList);
            var universityPaginated =  PaginatedList<UniversityResponse>.Pagination(universityResponse, page??1);
            return universityPaginated;
        }

        public UniversityResponse GetById(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                throw new BadInputException("Invalid format");
            }
            var university = _universityCollection.AsQueryable().Where(u => u.Id == id).FirstOrDefault();
            if (university == null)
            {
                throw new NotFoundException(nameof(university), id);
            }

            var universityRespone = _mapper.Map<UniversityResponse>(university);
            return universityRespone;
        }

        public async Task<UniversityResponse> UpdateAsync(UniversityRequest request, string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                throw new BadInputException("Invalid format");
            }
            var university =  _universityCollection.AsQueryable().Where(u => u.Id == id).FirstOrDefault();
            if(university == null)
            {
                throw new NotFoundException(nameof(university), id);
            }

            _mapper.Map(request, university);
            await _universityCollection.ReplaceOneAsync(
            Builders<University>.Filter.Eq(u => u.Id, id),
            university
            );

            return _mapper.Map<UniversityResponse>(university);

        }
    }
}
