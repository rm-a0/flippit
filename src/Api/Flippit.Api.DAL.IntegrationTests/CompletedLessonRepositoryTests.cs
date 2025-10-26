using Xunit;
using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.IntegrationTests;

public class CompletedLessonRepositoryTests : IClassFixture<InMemoryTestDataProvider>
{
    private readonly InMemoryTestDataProvider database;

    public CompletedLessonRepositoryTests(InMemoryTestDataProvider database)
    {
        this.database = database;
        database.ResetStorage();
    }

    [Fact]
    public void GetById_ExistingCompletedLesson_ReturnsCompletedLesson()
    {
        var repository = database.GetCompletedLessonRepository();
        var completedLessonId = database.CompletedLessonGuids[0];

        var completedLesson = database.GetCompletedLessonDirectly(completedLessonId);
        var result = repository.GetById(completedLessonId);

        Assert.NotNull(result);
        Assert.Equal(completedLesson, result);
        Assert.Equal(completedLessonId, result.Id);
    }
    
    [Fact]
    public void GetById_NonExistingCompletedLesson_ReturnsNull()
    {
        var repository = database.GetCompletedLessonRepository();
        var nonExistingId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        
        var result = repository.GetById(nonExistingId);

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllCompletedLessons()
    {
        var repository = database.GetCompletedLessonRepository();

        var result = repository.GetAll();

        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public void GetAll_WithPagination_ReturnsPagedResults()
    {
        var repository = database.GetCompletedLessonRepository();

        var result = repository.GetAll(null, null, 1, 1);

        Assert.Single(result);
    }
    
    [Fact]
    public void Search_ReturnsCorrectCards()
    {
        var repository = database.GetCompletedLessonRepository();
        
        var result = repository.Search("Search");
        
        Assert.NotNull(result);
        Assert.Contains(result, l => l.Id == database.CompletedLessonGuids[0]);
    }
    
    [Fact]
    public void SearchByUserId_ReturnsCorrectCompletedLessons()
    {
        var repository = database.GetCompletedLessonRepository();
        var userId = database.UserGuids[0];
        var wantedCompletedLessonId = database.CompletedLessonGuids[0];
        
        var result = repository.SearchByCreatorId(userId);
        
        Assert.NotNull(result);
        Assert.Contains(result, l => l.Id == wantedCompletedLessonId);
    }
    
    [Fact]
    public void SearchByCollectionId_ReturnsCorrectCompletedLessons()
    {
        var repository = database.GetCompletedLessonRepository();
        var collectionId = database.CollectionGuids[0];
        
        var result = repository.SearchByCollectionId(collectionId);
        
        var wantedCompletedLessonId = database.CompletedLessonGuids[0];
        
        Assert.NotNull(result);
        Assert.Contains(result, l => l.Id == wantedCompletedLessonId);
    }

    [Fact]
    public void Insert_CompletedLessonIsInserted()
    {
        var repository = database.GetCompletedLessonRepository();
        var newCompletedLessonId = Guid.NewGuid();

        var newCompletedLesson = new CompletedLessonEntity
        {
            Id = newCompletedLessonId,
            UserId = database.UserGuids[0],
            CollectionId = database.CollectionGuids[0],
            AnswersJson = "Answers",
            StatisticsJson = "Statistics"
        };

        repository.Insert(newCompletedLesson);

        var result = database.GetCompletedLessonDirectly(newCompletedLessonId);
        
        Assert.NotNull(result);
        Assert.Equal(result, newCompletedLesson);
    }
    
    [Fact]
    public void Update_ExistingCompletedLesson_CompletedLessonIsUpdated()
    {
        var repository = database.GetCompletedLessonRepository();
        var completedLessonId = database.CompletedLessonGuids[0];
        
        var completedLesson = database.GetCompletedLessonDirectly(completedLessonId);
        Assert.NotNull(completedLesson);
        
        var updatedCompletedLesson = new CompletedLessonEntity
        {
            Id = completedLessonId,
            UserId = completedLesson.UserId,
            CollectionId = completedLesson.CollectionId,
            AnswersJson = "Updated",
            StatisticsJson = "Updated"
        };
        
        repository.Update(updatedCompletedLesson);
        
        var result = database.GetCompletedLessonDirectly(completedLessonId);
        Assert.NotNull(result);
        Assert.Equal("Updated", result.AnswersJson);
        Assert.Equal("Updated", result.StatisticsJson);
    }
    
    [Fact]
    public void Delete_ExistingCompletedLesson_CompletedLessonIsDeleted()
    {
        var repository = database.GetCompletedLessonRepository();
        var completedLessonToDeleteId = database.CompletedLessonGuids[0];
        
        repository.Remove(completedLessonToDeleteId);
        
        var result = database.GetCompletedLessonDirectly(completedLessonToDeleteId);
        
        Assert.Null(result);
    }
    
    [Fact]
    public void Exists_ExistingCompletedLesson_ReturnsTrue()
    {
        var repository = database.GetCompletedLessonRepository();
        var completedLessonId = database.CompletedLessonGuids[0];
        
        var result = repository.Exists(completedLessonId);
        
        Assert.True(result);
    }
    
    [Fact]
    public void Exists_NonExistentCompletedLesson_ReturnsFalse()
    {
        var repository = database.GetCompletedLessonRepository();
        var completedLessonId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        
        var result = repository.Exists(completedLessonId);
        
        Assert.False(result);
    }
}
