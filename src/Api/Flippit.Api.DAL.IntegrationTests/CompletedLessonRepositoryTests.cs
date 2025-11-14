using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Memory;
using Xunit;
using System;
using System.Linq;
using Flippit.Api.DAL.Common.Repositories;

namespace Flippit.Api.DAL.IntegrationTests;

public class CompletedLessonRepositoryTests : IClassFixture<InMemoryTestDataProvider>
{
    private readonly InMemoryTestDataProvider _database;
    private readonly ICompletedLessonRepository _repository;

    public CompletedLessonRepositoryTests(InMemoryTestDataProvider database)
    {
        _database = database;
        _database.ResetStorage();
        _repository = database.GetCompletedLessonRepository();
    }

    [Fact]
    public void GetById_ExistingCompletedLesson_ReturnsCompletedLesson()
    {
        var completedLessonId = _database.CompletedLessonGuids[0];
        var completedLesson = _database.GetCompletedLessonDirectly(completedLessonId);
        var result = _repository.GetById(completedLessonId);

        Assert.NotNull(result);
        Assert.Equal(completedLesson, result);
        Assert.Equal(completedLessonId, result.Id);
    }

    [Fact]
    public void GetById_NonExistingCompletedLesson_ReturnsNull()
    {
        var result = _repository.GetById(Guid.Empty);
        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllCompletedLessons()
    {
        var result = _repository.GetAll();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetAll_WithPagination_ReturnsPagedResults()
    {
        var result = _repository.GetAll(null, null, 1, 1);
        Assert.Single(result);
    }

    [Fact]
    public void Search_ReturnsCorrectCards()
    {
        var result = _repository.Search("Search");

        Assert.Contains(result, l => l.Id == _database.CompletedLessonGuids[0]);
    }

    [Fact]
    public void SearchByUserId_ReturnsCorrectCompletedLessons()
    {
        var userId = _database.UserGuids[0];
        var wantedCompletedLessonId = _database.CompletedLessonGuids[0];
        var result = _repository.SearchByCreatorId(userId);

    Assert.NotNull(result);
    Assert.Contains(result, l => l.Id == wantedCompletedLessonId);
    }

    [Fact]
    public void SearchByCollectionId_ReturnsCorrectCompletedLessons()
    {
        var collectionId = _database.CollectionGuids[0];
        var result = _repository.SearchByCollectionId(collectionId);

        Assert.Contains(result, l => l.Id == _database.CompletedLessonGuids[0]);
    }

    [Fact]
    public void Insert_CompletedLessonIsInserted()
    {
        var newCompletedLessonId = Guid.NewGuid();
        var newCompletedLesson = new CompletedLessonEntity
        {
            Id = newCompletedLessonId,
            UserId = _database.UserGuids[0],
            CollectionId = _database.CollectionGuids[0],
            AnswersJson = "Answers",
            StatisticsJson = "Statistics"
        };

        _repository.Insert(newCompletedLesson);

        var result = _database.GetCompletedLessonDirectly(newCompletedLessonId);

        Assert.NotNull(result);
        Assert.Equal(newCompletedLessonId, result.Id);
        Assert.Equal("Answers", result.AnswersJson);
        Assert.Equal("Statistics", result.StatisticsJson);
    }

    [Fact]
    public void Update_ExistingCompletedLesson_CompletedLessonIsUpdated()
    {
        var completedLessonId = _database.CompletedLessonGuids[0];
        var original = _database.GetCompletedLessonDirectly(completedLessonId);
        Assert.NotNull(original);

        var updatedCompletedLesson = new CompletedLessonEntity
        {
            Id = completedLessonId,
            UserId = original.UserId,
            CollectionId = original.CollectionId,
            AnswersJson = "Updated",
            StatisticsJson = "Updated"
        };

        _repository.Update(updatedCompletedLesson);

        var result = _database.GetCompletedLessonDirectly(completedLessonId);

        Assert.NotNull(result);
        Assert.Equal(completedLessonId, result.Id);
        Assert.Equal("Updated", result.AnswersJson);
        Assert.Equal("Updated", result.StatisticsJson);
    }

    [Fact]
    public void Delete_ExistingCompletedLesson_CompletedLessonIsDeleted()
    {
        var completedLessonToDeleteId = _database.CompletedLessonGuids[0];

        _repository.Remove(completedLessonToDeleteId);

        var result = _database.GetCompletedLessonDirectly(completedLessonToDeleteId);
        Assert.Null(result);
    }

    [Fact]
    public void Exists_ExistingCompletedLesson_ReturnsTrue()
    {
        var result = _repository.Exists(_database.CompletedLessonGuids[0]);
        Assert.True(result);
    }

    [Fact]
    public void Exists_NonExistentCompletedLesson_ReturnsFalse()
    {
        var result = _repository.Exists(Guid.Empty);
        Assert.False(result);
    }
}
