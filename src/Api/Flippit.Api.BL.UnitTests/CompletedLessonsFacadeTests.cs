using Xunit;
using Moq;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Api.BL.Facades;
using Flippit.Common.Models.CompletedLesson;

namespace Flippit.Api.BL.UnitTests;

public class CompletedLessonFacadeTests
{
    private static CompletedLessonFacade GetFacadeWithForbiddenDependencyCalls()
    {
        var repository = new Mock<ICompletedLessonRepository>(MockBehavior.Strict).Object;
        var mapper = new Mock<CompletedLessonMapper>(MockBehavior.Strict).Object;
        return new CompletedLessonFacade(repository, mapper);
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
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();

        var entities = new List<CompletedLessonEntity>
        {
            new CompletedLessonEntity
            {
                Id = Guid.NewGuid(),
                AnswersJson = "Answers",
                StatisticsJson = "Statistics",
                UserId = Guid.NewGuid(),
                CollectionId = Guid.NewGuid(),
            },
            new CompletedLessonEntity
            {
                Id = Guid.NewGuid(),
                AnswersJson = "Answers",
                StatisticsJson = "Statistics",
                UserId = Guid.NewGuid(),
                CollectionId = Guid.NewGuid(),
            }
        };
        
        repositoryMock.Setup(r => r.GetAll(null, null, 1, 10)).Returns(entities);

        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var result = facade.GetAll();

        Assert.Equal(2, result.Count);
        Assert.Equal(entities[0].Id, result[0].Id);
        Assert.Equal(entities[1].Id, result[1].Id);
        
        repositoryMock.Verify(r => r.GetAll(null, null, 1, 10), Times.Once);
    }
    
    [Fact]
    public void GetAll_Empty_CallsRepository()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();

        repositoryMock.Setup(r => r.GetAll(null, null, 1, 10)).Returns(new List<CompletedLessonEntity>());

        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var result = facade.GetAll();

        Assert.Empty(result);
        
