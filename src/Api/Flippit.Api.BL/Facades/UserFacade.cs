using System;
using System.Collections.Generic;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Memory.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Common.Models.User;
using Flippit.Api.DAL.Common.Repositories;

namespace Flippit.Api.BL.Facades
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserRepository _repository;
        private readonly UserMapper _mapper;

        public UserFacade(IUserRepository repository, UserMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IList<UserListModel> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            if (page < 1)
                throw new ArgumentException("Page number must be greater than or equal to 1.", nameof(page));
            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than or equal to 1.", nameof(pageSize));

            var entities = _repository.GetAll(filter, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public UserDetailModel? GetById(Guid id)
        {
            var entity = _repository.GetById(id);
            return entity != null ? _mapper.ToDetailModel(entity) : null;
        }

        public IList<UserListModel> Search(string searchText)
        {
            var entities = _repository.Search(searchText);
            return _mapper.ToListModels(entities);
        }

        public Guid CreateOrUpdate(UserDetailModel userModel)
        {
            if (userModel == null)
                throw new ArgumentNullException(nameof(userModel));
            if (string.IsNullOrWhiteSpace(userModel.Name))
                throw new ArgumentException("User name cannot be empty.", nameof(userModel.Name));

            var entity = _mapper.ModelToEntity(userModel);
            return _repository.Exists(entity.Id) ? _repository.Update(entity) ?? entity.Id : _repository.Insert(entity);
        }

        public Guid Create(UserDetailModel userModel)
        {
            if (userModel == null)
                throw new ArgumentNullException(nameof(userModel));
            if (string.IsNullOrWhiteSpace(userModel.Name))
                throw new ArgumentException("User name cannot be empty.", nameof(userModel.Name));

            var entity = _mapper.ModelToEntity(userModel);
            return _repository.Insert(entity);
        }

        public Guid? Update(UserDetailModel userModel)
        {
            if (userModel == null)
                throw new ArgumentNullException(nameof(userModel));
            if (string.IsNullOrWhiteSpace(userModel.Name))
                throw new ArgumentException("User name cannot be empty.", nameof(userModel.Name));

            var entity = _mapper.ModelToEntity(userModel);
            return _repository.Update(entity);
        }

        public void Delete(Guid id)
        {
            _repository.Remove(id);
        }
    }
}
