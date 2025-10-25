using Xunit;
using Moq;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Api.BL.Facades;
using Flippit.Common.Models.Collection;

namespace Flippit.Api.BL.UnitTests;

public class CollectionFacadeTests
{
    private static CollectionFacade GetFacadeWithForbiddenDependencyCalls()
    {
        var repository = new Mock<ICollectionRepository>(MockBehavior.Strict).Object;
        var mapper = new Mock<CollectionMapper>(MockBehavior.Strict).Object;
        return new CollectionFacade(repository, mapper);
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
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();

        var entities = new List<CollectionEntity>
        {
            new CollectionEntity
            {
                Id = Guid.NewGuid(),
                Name = "Collection 1",
                CreatorId = Guid.NewGuid(),
                EndTime = DateTime.Now,
                StartTime = DateTime.Now
            },
            new CollectionEntity
            {
                Id = Guid.NewGuid(),
                Name = "Collection 2",
                CreatorId = Guid.NewGuid(),
                EndTime = DateTime.Now,
                StartTime = DateTime.Now
            }
        };
        
        repositoryMock.Setup(r => r.GetAll(null, null, 1, 10)).Returns(entities);

        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        var result = facade.GetAll();

        Assert.Equal(2, result.Count);
        Assert.Equal(entities[0].Id, result[0].Id);
        Assert.Equal(entities[1].Id, result[1].Id);
        
        repositoryMock.Verify(r => r.GetAll(null, null, 1, 10), Times.Once);
    }
    
    [Fact]
    public void GetAll_Empty_CallsRepository()
    {
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();

        repositoryMock.Setup(r => r.GetAll(null, null, 1, 10)).Returns(new List<CollectionEntity>());

        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        var result = facade.GetAll();

        Assert.Empty(result);
        
        repositoryMock.Verify(r => r.GetAll(null, null, 1, 10), Times.Once);
    }
    
    [Fact]
    public void GetById_CallsRepository()
    {
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();

        var entity = new CollectionEntity
        {
            Id = Guid.NewGuid(),
            Name = "Sample Collection",
            CreatorId = Guid.NewGuid(),
            EndTime = DateTime.Now,
            StartTime = DateTime.Now
        };
        
        repositoryMock.Setup(r => r.GetById(entity.Id)).Returns(entity);

        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        var result = facade.GetById(entity.Id);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Name, result.Name);
        
