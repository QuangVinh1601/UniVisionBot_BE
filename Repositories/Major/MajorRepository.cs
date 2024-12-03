using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.DTOs.Major;
using UniVisionBot.Exceptions;
using UniVisionBot.Helpers.Pagination;
using UniVisionBot.Models;
using UniVisionBot.Services.Major;

namespace UniVisionBot.Repositories.MajorRepository
{
    public class MajorRepository : IMajorRepository
    {
        private readonly IMongoCollection<Major> _majorCollection;
        private readonly IMongoCollection<Faculty> _facultyCollection;
        private readonly IOptions<MyDatabase> _options;
        private readonly IMapper _mapper;
        public MajorRepository(IOptions<MyDatabase> options, IMapper mapper)
        {
            _mapper = mapper;
            _options = options;
            var connectionString = new MongoClient(_options.Value.ConnectionString);
            var databaseName = connectionString.GetDatabase(_options.Value.DatabaseName);
            _majorCollection = databaseName.GetCollection<Major>(_options.Value.MajorCollectionName);
            _facultyCollection = databaseName.GetCollection<Faculty>(_options.Value.FacultyCollectionName);
        }

        public async Task<MajorResponse> CreateNewMajorAsync(MajorRequest request)
        {
            if (!ObjectId.TryParse(request.FacultyId, out ObjectId facultyId))
            {
                throw new BadInputException("Invalid input");
            }
            
            var major = _mapper.Map<MajorRequest, Major>(request);
            await _majorCollection.InsertOneAsync(major);

            var majorResponse = _mapper.Map<Major, MajorResponse>(major);
            return majorResponse;
        }

        public async Task<MajorResponse> UpdateAsync(MajorRequest request, string majorId)
        {
            if (!ObjectId.TryParse(majorId, out ObjectId majorObjectId))
            {
                throw new BadInputException("Invalid format");
            }
           
            var majorExisted = _majorCollection.AsQueryable().Where(m => m.Id == majorId).FirstOrDefault();
            if (majorExisted == null)
            {
                throw new NotFoundException("faculty", majorId);
            }
            var majorMap = _mapper.Map<MajorRequest, Major>(request);
            majorMap.Id = majorId;
            majorMap.FacultyId = majorExisted.FacultyId;
            var resultReplace = await _majorCollection.ReplaceOneAsync(Builders<Major>.Filter.Eq(m => m.Id, majorExisted.Id), majorMap);
            if (!resultReplace.IsAcknowledged)
            {
                throw new Exception("Can not replace existed instance");
            }
            var majorResponse = _mapper.Map<Major, MajorResponse>(majorMap);
            return majorResponse;
        }
        public async Task DeleteAsync(string majorId, string facultyId)
        {
            if (!ObjectId.TryParse(majorId, out ObjectId majorObjectId))
            {
                throw new BadInputException("Invalid format");
            }
            if (!ObjectId.TryParse(facultyId, out ObjectId facultyObjectId))
            {
                throw new BadInputException("Invalid format");
            }
            var filter = Builders<Major>.Filter.Eq(m => m.Id, majorId);

            var deleteResult = await _majorCollection.DeleteOneAsync(filter);
            if (!deleteResult.IsAcknowledged)
            {
                throw new Exception("Failed delete");
            }
        }

        public async Task<List<MajorResponse>> GetMajorsbyFacultyIdAsync(string facultyId, int? page)
        {
            if (!ObjectId.TryParse(facultyId, out ObjectId facultyObjectId))
            {
                throw new BadInputException("Invalid format");
            }

            var facultyExisted = _facultyCollection.AsQueryable().Where(f => f.Id == facultyId).FirstOrDefault();
            if (facultyExisted == null)
            {
                throw new NotFoundException("Faculty isn't exist");
            }
            var filter =  Builders<Major>.Filter.Eq(m => m.FacultyId, facultyId);
            var majorList = (await _majorCollection.FindAsync(filter)).ToList();
            //var majorList = _majorCollection.AsQueryable().Where(m => m.FacultyId == facultyId).ToList();

            var majorPaginatedList = PaginatedList<Major>.Pagination(majorList, page ?? 1);
            var majorPaginatedListResponse = _mapper.Map<List<Major>, List<MajorResponse>>(majorPaginatedList);
            return majorPaginatedListResponse;

        }

        public  MajorResponse GetMajorById(string majorId)
        {
            var filter = Builders<Major>.Filter.Eq(m => m.Id, majorId);
            var major =  _majorCollection.Find(filter).FirstOrDefault();
            var majorResponse = _mapper.Map<Major, MajorResponse>(major);
            return majorResponse;
        }
    }
}
