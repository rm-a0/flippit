using Flippit.Common.Models.Collection;
using Flippit.Web.BL.Mappers;
using Flippit.Web.DAL.Repositories;

namespace Flippit.Web.BL.Facades
{
    public class CollectionFacade : ICollectionFacade
    {
        private readonly CollectionRepository _repository;
        private readonly CollectionMapper _mapper;

        public CollectionFacade(CollectionRepository repository, CollectionMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IList<CollectionListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = await _repository.GetAllAsync();
            var query = entities.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(c => c.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }

            var paged = query.Skip((page - 1) * pageSize).Take(pageSize);
            return _mapper.DetailToListModels(paged);
        }

        public async Task<CollectionDetailModel?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Guid> CreateOrUpdateAsync(CollectionDetailModel collectionModel)
        {
            if (collectionModel.StartTime == default) collectionModel.StartTime = DateTime.Now;
            if (collectionModel.EndTime == default) collectionModel.EndTime = DateTime.Now;

            var modelToSave = collectionModel;

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
