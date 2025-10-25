using System;
using System.Collections.Generic;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Repositories;

namespace Flippit.Api.DAL.IntegrationTests;

public interface ITestDataProvider
{
    CardEntity? GetCardDirectly(Guid id);
    CollectionEntity? GetCollectionDirectly(Guid id); 
    CompletedLessonEntity? GetCompletedLessonDirectly(Guid id);
    UserEntity? GetUserDirectly(Guid id);
    
    ICardRepository GetCardRepository();
    ICollectionRepository GetCollectionRepository();
    ICompletedLessonRepository GetCompletedLessonRepository();
    IUserRepository GetUserRepository();
    
    IList<Guid> CardGuids { get; }
    IList<Guid> CollectionGuids { get; }
    IList<Guid> CompletedLessonGuids { get; }
    IList<Guid> UserGuids { get; }
}
