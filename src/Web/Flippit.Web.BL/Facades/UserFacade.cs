using Flippit.Common.Models.User;
using Flippit.Web.BL.Mappers;
using Flippit.Web.DAL.Repositories;

namespace Flippit.Web.BL.Facades
{
    public class UserFacade : IUserFacade
    {
        private readonly UserRepository _repository;
        private readonly UserMapper _mapper;

        public UserFacade(UserRepository repository, UserMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IList<Flippit.Common.Models.User.UserListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = await _repository.GetAllAsync();

            var query = entities.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(u => u.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }

            var pagedEntities = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return _mapper.DetailToListModels(pagedEntities);
        }

        public async Task<Flippit.Common.Models.User.UserDetailModel?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Guid> CreateOrUpdateAsync(Flippit.Common.Models.User.UserDetailModel userModel)
        {
            var modelToSave = userModel;
    
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