        repositoryMock.Verify(r => r.GetById(entity.Id), Times.Once);
    }

    [Fact]
    public void GetById_Empty_CallsRepository()
    {
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();
        
        var nonExistentId = Guid.NewGuid();
        
        repositoryMock.Setup(r => r.GetById(nonExistentId)).Returns((CollectionEntity?)null);
        
        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        var result = facade.GetById(nonExistentId);
        
        Assert.Null(result);
        
        repositoryMock.Verify(r => r.GetById(nonExistentId), Times.Once);
    }
   
    [Fact]
    public void Search_CallsRepositorySearch()
    {
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();

        var entities = new List<CollectionEntity>
        {
            new CollectionEntity
            {
                Id = Guid.NewGuid(),
                Name = "Collection 1",
                CreatorId = Guid.NewGuid(),
                EndTime = DateTime.Now,
                StartTime = DateTime.Now
            }
        };

        repositoryMock.Setup(r => r.Search("Collection 1")).Returns(new List<CollectionEntity> { entities[0] });

        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        var searchResult = facade.Search("Collection 1");

        Assert.Single(searchResult);
        Assert.Equal(entities[0].Id, searchResult[0].Id);

        repositoryMock.Verify(r => r.Search("Collection 1"), Times.Once());
    }

    [Fact]
    public void SearchByCreatorId_CallsRepositorySearch()
    {
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();
        var creatorId = Guid.NewGuid();

        var entities = new List<CollectionEntity>
        {
            new CollectionEntity
            {
                Id = Guid.NewGuid(),
                Name = "Collection 1",
                CreatorId = creatorId,
                EndTime = DateTime.Now,
                StartTime = DateTime.Now
            },
            
            new CollectionEntity
            {
                Id = Guid.NewGuid(),
                Name = "Collection 2",
                CreatorId = creatorId,
                EndTime = DateTime.Now,
                StartTime = DateTime.Now
            }
        };

        repositoryMock.Setup(r => r.SearchByCreatorId(creatorId)).Returns(new List<CollectionEntity> { entities[0], entities[1] });

        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        var searchResult = facade.SearchByCreatorId(creatorId);

        Assert.Equal(2, searchResult.Count);
        Assert.Equal(entities[0].Id, searchResult[0].Id);
        Assert.Equal(entities[1].Id, searchResult[1].Id);

        repositoryMock.Verify(r => r.SearchByCreatorId(creatorId), Times.Once());       
    }
    
    [Fact]
    public void CreateOrUpdate_NullCollectionModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.CreateOrUpdate(null!));
        
        Assert.Equal("collectionModel", exception.ParamName);
    }
    
    [Fact]
    public void CreateOrUpdate_EmptyName_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var collectionModel = new CollectionDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "",
            CreatorId = Guid.NewGuid(),
            EndTime = DateTime.Now,
            StartTime = DateTime.Now
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.CreateOrUpdate(collectionModel));
        
        Assert.Equal("Name", exception.ParamName);
        Assert.Contains("Collection name cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void CreateOrUpdate_WhitespaceName_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var collectionModel = new CollectionDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "   ",
            CreatorId = Guid.NewGuid(),
            EndTime = DateTime.Now,
            StartTime = DateTime.Now
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.CreateOrUpdate(collectionModel));
        
        Assert.Equal("Name", exception.ParamName);
        Assert.Contains("Collection name cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void CreateOrUpdate_NonExistentCollection_CallsInsert()
    {
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();
        var collectionId = Guid.NewGuid();
        
        var collectionModel = new CollectionDetailModel
        {
            Id = collectionId,
            Name = "New Collection",
            CreatorId = Guid.NewGuid(),
            EndTime = DateTime.Now,
            StartTime = DateTime.Now
        };

        var entity = mapper.ModelToEntity(collectionModel);
        
        repositoryMock.Setup(r => r.Exists(collectionId)).Returns(false);
        repositoryMock.Setup(r => r.Insert(entity)).Returns(collectionId);
        
        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        var result = facade.CreateOrUpdate(collectionModel);
        
        Assert.Equal(collectionId, result);
        repositoryMock.Verify(r => r.Exists(collectionId), Times.Once);
        repositoryMock.Verify(r => r.Insert(entity), Times.Once);
        repositoryMock.Verify(r => r.Update(entity), Times.Never);
    }

    [Fact]
    public void CreateOrUpdate_ExistentCollection_CallsUpdate()
    {
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();
        var collectionId = Guid.NewGuid();
    
        var updatedCollectionModel = new CollectionDetailModel
        {
            Id = collectionId,
            Name = "Updated Collection",
            CreatorId = Guid.NewGuid(),
            EndTime = DateTime.Now,
            StartTime = DateTime.Now
        };
    
        var updatedEntity = mapper.ModelToEntity(updatedCollectionModel);
    
        repositoryMock.Setup(r => r.Exists(collectionId)).Returns(true);
        repositoryMock.Setup(r => r.Update(updatedEntity)).Returns(collectionId);
    
        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        var result = facade.CreateOrUpdate(updatedCollectionModel);
    
        Assert.Equal(collectionId, result);
        repositoryMock.Verify(r => r.Exists(collectionId), Times.Once);
        repositoryMock.Verify(r => r.Update(updatedEntity), Times.Once);
        repositoryMock.Verify(r => r.Insert(updatedEntity), Times.Never);
    }
    
    [Fact]
    public void Create_NullCollectionModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.Create(null!));
        
        Assert.Equal("collectionModel", exception.ParamName);
    }
    
    [Fact]
    public void Create_EmptyName_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var collectionModel = new CollectionDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "",
            CreatorId = Guid.NewGuid(),
            EndTime = DateTime.Now,
            StartTime = DateTime.Now
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.Create(collectionModel));
        
        Assert.Equal("Name", exception.ParamName);
        Assert.Contains("Collection name cannot be empty.", exception.Message);
    }

    [Fact]
    public void Create_CallsInsert()
    {
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();
        var collectionId = Guid.NewGuid();
    
        var collectionModel = new CollectionDetailModel
        {
            Id = collectionId,
            Name = "Collection",
            CreatorId = Guid.NewGuid(),
            EndTime = DateTime.Now,
            StartTime = DateTime.Now
        };
    
        var entity = mapper.ModelToEntity(collectionModel);
    
        repositoryMock.Setup(r => r.Insert(entity)).Returns(collectionId);
    
        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        var result = facade.Create(collectionModel);
    
        Assert.Equal(collectionId, result);
        repositoryMock.Verify(r => r.Insert(entity), Times.Once);
    }
    
    [Fact]
    public void Update_NullCollectionModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.Update(null!));
        
        Assert.Equal("collectionModel", exception.ParamName);
    }
    
    [Fact]
    public void Update_EmptyName_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var collectionModel = new CollectionDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "",
            CreatorId = Guid.NewGuid(),
            EndTime = DateTime.Now,
            StartTime = DateTime.Now
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.Update(collectionModel));
        
        Assert.Equal("Name", exception.ParamName);
        Assert.Contains("Collection name cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void Update_CallsUpdate()
    {
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();
        var collectionId = Guid.NewGuid();
    
        var updatedCollectionModel = new CollectionDetailModel
        {
            Id = collectionId,
            Name = "Updated Collection",
            CreatorId = Guid.NewGuid(),
            EndTime = DateTime.Now,
            StartTime = DateTime.Now
        };
    
        var updatedEntity = mapper.ModelToEntity(updatedCollectionModel);
        
        repositoryMock.Setup(r => r.Update(updatedEntity)).Returns(collectionId);
    
        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        var result = facade.Update(updatedCollectionModel);
    
        Assert.NotNull(result);
        Assert.Equal(collectionId, result);
    
        repositoryMock.Verify(r => r.Update(updatedEntity), Times.Once);
    }
    
    [Fact]
    public void Delete_CallsRemove()
    {
        var repositoryMock = new Mock<ICollectionRepository>();
        var mapper = new CollectionMapper();
        var collectionId = Guid.NewGuid();
    
        repositoryMock.Setup(r => r.Remove(collectionId));
    
        var facade = new CollectionFacade(repositoryMock.Object, mapper);
        facade.Delete(collectionId);
    
        repositoryMock.Verify(r => r.Remove(collectionId), Times.Once);
    }
}
