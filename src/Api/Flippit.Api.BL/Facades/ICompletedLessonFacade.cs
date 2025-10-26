using System;
using System.Collections.Generic;
using Flippit.Common.BL.Facades;
using Flippit.Common.Models.CompletedLesson;

namespace Flippit.Api.BL.Facades
{
    public interface ICompletedLessonFacade : IAppFacade
    {
        IList<CompletedLessonListModel> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        CompletedLessonDetailModel? GetById(Guid id);
        IList<CompletedLessonListModel> Search(string searchText);
        IList<CompletedLessonListModel> SearchByCreatorId(Guid creatorId, string? sortBy = null, int page = 1, int pageSize = 10);
        IList<CompletedLessonListModel> SearchByCollectionId(Guid collectionId, string? sortBy = null, int page = 1, int pageSize = 10);
        Guid CreateOrUpdate(CompletedLessonDetailModel completedLessonModel);
        Guid Create(CompletedLessonDetailModel completedLessonModel);
        Guid? Update(CompletedLessonDetailModel completedLessonModel);
        void Delete(Guid id);
    }
}
