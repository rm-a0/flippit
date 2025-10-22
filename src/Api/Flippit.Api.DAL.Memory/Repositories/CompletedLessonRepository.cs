// Flippit.Api.DAL.Memory.Repositories/CompletedLessonRepository.cs
using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flippit.Api.DAL.Memory.Repositories
{
    public class CompletedLessonRepository
    {
        private readonly IList<CompletedLessonEntity> _completedLessons;
        private readonly CompletedLessonMapper _completedLessonMapper;

        public CompletedLessonRepository(Storage storage, CompletedLessonMapper completedLessonMapper)
        {
            _completedLessons = storage.CompletedLessons;
            _completedLessonMapper = completedLessonMapper;
        }

        public IList<CompletedLessonEntity> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var lessons = _completedLessons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter) && Guid.TryParse(filter, out var guidFilter))
            {
                lessons = lessons.Where(l => l.UserId == guidFilter || l.CollectionId == guidFilter);
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                lessons = sortBy.ToLower() switch
                {
                    "id" => lessons.OrderBy(l => l.Id),
                    "userid" => lessons.OrderBy(l => l.UserId),
                    "collectionid" => lessons.OrderBy(l => l.CollectionId),
                    _ => lessons
                };
            }

            lessons = lessons.Skip((page - 1) * pageSize).Take(pageSize);
            return lessons.ToList();
        }

        public CompletedLessonEntity? GetById(Guid id)
        {
            return _completedLessons.SingleOrDefault(lesson => lesson.Id == id);
        }

        public IEnumerable<CompletedLessonEntity> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText) || !Guid.TryParse(searchText, out var guidSearch))
                return _completedLessons;

            return _completedLessons.Where(lesson => lesson.UserId == guidSearch || lesson.CollectionId == guidSearch);
        }

        public IEnumerable<CompletedLessonEntity> SearchByCreatorId(Guid creatorId)
        {
            return _completedLessons.Where(l => l.UserId == creatorId);
        }

        public IEnumerable<CompletedLessonEntity> SearchByCollectionId(Guid collectionId)
        {
            return _completedLessons.Where(l => l.CollectionId == collectionId);
        }

        public Guid Insert(CompletedLessonEntity entity)
        {
            _completedLessons.Add(entity);
            return entity.Id;
        }

        public Guid? Update(CompletedLessonEntity lessonUpdated)
        {
            var lessonExisting = _completedLessons.SingleOrDefault(lesson => lesson.Id == lessonUpdated.Id);
            if (lessonExisting == null)
                return null;

            _completedLessonMapper.UpdateEntity(lessonUpdated, lessonExisting);
            return lessonExisting.Id;
        }

        public void Remove(Guid id)
        {
            var lessonToRemove = _completedLessons.SingleOrDefault(lesson => lesson.Id == id);
            if (lessonToRemove != null)
            {
                _completedLessons.Remove(lessonToRemove);
            }
        }

        public bool Exists(Guid id)
        {
            return _completedLessons.Any(lesson => lesson.Id == id);
        }
    }
}
