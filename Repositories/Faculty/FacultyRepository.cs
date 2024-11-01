using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UniVisionBot.Configurations.DbConfig;
using UniVisionBot.Configurations.Options;
using UniVisionBot.DTOs.Faculty;
using UniVisionBot.Exceptions;
using UniVisionBot.Helpers.Pagination;
using UniVisionBot.Models;
using UniVisionBot.Services.Faculty;

namespace UniVisionBot.Repositories.Faculties
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly IMapper _mapper;
        private readonly IOptions<MyDatabase> _options;
        private readonly IMongoCollection<Faculty> _facultyCollection;
        private readonly IMongoCollection<University> _universityCollection;
        public FacultyRepository(IMapper mapper, IOptions<MyDatabase> options) 
        {
            _mapper = mapper;
            _options = options;
            var connectionString = new MongoClient(_options.Value.ConnectionString);
            var databaseName = connectionString.GetDatabase(options.Value.DatabaseName);
            _facultyCollection = databaseName.GetCollection<Faculty>(_options.Value.FacultyCollectionName);
            _universityCollection = databaseName.GetCollection<University>(_options.Value.UniversityCollectionName);
        }

        public async Task<FacultyResponse> CreateAsync(FacultyRequest request, string universityId)
        {
            if(!ObjectId.TryParse(universityId, out ObjectId id))
            {
                throw new BadInputException("Invalid university Id");
            }
            var universityExisted = _universityCollection.AsQueryable().Where(u => u.Id == universityId).FirstOrDefault();
            if (universityExisted == null)
            {
                throw new NotFoundException("Failed to find university");
            }
            var faculty =  _mapper.Map<FacultyRequest, Faculty>(request);
            faculty.UniversityId = universityExisted.Id;
            await _facultyCollection.InsertOneAsync(faculty);
            var facultyResponse =  _mapper.Map<Faculty, FacultyResponse>(faculty);
            facultyResponse.Success = true;
            facultyResponse.Message = "Successfully created";
            return facultyResponse;
        }

        public async Task DeleteAsync(string universityId, string facultyId)
        {
            if (!ObjectId.TryParse(universityId, out ObjectId universityObjectId))
            {
                throw new BadInputException("Invalid format");
            }
            if(!ObjectId.TryParse(facultyId, out  ObjectId facultyObjectId))
            {
                throw new BadInputException("Invalid format");
            }
            var universityExisted = await _universityCollection.Find(u => u.Id == universityId).AnyAsync();
            if (!universityExisted)
            {
                throw new NotFoundException("university", universityId);
            }
            var deleteResult =  await _facultyCollection.DeleteOneAsync(f => f.Id == facultyId);
            if(deleteResult.DeletedCount == 0)
            {
                throw new NotFoundException("Faculty", facultyId);
            }
        }

        public async Task<FacultyResponse> UpdateAsync(FacultyRequest request, string universityId, string facultyId)
        {
            if (!ObjectId.TryParse(facultyId, out ObjectId objectId))
            {
                throw new BadInputException("Invalid input");
            }
            if(!ObjectId.TryParse(universityId, out ObjectId universityObjectId))
            {
                throw new BadInputException("Invalid input");
            }

            var universityExisted = _universityCollection.AsQueryable().Where(u => u.Id == universityId).FirstOrDefault();
            if (universityExisted == null)
            {
                throw new NotFoundException(nameof(universityExisted), universityId);
            }
            var facultyExisted = _facultyCollection.AsQueryable().Where(f => f.Id == facultyId).FirstOrDefault();
            if (facultyExisted == null)
            {
                throw new NotFoundException(nameof(facultyExisted), facultyId);
            }
            var faculty = _mapper.Map<FacultyRequest, Faculty>(request);
            faculty.UniversityId = universityExisted.Id;
            faculty.Id = facultyExisted.Id;

            await _facultyCollection.ReplaceOneAsync(
            Builders<Faculty>.Filter.Eq(f => f.Id, facultyExisted.Id),
            faculty
            );
            var facultyResponse =  _mapper.Map<Faculty, FacultyResponse>(faculty);
            facultyResponse.Success = true;
            facultyResponse.Message = "Successfully to update";
            return facultyResponse;
        }
        public async Task<List<FacultyResponse>> GetAllFacultyOfUniversity(string universityId, int? page)
        {
            if(!ObjectId.TryParse(universityId, out ObjectId objectId))
            {
                throw new BadInputException("Invalid format");
            }
            var universityExisted =  _universityCollection.AsQueryable().Where(u => u.Id == universityId).FirstOrDefault();
            if(universityExisted == null)
            {
                throw new NotFoundException(nameof(universityExisted), universityId);
            }
            var filter = Builders<Faculty>.Filter.Eq(f => f.UniversityId, universityId);
            var facultiesList =  await _facultyCollection.Find(filter).ToListAsync();
            var facultiesResponse =  _mapper.Map<List<Faculty>, List<FacultyResponse>>(facultiesList);

            var facultiesPaginated =  PaginatedList<FacultyResponse>.Pagination(facultiesResponse, page ?? 1);
            return facultiesPaginated;
        }
    }
}
