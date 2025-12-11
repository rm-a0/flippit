using System.Linq;
using Flippit.Common.Models.CompletedLesson;
using Flippit.Web.BL.Mappers;
using Flippit.Web.DAL.Repositories;

namespace Flippit.Web.BL.Facades
{
    public class CompletedLessonFacade : ICompletedLessonFacade
    {
        private readonly CompletedLessonRepository _repository;
        private readonly CompletedLessonMapper _mapper;
        private readonly LocalDbOptions _localDbOptions;
        private readonly IApiApiClient _apiClient;
        private readonly IUsersApiClient _usersApiClient;
        private readonly ICompletedLessonsApiClient _completedLessonsApiClient;
        private readonly ApiModelMapper _apiMapper;

        public CompletedLessonFacade(
            CompletedLessonRepository repository, 
            CompletedLessonMapper mapper,
            LocalDbOptions localDbOptions,
            IApiApiClient apiClient,
            IUsersApiClient usersApiClient,
            ICompletedLessonsApiClient completedLessonsApiClient,
            ApiModelMapper apiMapper)
        {
            _repository = repository;
            _mapper = mapper;
            _localDbOptions = localDbOptions;
            _apiClient = apiClient;
            _completedLessonsApiClient = completedLessonsApiClient;
            _apiMapper = apiMapper;
            _usersApiClient = usersApiClient;
        }

        public async Task<IList<Flippit.Common.Models.CompletedLesson.CompletedLessonListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                var entities = await _repository.GetAllAsync();
                var paged = entities.Skip((page - 1) * pageSize).Take(pageSize);
                return _mapper.DetailToListModels(paged);
            }
            else
            {
                var apiLessons = await _apiClient.CompletedLessonsGetAsync(pageSize, page, filter, sortBy, CancellationToken.None);
                return _apiMapper.ToCommonCompletedLessonLists(apiLessons);
            }
        }

        public async Task<IList<Flippit.Common.Models.CompletedLesson.CompletedLessonListModel>> SearchByCollectionIdAsync(Guid collectionId)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                var entities = await _repository.GetAllAsync();
                var filtered = entities.Where(x => x.CollectionId == collectionId);
                return _mapper.DetailToListModels(filtered);
            }
            else
            {
                // Note: API doesn't have a specific endpoint for searching by collection ID
                // Fetch all and filter client-side (not ideal but matches API capabilities)
                var apiLessons = await _apiClient.CompletedLessonsGetAsync(1000, 1, null, null, CancellationToken.None);
                var filtered = apiLessons.Where(x => x.CollectionId == collectionId);
                return _apiMapper.ToCommonCompletedLessonLists(filtered);
            }
        }

        public async Task<Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel?> GetByIdAsync(Guid id)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                return await _repository.GetByIdAsync(id);
            }
            else
            {
                var apiLesson = await _apiClient.CompletedLessonsGetAsync(id, CancellationToken.None);
                return apiLesson != null ? _apiMapper.ToCommonCompletedLessonDetail(apiLesson) : null;
            }
        }

        public async Task<Guid> CreateOrUpdateAsync(Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel lessonModel)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                var modelToSave = lessonModel;

                if (modelToSave.Id == Guid.Empty)
                {
                    modelToSave = modelToSave with { Id = Guid.NewGuid() };
                }

                await _repository.RemoveAsync(modelToSave.Id);
                await _repository.InsertAsync(modelToSave);
                return modelToSave.Id;
            }
            else
            {
                var apiModel = _apiMapper.ToApiCompletedLessonDetail(lessonModel);
                return await _completedLessonsApiClient.UpsertAsync(apiModel, CancellationToken.None);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                await _repository.RemoveAsync(id);
            }
            else
            {
                await _apiClient.CompletedLessonsDeleteAsync(id, CancellationToken.None);
            }
        }

        public async Task<IList<Common.Models.CompletedLesson.CompletedLessonListModel>> GetAllByUserid(Guid userId, int page = 1, int pageSize = 10, string? sortBy = null)
        {

            if (_localDbOptions.IsLocalDbEnabled)
            {
                var entities = await _repository.GetAllAsync();
                var paged = entities.Where(e => e.UserId == userId).Skip((page - 1) * pageSize).Take(pageSize);
                return _mapper.DetailToListModels(paged);
            }
            else
            {
                var apiLessons = await _usersApiClient.CompletedLessonsAsync(userId,page, pageSize, sortBy, CancellationToken.None);
                return _apiMapper.ToCommonCompletedLessonLists(apiLessons);
            }
        }
    }
}
