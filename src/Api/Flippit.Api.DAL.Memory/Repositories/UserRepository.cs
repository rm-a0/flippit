using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flippit.Api.DAL.Memory.Repositories
{
    public class UserRepository
    {
        private readonly IList<UserEntity> _users;
        private readonly UserMapper _userMapper;

        public UserRepository(Storage storage, UserMapper userMapper)
        {
            _users = storage.Users;
            _userMapper = userMapper;
        }

        public IList<UserEntity> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var users = _users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                users = users.Where(u => u.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                users = sortBy.ToLower() switch
                {
                    "name" => users.OrderBy(u => u.Name),
                    "id" => users.OrderBy(u => u.Id),
                    _ => users
                };
            }

            users = users.Skip((page - 1) * pageSize).Take(pageSize);
            return users.ToList();
        }

        public UserEntity? GetById(Guid id)
        {
            return _users.SingleOrDefault(user => user.Id == id);
        }

        public IEnumerable<UserEntity> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _users;

            return _users.Where(user => user.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        public Guid Insert(UserEntity entity)
        {
            _users.Add(entity);
            return entity.Id;
        }

        public Guid? Update(UserEntity userUpdated)
        {
            var userExisting = _users.SingleOrDefault(user => user.Id == userUpdated.Id);
            if (userExisting == null)
                return null;

            _userMapper.UpdateEntity(userUpdated, userExisting);
            return userExisting.Id;
        }

        public void Remove(Guid id)
        {
            var userToRemove = _users.SingleOrDefault(user => user.Id == id);
            if (userToRemove != null)
            {
                _users.Remove(userToRemove);
            }
        }

        public bool Exists(Guid id)
        {
            return _users.Any(user => user.Id == id);
        }
    }
}
