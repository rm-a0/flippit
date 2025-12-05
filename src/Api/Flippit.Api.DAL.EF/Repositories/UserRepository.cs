using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Mappers;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.DAL.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flippit.Api.DAL.EF.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FlippitDbContext _dbContext;
        private readonly UserMapper _userMapper;

        public UserRepository(FlippitDbContext dbContext, UserMapper userMapper)
        {
            _dbContext = dbContext;
            _userMapper = userMapper;
        }

        public IList<UserEntity> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            return _dbContext.Users
                .AsQueryable()
                .ApplyFilterSortAndPage(filter, sortBy, page, pageSize)
                .ToList();
        }

        public UserEntity? GetById(Guid id)
        {
            return _dbContext.Users.SingleOrDefault(user => user.Id == id);
        }

        public IEnumerable<UserEntity> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _dbContext.Users.ToList();

            return _dbContext.Users
                .Where(user => user.Name.Contains(searchText))
                .ToList();
        }

        public Guid Insert(UserEntity entity)
        {
            _dbContext.Users.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public Guid? Update(UserEntity userUpdated)
        {
            var userExisting = _dbContext.Users.SingleOrDefault(user => user.Id == userUpdated.Id);
            if (userExisting == null)
                return null;

            _userMapper.UpdateEntity(userUpdated, userExisting);
            _dbContext.Users.Update(userExisting);
            _dbContext.SaveChanges();
            return userExisting.Id;
        }

        public void Remove(Guid id)
        {
            var userToRemove = _dbContext.Users.SingleOrDefault(user => user.Id == id);
            if (userToRemove != null)
            {
                _dbContext.Users.Remove(userToRemove);
                _dbContext.SaveChanges();
            }
        }

        public bool Exists(Guid id)
        {
            return _dbContext.Users.Any(user => user.Id == id);
        }
    }
}
