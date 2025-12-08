using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Api.BL.Services;
using Flippit.Common.Models.CompletedLesson;
using Flippit.Api.BL.Validators;

namespace Flippit.Api.BL.Facades
{
    public class CompletedLessonFacade : ICompletedLessonFacade
    {
        private readonly ICompletedLessonRepository _repository;
        private readonly CompletedLessonMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public CompletedLessonFacade(ICompletedLessonRepository repository, CompletedLessonMapper mapper, ICurrentUserService currentUserService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public IList<CompletedLessonListModel> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            PaginationValidator.Validate(page, pageSize);
            var entities = _repository.GetAll(filter, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public CompletedLessonDetailModel? GetById(Guid id)
        {
            var entity = _repository.GetById(id);
            return entity != null ? _mapper.ToDetailModel(entity) : null;
        }

        public IList<CompletedLessonListModel> Search(string searchText)
        {
            var entities = _repository.Search(searchText);
            return _mapper.ToListModels(entities);
        }

        public IList<CompletedLessonListModel> SearchByCreatorId(Guid creatorId, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = _repository.SearchByCreatorId(creatorId, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public IList<CompletedLessonListModel> SearchByCollectionId(Guid collectionId, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = _repository.SearchByCollectionId(collectionId, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public Guid CreateOrUpdate(CompletedLessonDetailModel lessonModel)
        {
            ValidateCompletedLessonDetailModel(lessonModel);
            var entity = _mapper.ModelToEntity(lessonModel);
            entity = entity with { UserId = EnsureAuthenticatedUserId() };
            return _repository.Exists(entity.Id) ? _repository.Update(entity) ?? entity.Id : _repository.Insert(entity);
        }

        public Guid Create(CompletedLessonDetailModel lessonModel)
        {
            ValidateCompletedLessonDetailModel(lessonModel);
            var entity = _mapper.ModelToEntity(lessonModel);
            entity = entity with { UserId = EnsureAuthenticatedUserId() };
            return _repository.Insert(entity);
        }

        public Guid? Update(CompletedLessonDetailModel lessonModel)
        {
            ValidateCompletedLessonDetailModel(lessonModel);
            var entity = _mapper.ModelToEntity(lessonModel);
            entity = entity with { UserId = EnsureAuthenticatedUserId() };
            return _repository.Update(entity);
        }

        public void Delete(Guid id)
        {
            _repository.Remove(id);
        }

        private static void ValidateCompletedLessonDetailModel(CompletedLessonDetailModel lessonModel)
        {
            if (lessonModel == null)
                throw new ArgumentNullException(nameof(lessonModel));
            if (lessonModel.CollectionId == Guid.Empty)
                throw new ArgumentException("Collection ID cannot be empty.", nameof(lessonModel.CollectionId));
        }

        private Guid EnsureAuthenticatedUserId()
        {
            var userId = _currentUserService.CurrentUserId;
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User must be authenticated to perform this operation.");
            return userId.Value;
        }
    }
}
