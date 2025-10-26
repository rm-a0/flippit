using Xunit;
using Moq;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Api.BL.Facades;
using Flippit.Common.Models.User;

namespace Flippit.Api.BL.UnitTests;

public class UserFacadeTests
{
    private static UserFacade GetFacadeWithForbiddenDependencyCalls()
    {
        var repository = new Mock<IUserRepository>(MockBehavior.Strict).Object;
        var mapper = new Mock<UserMapper>(MockBehavior.Strict).Object;
        return new UserFacade(repository, mapper);
    }
    
    [Fact]
    public void GetAll_InvalidPageNumber_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentException>(() => facade.GetAll(page: 0));
        
        Assert.Equal("page", exception.ParamName);
        Assert.Contains("Page number must be greater than or equal to 1.", exception.Message);
    }
    
    [Fact]
    public void GetAll_InvalidPageSize_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentException>(() => facade.GetAll(pageSize: 0));
        
        Assert.Equal("pageSize", exception.ParamName);
        Assert.Contains("Page size must be greater than or equal to 1.", exception.Message);
    }
    
    [Fact]
    public void GetAll_RepositoryThrowsException_ExceptionPropagated()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        Assert.Throws<MockException>(() => facade.GetAll());
    }
    
    [Fact]
    public void GetAll_CallsRepository()
    {
        var repositoryMock = new Mock<IUserRepository>();
        var mapper = new UserMapper();

        var entities = new List<UserEntity>
        {
            new UserEntity
            {
                Id = Guid.NewGuid(),
                Name = "User 1"
            },
            new UserEntity
            {
                Id = Guid.NewGuid(),
                Name = "User 2"
            }
        };
        
        repositoryMock.Setup(r => r.GetAll(null, null, 1, 10)).Returns(entities);

        var facade = new UserFacade(repositoryMock.Object, mapper);
        var result = facade.GetAll();

        Assert.Equal(2, result.Count);
        Assert.Equal(entities[0].Id, result[0].Id);
        Assert.Equal(entities[1].Id, result[1].Id);
        
        repositoryMock.Verify(r => r.GetAll(null, null, 1, 10), Times.Once);
    }
    
    [Fact]
    public void GetAll_Empty_CallsRepository()
    {
        var repositoryMock = new Mock<IUserRepository>();
        var mapper = new UserMapper();

        repositoryMock.Setup(r => r.GetAll(null, null, 1, 10)).Returns(new List<UserEntity>());

        var facade = new UserFacade(repositoryMock.Object, mapper);
        var result = facade.GetAll();

        Assert.Empty(result);
        
        repositoryMock.Verify(r => r.GetAll(null, null, 1, 10), Times.Once);
    }
    
    [Fact]
    public void GetById_CallsRepository()
    {
        var repositoryMock = new Mock<IUserRepository>();
        var mapper = new UserMapper();

        var entity = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test User"
        };
        
        repositoryMock.Setup(r => r.GetById(entity.Id)).Returns(entity);

        var facade = new UserFacade(repositoryMock.Object, mapper);
        var result = facade.GetById(entity.Id);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        
        repositoryMock.Verify(r => r.GetById(entity.Id), Times.Once);
    }

    [Fact]
    public void GetById_Empty_CallsRepository()
    {
        var repositoryMock = new Mock<IUserRepository>();
        var mapper = new UserMapper();
        
        var nonExistentId = Guid.NewGuid();
        
        repositoryMock.Setup(r => r.GetById(nonExistentId)).Returns((UserEntity?)null);
        
        var facade = new UserFacade(repositoryMock.Object, mapper);
        var result = facade.GetById(nonExistentId);
        
        Assert.Null(result);
        
        repositoryMock.Verify(r => r.GetById(nonExistentId), Times.Once);
    }
   
    [Fact]
    public void Search_CallsRepositorySearch()
    {
        var repositoryMock = new Mock<IUserRepository>();
        var mapper = new UserMapper();

        var entities = new List<UserEntity>
        {
            new UserEntity
            {
                Id = Guid.NewGuid(),
                Name = "John Doe"
            }
        };

        repositoryMock.Setup(r => r.Search("John")).Returns(new List<UserEntity> { entities[0] });

        var facade = new UserFacade(repositoryMock.Object, mapper);
        var searchResult = facade.Search("John");

        Assert.Single(searchResult);
        Assert.Equal(entities[0].Id, searchResult[0].Id);

        repositoryMock.Verify(r => r.Search("John"), Times.Once());
    }
    
    [Fact]
    public void CreateOrUpdate_NullUserModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.CreateOrUpdate(null!));
        
        Assert.Equal("userModel", exception.ParamName);
    }
    
    [Fact]
    public void CreateOrUpdate_EmptyName_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var userModel = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            Name = ""
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.CreateOrUpdate(userModel));
        
        Assert.Equal("Name", exception.ParamName);
        Assert.Contains("User name cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void CreateOrUpdate_WhitespaceName_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var userModel = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "   "
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.CreateOrUpdate(userModel));
        
        Assert.Equal("Name", exception.ParamName);
        Assert.Contains("User name cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void CreateOrUpdate_NonExistentUser_CallsInsert()
    {
        var repositoryMock = new Mock<IUserRepository>();
        var mapper = new UserMapper();
        var userId = Guid.NewGuid();
        
        var userModel = new UserDetailModel
        {
            Id = userId,
            Name = "New User"
        };

        var entity = mapper.ModelToEntity(userModel);
        
        repositoryMock.Setup(r => r.Exists(userId)).Returns(false);
        repositoryMock.Setup(r => r.Insert(entity)).Returns(userId);
        
        var facade = new UserFacade(repositoryMock.Object, mapper);
        var result = facade.CreateOrUpdate(userModel);
        
        Assert.Equal(userId, result);
        repositoryMock.Verify(r => r.Exists(userId), Times.Once);
        repositoryMock.Verify(r => r.Insert(entity), Times.Once);
        repositoryMock.Verify(r => r.Update(entity), Times.Never);
    }

    [Fact]
    public void CreateOrUpdate_ExistentUser_CallsUpdate()
    {
        var repositoryMock = new Mock<IUserRepository>();
        var mapper = new UserMapper();
        var userId = Guid.NewGuid();
    
        var updatedUserModel = new UserDetailModel
        {
            Id = userId,
            Name = "Updated User"
        };
    
        var updatedEntity = mapper.ModelToEntity(updatedUserModel);
    
        repositoryMock.Setup(r => r.Exists(userId)).Returns(true);
        repositoryMock.Setup(r => r.Update(updatedEntity)).Returns(userId);
    
        var facade = new UserFacade(repositoryMock.Object, mapper);
        var result = facade.CreateOrUpdate(updatedUserModel);
    
        Assert.Equal(userId, result);
        repositoryMock.Verify(r => r.Exists(userId), Times.Once);
        repositoryMock.Verify(r => r.Update(updatedEntity), Times.Once);
        repositoryMock.Verify(r => r.Insert(updatedEntity), Times.Never);
    }
    
    [Fact]
    public void Create_NullUserModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.Create(null!));
        
        Assert.Equal("userModel", exception.ParamName);
    }
    
    [Fact]
    public void Create_EmptyName_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var userModel = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            Name = ""
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.Create(userModel));
        
        Assert.Equal("Name", exception.ParamName);
        Assert.Contains("User name cannot be empty.", exception.Message);
    }

    [Fact]
    public void Create_CallsInsert()
    {
        var repositoryMock = new Mock<IUserRepository>();
        var mapper = new UserMapper();
        var userId = Guid.NewGuid();
    
        var userModel = new UserDetailModel
        {
            Id = userId,
            Name = "New User"
        };
    
        var entity = mapper.ModelToEntity(userModel);
    
        repositoryMock.Setup(r => r.Insert(entity)).Returns(userId);
    
        var facade = new UserFacade(repositoryMock.Object, mapper);
        var result = facade.Create(userModel);
    
        Assert.Equal(userId, result);
        repositoryMock.Verify(r => r.Insert(entity), Times.Once);
    }
    
    [Fact]
    public void Update_NullUserModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.Update(null!));
        
        Assert.Equal("userModel", exception.ParamName);
    }
    
    [Fact]
    public void Update_EmptyName_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var userModel = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            Name = ""
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.Update(userModel));
        
        Assert.Equal("Name", exception.ParamName);
        Assert.Contains("User name cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void Update_CallsUpdate()
    {
        var repositoryMock = new Mock<IUserRepository>();
        var mapper = new UserMapper();
        var userId = Guid.NewGuid();
    
        var updatedUserModel = new UserDetailModel
        {
            Id = userId,
            Name = "Updated User"
        };
    
        var updatedEntity = mapper.ModelToEntity(updatedUserModel);
        
        repositoryMock.Setup(r => r.Update(updatedEntity)).Returns(userId);
    
        var facade = new UserFacade(repositoryMock.Object, mapper);
        var result = facade.Update(updatedUserModel);
    
        Assert.NotNull(result);
        Assert.Equal(userId, result);
    
        repositoryMock.Verify(r => r.Update(updatedEntity), Times.Once);
    }
    
    [Fact]
    public void Delete_CallsRemove()
    {
        var repositoryMock = new Mock<IUserRepository>();
        var mapper = new UserMapper();
        var userId = Guid.NewGuid();
    
        repositoryMock.Setup(r => r.Remove(userId));
    
        var facade = new UserFacade(repositoryMock.Object, mapper);
        facade.Delete(userId);
    
        repositoryMock.Verify(r => r.Remove(userId), Times.Once);
    }
}