        repositoryMock.Verify(r => r.GetAll(null, null, 1, 10), Times.Once);
    }
    
    [Fact]
    public void GetById_CallsRepository()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();
        var entityId = Guid.NewGuid();

        var entity = new CompletedLessonEntity
        {
            Id = entityId,
            AnswersJson = "Answers",
            StatisticsJson = "Statistics",
            UserId = Guid.NewGuid(),
            CollectionId = Guid.NewGuid(),
        };
        
        repositoryMock.Setup(r => r.GetById(entityId)).Returns(entity);

        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var result = facade.GetById(entityId);

        Assert.NotNull(result);
        Assert.Equal(entityId, result.Id);
        
        repositoryMock.Verify(r => r.GetById(entityId), Times.Once);
    }

    [Fact]
    public void GetById_Empty_CallsRepository()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();
        
        var nonExistentId = Guid.NewGuid();
        
        repositoryMock.Setup(r => r.GetById(nonExistentId)).Returns((CompletedLessonEntity?)null);
        
        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var result = facade.GetById(nonExistentId);
        
        Assert.Null(result);
        
        repositoryMock.Verify(r => r.GetById(nonExistentId), Times.Once);
    }
   
    [Fact]
    public void Search_CallsRepositorySearch()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();
        var entityId = Guid.NewGuid();

        var entities = new List<CompletedLessonEntity>
        {
            new CompletedLessonEntity
            {
                Id = entityId,
                AnswersJson = "Answers",
                StatisticsJson = "Statistics",
                UserId = Guid.NewGuid(),
                CollectionId = Guid.NewGuid(),
            }
        };

        repositoryMock.Setup(r => r.Search("Answers")).Returns(new List<CompletedLessonEntity> { entities[0] });

        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var searchResult = facade.Search("Answers");

        Assert.Single(searchResult);
        Assert.Equal(entities[0].Id, searchResult[0].Id);

        repositoryMock.Verify(r => r.Search("Answers"), Times.Once());
    }

    [Fact]
    public void SearchByCreatorId_CallsRepositorySearch()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();
        var userId = Guid.NewGuid();

        var entities = new List<CompletedLessonEntity>
        {
            new CompletedLessonEntity
            {
                Id = Guid.NewGuid(),
                AnswersJson = "Answers",
                StatisticsJson = "Statistics",
                UserId = userId,
                CollectionId = Guid.NewGuid(),
            },
            new CompletedLessonEntity
            {
                Id = Guid.NewGuid(),
                AnswersJson = "Answers",
                StatisticsJson = "Statistics",
                UserId = userId,
                CollectionId = Guid.NewGuid()
            }
        };

        repositoryMock.Setup(r => r.SearchByCreatorId(userId, null, 1, 10)).Returns(new List<CompletedLessonEntity> { entities[0], entities[1] });

        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var searchResult = facade.SearchByCreatorId(userId);

        Assert.Equal(2, searchResult.Count);

        repositoryMock.Verify(r => r.SearchByCreatorId(userId, null, 1, 10), Times.Once());       
    }

    [Fact]
    public void SearchByCollectionId_CallsRepositorySearch()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();
        var collectionId = Guid.NewGuid();

        var entities = new List<CompletedLessonEntity>
        {
            new CompletedLessonEntity
            {
                Id = Guid.NewGuid(),
                AnswersJson = "Answers",
                StatisticsJson = "Statistics",
                UserId = Guid.NewGuid(),
                CollectionId = collectionId
            },
            new CompletedLessonEntity
            {
                Id = Guid.NewGuid(),
                AnswersJson = "Answers",
                StatisticsJson = "Statistics",
                UserId = Guid.NewGuid(),
                CollectionId = collectionId
            }
        };

        repositoryMock.Setup(r => r.SearchByCollectionId(collectionId, null, 1, 10)).Returns(new List<CompletedLessonEntity> { entities[0], entities[1] });

        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var searchResult = facade.SearchByCollectionId(collectionId);

        Assert.Equal(2, searchResult.Count);
        Assert.Equal(collectionId, searchResult[0].CollectionId);
        Assert.Equal(collectionId, searchResult[1].CollectionId);

        repositoryMock.Verify(r => r.SearchByCollectionId(collectionId, null, 1, 10), Times.Once());       
    }
    
    [Fact]
    public void CreateOrUpdate_NullLessonModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.CreateOrUpdate(null!));
        
        Assert.Equal("lessonModel", exception.ParamName);
    }
    
    [Fact]
    public void CreateOrUpdate_EmptyUserId_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var lessonModel = new CompletedLessonDetailModel
        {
            Id = Guid.NewGuid(),
            AnswersJson = "Answers",
            StatisticsJson = "Statistics",
            UserId = Guid.Empty,
            CollectionId = Guid.NewGuid(),
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.CreateOrUpdate(lessonModel));
        
        Assert.Equal("UserId", exception.ParamName);
        Assert.Contains("User ID cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void CreateOrUpdate_EmptyCollectionId_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var lessonModel = new CompletedLessonDetailModel
        {
            Id = Guid.NewGuid(),
            AnswersJson = "Answers",
            StatisticsJson = "Statistics",
            UserId = Guid.NewGuid(),
            CollectionId = Guid.Empty,
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.CreateOrUpdate(lessonModel));
        
        Assert.Equal("CollectionId", exception.ParamName);
        Assert.Contains("Collection ID cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void CreateOrUpdate_NonExistentLesson_CallsInsert()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();
        var lessonId = Guid.NewGuid();
        
        var lessonModel = new CompletedLessonDetailModel
        {
            Id = lessonId,
            AnswersJson = "Answers",
            StatisticsJson = "Statistics",
            UserId = Guid.NewGuid(),
            CollectionId = Guid.NewGuid(),
        };

        var entity = mapper.ModelToEntity(lessonModel);
        
        repositoryMock.Setup(r => r.Exists(lessonId)).Returns(false);
        repositoryMock.Setup(r => r.Insert(entity)).Returns(lessonId);
        
        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var result = facade.CreateOrUpdate(lessonModel);
        
        Assert.Equal(lessonId, result);
        repositoryMock.Verify(r => r.Exists(lessonId), Times.Once);
        repositoryMock.Verify(r => r.Insert(entity), Times.Once);
        repositoryMock.Verify(r => r.Update(entity), Times.Never);
    }

    [Fact]
    public void CreateOrUpdate_ExistentLesson_CallsUpdate()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();
        var lessonId = Guid.NewGuid();
    
        var updatedLessonModel = new CompletedLessonDetailModel
        {
            Id = lessonId,
            AnswersJson = "Answers",
            StatisticsJson = "Statistics",
            UserId = Guid.NewGuid(),
            CollectionId = Guid.NewGuid(),
        };
    
        var updatedEntity = mapper.ModelToEntity(updatedLessonModel);
    
        repositoryMock.Setup(r => r.Exists(lessonId)).Returns(true);
        repositoryMock.Setup(r => r.Update(updatedEntity)).Returns(lessonId);
    
        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var result = facade.CreateOrUpdate(updatedLessonModel);
    
        Assert.Equal(lessonId, result);
        repositoryMock.Verify(r => r.Exists(lessonId), Times.Once);
        repositoryMock.Verify(r => r.Update(updatedEntity), Times.Once);
        repositoryMock.Verify(r => r.Insert(updatedEntity), Times.Never);
    }
    
    [Fact]
    public void Create_NullLessonModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.Create(null!));
        
        Assert.Equal("lessonModel", exception.ParamName);
    }
    
    [Fact]
    public void Create_EmptyUserId_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var lessonModel = new CompletedLessonDetailModel
        {
            Id = Guid.NewGuid(),
            AnswersJson = "Answers",
            StatisticsJson = "Statistics",
            UserId = Guid.Empty,
            CollectionId = Guid.NewGuid(),
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.Create(lessonModel));
        
        Assert.Equal("UserId", exception.ParamName);
        Assert.Contains("User ID cannot be empty.", exception.Message);
    }

    [Fact]
    public void Create_CallsInsert()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();
        var lessonId = Guid.NewGuid();
    
        var lessonModel = new CompletedLessonDetailModel
        {
            Id = lessonId,
            AnswersJson = "Answers",
            StatisticsJson = "Statistics",
            UserId = Guid.NewGuid(),
            CollectionId = Guid.NewGuid(),
        };
    
        var entity = mapper.ModelToEntity(lessonModel);
    
        repositoryMock.Setup(r => r.Insert(entity)).Returns(lessonId);
    
        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var result = facade.Create(lessonModel);
    
        Assert.Equal(lessonId, result);
        repositoryMock.Verify(r => r.Insert(entity), Times.Once);
    }
    
    [Fact]
    public void Update_NullLessonModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.Update(null!));
        
        Assert.Equal("lessonModel", exception.ParamName);
    }
    
    [Fact]
    public void Update_EmptyUserId_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var lessonModel = new CompletedLessonDetailModel
        {
            Id = Guid.NewGuid(),
            AnswersJson = "Answers",
            StatisticsJson = "Statistics",
            UserId = Guid.Empty,
            CollectionId = Guid.NewGuid(),
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.Update(lessonModel));
        
        Assert.Equal("UserId", exception.ParamName);
        Assert.Contains("User ID cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void Update_CallsUpdate()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();
        var lessonId = Guid.NewGuid();
    
        var updatedLessonModel = new CompletedLessonDetailModel
        {
            Id = lessonId,
            AnswersJson = "Answers",
            StatisticsJson = "Statistics",
            UserId = Guid.NewGuid(),
            CollectionId = Guid.NewGuid(),
        };
    
        var updatedEntity = mapper.ModelToEntity(updatedLessonModel);
        
        repositoryMock.Setup(r => r.Update(updatedEntity)).Returns(lessonId);
    
        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        var result = facade.Update(updatedLessonModel);
    
        Assert.NotNull(result);
        Assert.Equal(lessonId, result);
    
        repositoryMock.Verify(r => r.Update(updatedEntity), Times.Once);
    }
    
    [Fact]
    public void Delete_CallsRemove()
    {
        var repositoryMock = new Mock<ICompletedLessonRepository>();
        var mapper = new CompletedLessonMapper();
        var lessonId = Guid.NewGuid();
    
        repositoryMock.Setup(r => r.Remove(lessonId));
    
        var facade = new CompletedLessonFacade(repositoryMock.Object, mapper);
        facade.Delete(lessonId);
    
        repositoryMock.Verify(r => r.Remove(lessonId), Times.Once);
    }
}
