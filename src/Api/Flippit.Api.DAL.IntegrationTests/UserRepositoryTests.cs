using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.IntegrationTests;

public class UserRepositoryTests : IClassFixture<InMemoryTestDataProvider>
{
    private readonly InMemoryTestDataProvider database;

    public UserRepositoryTests(InMemoryTestDataProvider database)
    {
        this.database = database;
        database.ResetStorage();
    }

    [Fact]
    public void GetById_ExistingUser_ReturnsUser()
    {
        var repository = database.GetUserRepository();
        var userId = database.UserGuids[0];

        var user = database.GetUserDirectly(userId);
        var result = repository.GetById(userId);

        Assert.NotNull(result);
        Assert.Equal(user, result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public void GetById_NonExistingUser_ReturnsNull()
    {
        var repository = database.GetUserRepository();
        var nonExistingId = Guid.NewGuid();

        var result = repository.GetById(nonExistingId);

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllUsers()
    {
        var repository = database.GetUserRepository();

        var result = repository.GetAll();

        Assert.NotEmpty(result);
        Assert.Equal(database.UserGuids.Count, result.Count);
    }

    [Fact]
    public void GetAll_WithPagination_ReturnsPagedResults()
    {
        var repository = database.GetUserRepository();

        var result = repository.GetAll(null, null, 1, 2);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Search_ReturnsCorrectUsers()
    {
        var repository = database.GetUserRepository();

        var result = repository.Search("Test User 1");

        Assert.NotNull(result);
        Assert.Contains(result, u => u.Name == "Test User 1");
    }

    [Fact]
    public void Search_NotFound_ReturnsEmpty()
    {
        var repository = database.GetUserRepository();

        var result = repository.Search("NonExistent User");

        Assert.Empty(result);
    }

    [Fact]
    public void Insert_UserIsInserted()
    {
        var repository = database.GetUserRepository();
        var newUserId = Guid.NewGuid();
        
        var newUser = new UserEntity
        {
            Id = newUserId,
            Name = "New Test User"
        };

        repository.Insert(newUser);
        var result = database.GetUserDirectly(newUserId);

        Assert.NotNull(result);
        Assert.Equal(newUser, result);
        Assert.Equal(newUser.Name, result.Name);
    }

    [Fact]
    public void Update_ExistingUser_UserIsUpdated()
    {
        var repository = database.GetUserRepository();
        var userId = database.UserGuids[0];
        
        var updatedUser = new UserEntity
        {
            Id = userId,
            Name = "Updated User Name"
        };

        repository.Update(updatedUser);

        var result = database.GetUserDirectly(userId);
        
        Assert.NotNull(result);
        Assert.Equal("Updated User Name", result.Name);
    }

    [Fact]
    public void Update_NonExistingUser_ReturnsNull()
    {
        var repository = database.GetUserRepository();
        var nonExistingUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = "Non Existing User"
        };

        var result = repository.Update(nonExistingUser);

        Assert.Null(result);
    }

    [Fact]
    public void Delete_ExistingUser_UserIsDeleted()
    {
        var repository = database.GetUserRepository();
        var userId = database.UserGuids[0];

        repository.Remove(userId);

        var result = database.GetUserDirectly(userId);
        Assert.Null(result);
    }

    [Fact]
    public void Exists_ExistingUser_ReturnsTrue()
    {
        var repository = database.GetUserRepository();
        var userId = database.UserGuids[0];

        var result = repository.Exists(userId);

        Assert.True(result);
    }

    [Fact]
    public void Exists_NonExistingUser_ReturnsFalse()
    {
        var repository = database.GetUserRepository();
        var nonExistingId = Guid.NewGuid();

        var result = repository.Exists(nonExistingId);

        Assert.False(result);
    }
}
