using Xunit;
using Moq;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Common.Enums;
using Flippit.Common.Models;
using Flippit.Api.BL.Facades;
using Flippit.Common.Models.Card;

namespace Flippit.Api.BL.UnitTests;

public class CardFacadeTests
{
    private static CardFacade GetFacadeWithForbiddenDependencyCalls()
    {
        var repository = new Mock<ICardRepository>(MockBehavior.Strict).Object;
        var mapper = new Mock<CardMapper>(MockBehavior.Strict).Object;
        return new CardFacade(repository, mapper);
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
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();

        var entities = new List<CardEntity>
        {
            new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                Question = "Question 1",
                AnswerType = QAType.Text,
                Answer = "Answer 1",
                CollectionId = Guid.NewGuid(),
                CreatorId = Guid.NewGuid(),
                Description = null
            },
            new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Url,
                Question = "Question 2",
                AnswerType = QAType.Url,
                Answer = "answer.url",
                CollectionId = Guid.NewGuid(),
                CreatorId = Guid.NewGuid(),
                Description = null
            }
        };
        
        repositoryMock.Setup(r => r.GetAll(null, null, 1, 10)).Returns(entities);

        var facade = new CardFacade(repositoryMock.Object, mapper);
        var result = facade.GetAll();

        Assert.Equal(2, result.Count);
        
        Assert.Equal(entities[0].Id, result[0].Id);
        Assert.Equal(entities[1].Id, result[1].Id);
        
        repositoryMock.Verify(r => r.GetAll(null, null, 1, 10), Times.Once);
    }
    
    [Fact]
    public void GetAll_Empty_CallsRepository()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();

        repositoryMock.Setup(r => r.GetAll(null, null, 1, 10)).Returns(new List<CardEntity>());

        var facade = new CardFacade(repositoryMock.Object, mapper);
        var result = facade.GetAll();

        Assert.Empty(result);
        
        repositoryMock.Verify(r => r.GetAll(null, null, 1, 10), Times.Once);
    }
    
    [Fact]
    public void GetById_CallsRepository()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();

        var entity = new CardEntity
        {
            Id = Guid.NewGuid(),
            QuestionType = QAType.Text,
            Question = "Sample Question",
            AnswerType = QAType.Text,
            Answer = "Sample Answer",
            CollectionId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid(),
            Description = null
        };
        
        repositoryMock.Setup(r => r.GetById(entity.Id)).Returns(entity);

        var facade = new CardFacade(repositoryMock.Object, mapper);
        var result = facade.GetById(entity.Id);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        
        Assert.Equal(entity.Question, result.Question);
        Assert.Equal(entity.Answer, result.Answer);
        
        repositoryMock.Verify(r => r.GetById(entity.Id), Times.Once);
    }

    [Fact]
    public void GetById_Empty_CallsRepository()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();
        
        var nonExistentId = Guid.NewGuid();
        
        repositoryMock.Setup(r => r.GetById(nonExistentId)).Returns((CardEntity?)null);
        
        var facade = new CardFacade(repositoryMock.Object, mapper);
        var result = facade.GetById(nonExistentId);
        
        Assert.Null(result);
        
        repositoryMock.Verify(r => r.GetById(nonExistentId), Times.Once);
    }
   
    [Fact]
    public void Search_CallsRepositorySearch()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();

        var entities = new List<CardEntity>
        {
            new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                Question = "Question 1",
                AnswerType = QAType.Text,
                Answer = "Answer 1",
                CollectionId = Guid.NewGuid(),
                CreatorId = Guid.NewGuid(),
                Description = null
            }
        };

        repositoryMock.Setup(r => r.Search("Question 1")).Returns(new List<CardEntity> { entities[0] });

        var facade = new CardFacade(repositoryMock.Object, mapper);
        var searchResult = facade.Search("Question 1");

        Assert.Single(searchResult);
        Assert.Equal(entities[0].Id, searchResult[0].Id);

        repositoryMock.Verify(r => r.Search("Question 1"), Times.Once());
    }

    [Fact]
    public void SearchByCreatorId_CallsRepositorySearch()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();
        var creatorId = Guid.NewGuid();

        var entities = new List<CardEntity>
        {
            new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                Question = "Question 1",
                AnswerType = QAType.Text,
                Answer = "Answer 1",
                CollectionId = Guid.NewGuid(),
                CreatorId = creatorId,
                Description = null
            },

            new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Url,
                Question = "Question 2",
                AnswerType = QAType.Url,
                Answer = "answer.url",
                CollectionId = Guid.NewGuid(),
                CreatorId = creatorId,
                Description = null
            }
        };

        repositoryMock.Setup(r => r.SearchByCreatorId(creatorId, null, null, 1, 10)).Returns(new List<CardEntity> { entities[0], entities[1] });

        var facade = new CardFacade(repositoryMock.Object, mapper);
        var searchResult = facade.SearchByCreatorId(creatorId);

        Assert.Equal(2, searchResult.Count);
        Assert.Equal(creatorId, searchResult[0].CreatorId);
        Assert.Equal(creatorId, searchResult[1].CreatorId);

        repositoryMock.Verify(r => r.SearchByCreatorId(creatorId, null, null, 1, 10), Times.Once());       
    }

    [Fact]
    public void SearchByCollectionId_CallsRepositorySearch()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();
        var collectionId = Guid.NewGuid();

        var entities = new List<CardEntity>
        {
            new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                Question = "Question 1",
                AnswerType = QAType.Text,
                Answer = "Answer 1",
                CollectionId = collectionId,
                CreatorId = Guid.NewGuid(),
                Description = null
            },

            new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Url,
                Question = "Question 2",
                AnswerType = QAType.Url,
                Answer = "answer.url",
                CollectionId = collectionId,
                CreatorId = Guid.NewGuid(),
                Description = null
            }
        };

        repositoryMock.Setup(r => r.SearchByCollectionId(collectionId, null, null, 1, 10)).Returns(new List<CardEntity> { entities[0], entities[1] });

        var facade = new CardFacade(repositoryMock.Object, mapper);
        var searchResult = facade.SearchByCollectionId(collectionId);

        Assert.Equal(2, searchResult.Count);
        Assert.Equal(collectionId, searchResult[0].CollectionId);
        Assert.Equal(collectionId, searchResult[1].CollectionId);

        repositoryMock.Verify(r => r.SearchByCollectionId(collectionId, null, null, 1, 10), Times.Once());       
    }
    
    [Fact]
    public void CreateOrUpdate_NullCardModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.CreateOrUpdate(null!));
        
        Assert.Equal("cardModel", exception.ParamName);
    }
    
    [Fact]
    public void CreateOrUpdate_EmptyQuestion_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var cardModel = new CardDetailModel
        {
            Id = Guid.NewGuid(),
            QuestionType = QAType.Text,
            Question = "",
            AnswerType = QAType.Text,
            Answer = "Answer",
            CollectionId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid()
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.CreateOrUpdate(cardModel));
        
        Assert.Equal("Question", exception.ParamName);
        Assert.Contains("Card question cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void CreateOrUpdate_WhitespaceQuestion_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var cardModel = new CardDetailModel
        {
            Id = Guid.NewGuid(),
            QuestionType = QAType.Text,
            Question = "   ",
            AnswerType = QAType.Text,
            Answer = "Answer",
            CollectionId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid()
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.CreateOrUpdate(cardModel));
        
        Assert.Equal("Question", exception.ParamName);
        Assert.Contains("Card question cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void CreateOrUpdate_NonExistentCard_CallsInsert()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();
        var cardId = Guid.NewGuid();
        
        var cardModel = new CardDetailModel
        {
            Id = cardId,
            QuestionType = QAType.Text,
            Question = "New Question",
            AnswerType = QAType.Text,
            Answer = "Answer",
            CollectionId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid(),
            Description = null
        };

        var entity = mapper.ModelToEntity(cardModel);
        
        repositoryMock.Setup(r => r.Exists(cardId)).Returns(false);
        repositoryMock.Setup(r => r.Insert(entity)).Returns(cardId);
        
        var facade = new CardFacade(repositoryMock.Object, mapper);
        var result = facade.CreateOrUpdate(cardModel);
        
        Assert.Equal(cardId, result);
        repositoryMock.Verify(r => r.Exists(cardId), Times.Once);
        repositoryMock.Verify(r => r.Insert(entity), Times.Once);
        repositoryMock.Verify(r => r.Update(entity), Times.Never);
    }

    [Fact]
    public void CreateOrUpdate_ExistentCard_CallsUpdate()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();
        var cardId = Guid.NewGuid();
    
        var updatedCardModel = new CardDetailModel
        {
            Id = cardId,
            QuestionType = QAType.Text,
            Question = "Updated Question",
            AnswerType = QAType.Text,
            Answer = "Updated Answer",
            CollectionId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid(),
            Description = "Updated Description"
        };
    
        var updatedEntity = mapper.ModelToEntity(updatedCardModel);
    
        repositoryMock.Setup(r => r.Exists(cardId)).Returns(true);
        repositoryMock.Setup(r => r.Update(updatedEntity)).Returns(cardId);
    
        var facade = new CardFacade(repositoryMock.Object, mapper);
        var result = facade.CreateOrUpdate(updatedCardModel);
    
        Assert.Equal(cardId, result);
        repositoryMock.Verify(r => r.Exists(cardId), Times.Once);
        repositoryMock.Verify(r => r.Update(updatedEntity), Times.Once);
        repositoryMock.Verify(r => r.Insert(updatedEntity), Times.Never);
    }
    
    [Fact]
    public void Create_NullCardModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.Create(null!));
        
        Assert.Equal("cardModel", exception.ParamName);
    }
    
    [Fact]
    public void Create_EmptyQuestion_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var cardModel = new CardDetailModel
        {
            Id = Guid.NewGuid(),
            QuestionType = QAType.Text,
            Question = "",
            AnswerType = QAType.Text,
            Answer = "Answer",
            CollectionId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid()
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.Create(cardModel));
        
        Assert.Equal("Question", exception.ParamName);
        Assert.Contains("Card question cannot be empty.", exception.Message);
    }

    [Fact]
    public void Create_CallsInsert()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();
        var cardId = Guid.NewGuid();
    
        var cardModel = new CardDetailModel
        {
            Id = cardId,
            QuestionType = QAType.Text,
            Question = "Question",
            AnswerType = QAType.Text,
            Answer = "Answer",
            CollectionId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid(),
            Description = null
        };
    
        var entity = mapper.ModelToEntity(cardModel);
    
        repositoryMock.Setup(r => r.Insert(entity)).Returns(cardId);
    
        var facade = new CardFacade(repositoryMock.Object, mapper);
        var result = facade.Create(cardModel);
    
        Assert.Equal(cardId, result);
        repositoryMock.Verify(r => r.Insert(entity), Times.Once);
    }
    
    [Fact]
    public void Update_NullCardModel_ThrowsArgumentNullException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var exception = Assert.Throws<ArgumentNullException>(() => facade.Update(null!));
        
        Assert.Equal("cardModel", exception.ParamName);
    }
    
    [Fact]
    public void Update_EmptyQuestion_ThrowsArgumentException()
    {
        var facade = GetFacadeWithForbiddenDependencyCalls();
        
        var cardModel = new CardDetailModel
        {
            Id = Guid.NewGuid(),
            QuestionType = QAType.Text,
            Question = "",
            AnswerType = QAType.Text,
            Answer = "Answer",
            CollectionId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid()
        };
        
        var exception = Assert.Throws<ArgumentException>(() => facade.Update(cardModel));
        
        Assert.Equal("Question", exception.ParamName);
        Assert.Contains("Card question cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void Update_CallsUpdate()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();
        var cardId = Guid.NewGuid();
    
        var updatedCardModel = new CardDetailModel
        {
            Id = cardId,
            QuestionType = QAType.Text,
            Question = "Updated Question",
            AnswerType = QAType.Text,
            Answer = "Updated Answer",
            CollectionId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid(),
            Description = "Updated Description"
        };
    
        var updatedEntity = mapper.ModelToEntity(updatedCardModel);
        
        repositoryMock.Setup(r => r.Update(updatedEntity)).Returns(cardId);
    
        var facade = new CardFacade(repositoryMock.Object, mapper);
        var result = facade.Update(updatedCardModel);
        
    
        Assert.NotNull(result);
        Assert.Equal(cardId, result);
    
        repositoryMock.Verify(r => r.Update(updatedEntity), Times.Once);
    }
    
    [Fact]
    public void Delete_CallsRemove()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();
        var cardId = Guid.NewGuid();
    
        repositoryMock.Setup(r => r.Remove(cardId));
    
        var facade = new CardFacade(repositoryMock.Object, mapper);
        facade.Delete(cardId);
    
        repositoryMock.Verify(r => r.Remove(cardId), Times.Once);
    }
}
