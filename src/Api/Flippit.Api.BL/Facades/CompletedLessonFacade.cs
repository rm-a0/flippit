using System;
using System.Collections.Generic;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Common.BL.Facades;
using Flippit.Common.Models.CompletedLesson;
using Flippit.Api.BL.Validators;

namespace Flippit.Api.BL.Facades
{
    public class CompletedLessonFacade : ICompletedLessonFacade
    {
        private readonly ICompletedLessonRepository _repository;
        private readonly CompletedLessonMapper _mapper;

        public CompletedLessonFacade(ICompletedLessonRepository repository, CompletedLessonMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            var entity = _mapper.ModelToEntity(lessonModel);
            return _repository.Exists(entity.Id) ? _repository.Update(entity) ?? entity.Id : _repository.Insert(entity);
        }

        public Guid Create(CompletedLessonDetailModel lessonModel)
        {
            var entity = _mapper.ModelToEntity(lessonModel);
            return _repository.Insert(entity);
        }

        public Guid? Update(CompletedLessonDetailModel lessonModel)
        {
            var entity = _mapper.ModelToEntity(lessonModel);
            return _repository.Update(entity);
        }

        public void Delete(Guid id)
        {
            _repository.Remove(id);
        }
    }
}
