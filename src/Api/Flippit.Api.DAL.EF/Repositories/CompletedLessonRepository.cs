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
    public class CompletedLessonRepository : ICompletedLessonRepository
    {
        private readonly FlippitDbContext _dbContext;
        private readonly CompletedLessonMapper _completedLessonMapper;

        public CompletedLessonRepository(FlippitDbContext dbContext, CompletedLessonMapper mapper)
        {
            _dbContext = dbContext;
            _completedLessonMapper = mapper;
        }

        public IList<CompletedLessonEntity> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var lessons = _dbContext.CompletedLessons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter) && Guid.TryParse(filter, out var guidFilter))
            {
                lessons = lessons.Where(l => l.UserId == guidFilter || l.CollectionId == guidFilter);
            }

            return lessons.ApplySortAndPage(sortBy, page, pageSize)
                          .ToList();
        }

        public CompletedLessonEntity? GetById(Guid id)
        {
            return _dbContext.CompletedLessons.SingleOrDefault(lesson => lesson.Id == id);
        }

        public IEnumerable<CompletedLessonEntity> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText) || !Guid.TryParse(searchText, out var guidSearch))
                return _dbContext.CompletedLessons.ToList();

            return _dbContext.CompletedLessons
                .Where(lesson => lesson.UserId == guidSearch || lesson.CollectionId == guidSearch)
                .ToList();
        }

        public IEnumerable<CompletedLessonEntity> SearchByCreatorId(Guid creatorId, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var query = _dbContext.CompletedLessons.Where(l => l.UserId == creatorId)
                .ApplySortAndPage(sortBy, page, pageSize)
                .Skip((page-1)*pageSize).Take(pageSize);

            return query.ToList();
        }

        public IEnumerable<CompletedLessonEntity> SearchByCollectionId(Guid collectionId, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var query = _dbContext.CompletedLessons.Where(l => l.CollectionId == collectionId)
                .ApplySortAndPage(sortBy, page, pageSize)
                .Skip((page-1)*pageSize).Take(pageSize);

            return query.ToList();
        }

        public Guid Insert(CompletedLessonEntity entity)
        {
            _dbContext.CompletedLessons.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public Guid? Update(CompletedLessonEntity lessonUpdated)
        {
            var lessonExisting = _dbContext.CompletedLessons.SingleOrDefault(lesson => lesson.Id == lessonUpdated.Id);
            if (lessonExisting == null)
                return null;

            _completedLessonMapper.UpdateEntity(lessonUpdated, lessonExisting);
            _dbContext.CompletedLessons.Update(lessonExisting);
            _dbContext.SaveChanges();
            return lessonExisting.Id;
        }

        public void Remove(Guid id)
        {
            var lessonToRemove = _dbContext.CompletedLessons.SingleOrDefault(lesson => lesson.Id == id);
            if (lessonToRemove != null)
            {
                _dbContext.CompletedLessons.Remove(lessonToRemove);
                _dbContext.SaveChanges();
            }
        }

        public bool Exists(Guid id)
        {
            return _dbContext.CompletedLessons.Any(lesson => lesson.Id == id);
        }
    }
}
