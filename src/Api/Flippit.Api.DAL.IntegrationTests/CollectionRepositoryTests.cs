using System.Runtime.InteropServices.JavaScript;
using Xunit;
using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.IntegrationTests;

public class CollectionRepositoryTests : IClassFixture<InMemoryTestDataProvider>
{
    private readonly InMemoryTestDataProvider database;

    public CollectionRepositoryTests(InMemoryTestDataProvider database)
    {
        this.database = database;
    }

    [Fact]
    public void GetById_ExistingCollection_ReturnsCollection()
    {
        var repository = database.GetCollectionRepository();
        var collectionId = database.CollectionGuids[0];

        var collection = database.GetCollectionDirectly(collectionId);
        var result = repository.GetById(collectionId);

        Assert.NotNull(result);
        Assert.Equal(collection, result);
        Assert.Equal(collectionId, result.Id);
    }

    [Fact]
    public void GetById_NonExistingCollection_ReturnsNull()
    {
        var repository = database.GetCollectionRepository();
        var nonExistingId = Guid.NewGuid();

        var result = repository.GetById(nonExistingId);

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllCollections()
    {
        var repository = database.GetCollectionRepository();

        var result = repository.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetAll_WithPagination_ReturnsPagedResults()
    {
        var repository = database.GetCollectionRepository();

        var result = repository.GetAll(null, null, 1, 1);

        Assert.Single(result);
    }

    [Fact]
    public void Search_ReturnsCorrectCollections()
    {
        var repository = database.GetCollectionRepository();
        var wantedCollectionId = database.CollectionGuids[0];

        var result = repository.Search("Search me");

        Assert.NotNull(result);
        Assert.Contains(result, e => e.Id == wantedCollectionId);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Search_NotFound_ReturnsEmpty()
    {
        var repository = database.GetCollectionRepository();

        var result = repository.Search("NonExistentTerm");

        Assert.Empty(result);
    }

    [Fact]
    public void SearchByCreatorId_ReturnsCorrectCollections()
    {
        var repository = database.GetCollectionRepository();
        var creatorId = database.UserGuids[0];
        var wantedCollectionId = database.CollectionGuids[0];

        var result = repository.SearchByCreatorId(creatorId);

        Assert.NotNull(result);
        Assert.Equal(result.First().Id, wantedCollectionId);
    }

    [Fact]
    public void Insert_NewCollection_CollectionIsInserted()
    {
        var repository = database.GetCollectionRepository();
        var newCollectionId = Guid.NewGuid();

        var newCollection = new CollectionEntity
        {
            Id = newCollectionId,
            Name = "New Collection",
            CreatorId = database.UserGuids[0],
            StartTime = DateTime.Now,
            EndTime = DateTime.Now
        };

        repository.Insert(newCollection);

        var result = database.GetCollectionDirectly(newCollectionId);
        Assert.NotNull(result);
        Assert.Equal("New Collection", result.Name);
    }

    [Fact]
    public void Update_ExistingCollection_CollectionIsUpdated()
    {
        var repository = database.GetCollectionRepository();
        var collectionId = database.CollectionGuids[0];

        var collection = repository.GetById(collectionId);
        Assert.NotNull(collection);

        var updatedCollection = new CollectionEntity
        {
            Id = collectionId,
            Name = "Updated Name",
            CreatorId = database.UserGuids[0],
            StartTime = DateTime.Now,
            EndTime = DateTime.Now
        };

        repository.Update(updatedCollection);

        var result = database.GetCollectionDirectly(collectionId);
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public void Delete_ExistingCollection_CollectionIsDeleted()
    {
        var repository = database.GetCollectionRepository();
        var collectionToDeleteId = database.CollectionGuids[0];

        repository.Remove(collectionToDeleteId);

        var result = database.GetCollectionDirectly(collectionToDeleteId);

        Assert.Null(result);
    }

    [Fact]
    public void Exists_ExistingCollection_ReturnsTrue()
    {
        var repository = database.GetCollectionRepository();
        var collectionId = database.CollectionGuids[0];

        var result = repository.Exists(collectionId);

        Assert.True(result);
    }

    [Fact]
    public void Exists_NonExistentCollection_ReturnsFalse()
    {
        var repository = database.GetCollectionRepository();
        var collectionId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        var result = repository.Exists(collectionId);

        Assert.False(result);
    }
}
