using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Memory;
using Xunit;
using System;
using System.Linq;
using Flippit.Api.DAL.Common.Repositories;

namespace Flippit.Api.DAL.IntegrationTests;

public class UserRepositoryTests : IClassFixture<InMemoryTestDataProvider>
{
    private readonly InMemoryTestDataProvider _database;
    private readonly IUserRepository _repository;

    public UserRepositoryTests(InMemoryTestDataProvider database)
    {
        _database = database;
        _database.ResetStorage();
        _repository = database.GetUserRepository();
    }

    [Fact]
    public void GetById_ExistingUser_ReturnsUser()
    {
        var userId = _database.UserGuids[0];
        var user = _database.GetUserDirectly(userId);
        var result = _repository.GetById(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal(user!.Name, result.Name);
    }

    [Fact]
    public void GetById_NonExistingUser_ReturnsNull()
    {
        var result = _repository.GetById(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllUsers()
    {
        var result = _repository.GetAll();
        Assert.Equal(_database.UserGuids.Count, result.Count);
    }

    [Fact]
    public void GetAll_WithPagination_ReturnsPagedResults()
    {
        var result = _repository.GetAll(null, null, 1, 2);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Searching_ReturnsCorrectUsers()
    {
        var result = _repository.Search("Test User 1");

        Assert.Contains(result, u => u.Name == "Test User 1");
    }

    [Fact]
    public void Searching_NotFound_ReturnsEmpty()
    {
        var result = _repository.Search("NonExistent User");
        Assert.Empty(result);
    }

    [Fact]
    public void Inserting_UserIsInserted()
    {
        var newUserId = Guid.NewGuid();
        var newUser = new UserEntity
        {
            Id = newUserId,
            Name = "New Test User"
        };

        _repository.Insert(newUser);

        var result = _database.GetUserDirectly(newUserId);

        Assert.NotNull(result);
        Assert.Equal(newUserId, result.Id);
        Assert.Equal("New Test User", result.Name);
    }

    [Fact]
    public void Updating_ExistingUser_UserIsUpdated()
    {
        var userId = _database.UserGuids[0];
        var updatedUser = new UserEntity
        {
            Id = userId,
            Name = "Updated User Name"
        };

        _repository.Update(updatedUser);

        var result = _database.GetUserDirectly(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("Updated User Name", result.Name);
    }

    [Fact]
    public void Updating_NonExistingUser_ReturnsNull()
    {
        var nonExistingUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = "Non Existing User"
        };

        var result = _repository.Update(nonExistingUser);
        Assert.Null(result);
    }

    [Fact]
    public void Deleting_ExistingUser_UserIsDeleted()
    {
        var userId = _database.UserGuids[0];

        _repository.Remove(userId);

        var result = _database.GetUserDirectly(userId);
        Assert.Null(result);
    }

    [Fact]
    public void Exists_ExistingUser_ReturnsTrue()
    {
        var result = _repository.Exists(_database.UserGuids[0]);
        Assert.True(result);
    }

    [Fact]
    public void Exists_NonExistingUser_ReturnsFalse()
    {
        var result = _repository.Exists(Guid.Empty);
        Assert.False(result);
    }
}
