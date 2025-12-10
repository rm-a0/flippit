using Flippit.Common.Models;
using Flippit.Common.Models.User;
using Flippit.Web.BL.Mappers;
using Flippit.Web.DAL.Repositories;

namespace Flippit.Web.BL.Facades
{
    public class UserFacade : IUserFacade
    {
        private readonly UserRepository _repository;
        private readonly UserMapper _mapper;
        private readonly LocalDbOptions _localDbOptions;
        private readonly IApiApiClient _apiClient;
        private readonly IUsersApiClient _usersApiClient;
        private readonly IAuthApiClient _authApiClient;
        private readonly ApiModelMapper _apiMapper;

        public UserFacade(
            UserRepository repository, 
            UserMapper mapper, 
            LocalDbOptions localDbOptions,
            IApiApiClient apiClient,
            IUsersApiClient usersApiClient,
            IAuthApiClient authApiClient,
            ApiModelMapper apiMapper)
        {
            _repository = repository;
            _mapper = mapper;
            _localDbOptions = localDbOptions;
            _apiClient = apiClient;
            _usersApiClient = usersApiClient;
            _apiMapper = apiMapper;
            _authApiClient = authApiClient;
        }

        public async Task<IList<Flippit.Common.Models.User.UserListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                var entities = await _repository.GetAllAsync();

                var query = entities.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query = query.Where(u => u.Name != null && u.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));
                }

                var pagedEntities = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                return _mapper.DetailToListModels(pagedEntities);
            }
            else
            {
                var apiUsers = await _apiClient.UsersGetAsync(filter, page, pageSize, sortBy, CancellationToken.None);
                return _apiMapper.ToCommonUserLists(apiUsers);
            }
        }

        public async Task<Flippit.Common.Models.User.UserDetailModel?> GetByIdAsync(Guid id)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                return await _repository.GetByIdAsync(id);
            }
            else
            {
                var apiUser = await _apiClient.UsersGetAsync(id, CancellationToken.None);
                return apiUser != null ? _apiMapper.ToCommonUserDetail(apiUser) : null;
            }
        }

        public async Task<Guid> CreateOrUpdateAsync(Flippit.Common.Models.User.UserDetailModel userModel)
        {
            if (_localDbOptions.IsLocalDbEnabled)
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
            else
            {
                var apiModel = _apiMapper.ToApiUserDetail(userModel);
                return await _usersApiClient.UpsertAsync(apiModel, CancellationToken.None);
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
                await _apiClient.UsersDeleteAsync(id, CancellationToken.None);
            }
        }

        public async Task<(Flippit.Common.Models.AuthModels.LoginResponse?, string?)> RegisterLoginAsync(Flippit.Common.Models.AuthModels.RegisterRequest registerRequest)
        {
            try
            {
                var response = await _authApiClient.RegisterAsync(_apiMapper.ToApiRegisterModel(registerRequest), CancellationToken.None);
            }catch(ApiException<string> e)
            {
                return (null, e.Result);
            }

            LoginResponse loginResponse;
            try
            {
                loginResponse = await _authApiClient.LoginAsync(new LoginRequest { UserName = registerRequest.UserName, Password = registerRequest.Password });
            }
            catch(ApiException<string> e)
            {
                return (null, e.Result);
            }

            return (_apiMapper.ToCommonLoginResponse(loginResponse), null);

        }

        public async Task<(Flippit.Common.Models.AuthModels.LoginResponse?, string?)> LoginAsync(Flippit.Common.Models.AuthModels.LoginRequest loginRequest)
        {
            LoginResponse loginResponse;
            try
            {
                loginResponse = await _authApiClient.LoginAsync(_apiMapper.ToApiLoginModel(loginRequest));
            }
            catch (ApiException<string> e)
            {
                return (null, e.Result);
            }

            return (_apiMapper.ToCommonLoginResponse(loginResponse), null);
        }
    }
}
