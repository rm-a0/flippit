using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Memory;
using Xunit;
using System;
using System.Linq;
using Flippit.Api.DAL.Common.Repositories;

namespace Flippit.Api.DAL.IntegrationTests;

public class CollectionRepositoryTests : IClassFixture<InMemoryTestDataProvider>
{
    private readonly InMemoryTestDataProvider _database;
    private readonly ICollectionRepository _repository;

    public CollectionRepositoryTests(InMemoryTestDataProvider database)
    {
        _database = database;
        _database.ResetStorage();
        _repository = database.GetCollectionRepository(); // ← Raz a navždy
    }

    [Fact]
    public void GetById_ExistingCollection_ReturnsCollection()
    {
        var collectionId = _database.CollectionGuids[0];
        var collection = _database.GetCollectionDirectly(collectionId);
        var result = _repository.GetById(collectionId);

        Assert.NotNull(result);
        Assert.Equal(collection, result);
        Assert.Equal(collectionId, result.Id);
    }

    [Fact]
    public void GetById_NonExistingCollection_ReturnsNull()
    {
        var result = _repository.GetById(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllCollections()
    {
        var result = _repository.GetAll();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetAll_WithPagination_ReturnsPagedResults()
    {
        var result = _repository.GetAll(null, null, 1, 1);
        Assert.Single(result); // alebo: Assert.Equal(1, result.Count)
    }

    [Fact]
    public void Search_ReturnsCorrectCollections()
    {
        var wantedCollectionId = _database.CollectionGuids[0];
        var result = _repository.Search("Search me");

        Assert.Contains(result, e => e.Id == wantedCollectionId);
    }

    [Fact]
    public void Search_NotFound_ReturnsEmpty()
    {
        var result = _repository.Search("NonExistentTerm");
        Assert.Empty(result);
    }

    [Fact]
    public void SearchByCreatorId_ReturnsCorrectCollections()
    {
        var creatorId = _database.UserGuids[0];
        var result = _repository.SearchByCreatorId(creatorId);

        Assert.Equal(_database.CollectionGuids[0], result.First().Id);
    }

    [Fact]
    public void Insert_NewCollection_CollectionIsInserted()
    {
        var newCollectionId = Guid.NewGuid();
        var newCollection = new CollectionEntity
        {
            Id = newCollectionId,
            Name = "New Collection",
            CreatorId = _database.UserGuids[0],
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        _repository.Insert(newCollection);

        var result = _database.GetCollectionDirectly(newCollectionId);

        Assert.NotNull(result);
        Assert.Equal(newCollectionId, result.Id);
        Assert.Equal("New Collection", result.Name);
    }

    [Fact]
    public void Update_ExistingCollection_CollectionIsUpdated()
    {
        var collectionId = _database.CollectionGuids[0];
        var collection = _repository.GetById(collectionId);
        Assert.NotNull(collection);

        var updatedCollection = new CollectionEntity
        {
            Id = collectionId,
            Name = "Updated Name",
            CreatorId = _database.UserGuids[0],
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2)
        };

        _repository.Update(updatedCollection);

        var result = _database.GetCollectionDirectly(collectionId);

        Assert.NotNull(result);
        Assert.Equal(collectionId, result.Id);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public void Delete_ExistingCollection_CollectionIsDeleted()
    {
        var collectionToDeleteId = _database.CollectionGuids[0];

        _repository.Remove(collectionToDeleteId);

        var result = _database.GetCollectionDirectly(collectionToDeleteId);
        Assert.Null(result);
    }

    [Fact]
    public void Exists_ExistingCollection_ReturnsTrue()
    {
        var result = _repository.Exists(_database.CollectionGuids[0]);
        Assert.True(result);
    }

    [Fact]
    public void Exists_NonExistentCollection_ReturnsFalse()
    {
        var result = _repository.Exists(Guid.Parse("00000000-0000-0000-0000-000000000000"));
        Assert.False(result);
    }
}
