using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.DTOs.Major;
using UniVisionBot.Exceptions;
using UniVisionBot.Models;
using UniVisionBot.Services.Major;

namespace UniVisionBot.Repositories.MajorRepository
{
    public class MajorRepository : IMajorRepository
    {
        private readonly IMongoCollection<Major> _majorCollection;
        private readonly IOptions<MyDatabase> _options; 
        private readonly IMapper _mapper;
        public MajorRepository(IOptions<MyDatabase> options, IMapper mapper ) 
        {
            _mapper = mapper;
            _options = options;
            var connectionString = new MongoClient(_options.Value.ConnectionString);
            var databaseName = connectionString.GetDatabase(_options.Value.DatabaseName);
            _majorCollection = databaseName.GetCollection<Major>(_options.Value.MajorCollectionName);
        }

        public async Task<MajorResponse> CreateNewMajor(MajorRequest request)
        {
            if(!ObjectId.TryParse(request.FacultyId, out ObjectId facultyId))
            {
                throw new BadInputException("Invalid input");
            }
            if(!ObjectId.TryParse(request.CareerId, out ObjectId careerId))
            {
                throw new BadInputException("Invalid input");
            }

            var major = _mapper.Map<MajorRequest, Major>(request);
            await _majorCollection.InsertOneAsync(major);

            var majorResponse =  _mapper.Map<Major, MajorResponse>(major);
            return majorResponse;
        }
    }
}
