using Flippit.Common.Models.CompletedLesson;
using Flippit.Web.BL.Mappers;
using Flippit.Web.DAL.Repositories;

namespace Flippit.Web.BL.Facades
{
    public class CompletedLessonFacade : ICompletedLessonFacade
    {
        private readonly CompletedLessonRepository _repository;
        private readonly CompletedLessonMapper _mapper;

        public CompletedLessonFacade(CompletedLessonRepository repository, CompletedLessonMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IList<Flippit.Common.Models.CompletedLesson.CompletedLessonListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = await _repository.GetAllAsync();
            var paged = entities.Skip((page - 1) * pageSize).Take(pageSize);
            return _mapper.DetailToListModels(paged);
        }

        public async Task<IList<Flippit.Common.Models.CompletedLesson.CompletedLessonListModel>> SearchByCollectionIdAsync(Guid collectionId)
        {
            var entities = await _repository.GetAllAsync();
            var filtered = entities.Where(x => x.CollectionId == collectionId);
            return _mapper.DetailToListModels(filtered);
        }

        public async Task<Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Guid> CreateOrUpdateAsync(Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel lessonModel)
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

        public async Task DeleteAsync(Guid id)
        {
            await _repository.RemoveAsync(id);
        }
    }
}
