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
    public void GetAll_ReturnsMappedCards()
    {
        var repositoryMock = new Mock<ICardRepository>();
        var mapper = new CardMapper();

        var entities = new List<CardEntity>
        {
            new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.text,
                Question = "Question 1",
                AnswerType = QAType.text,
                Answer = "Answer 1",
                CollectionId = Guid.NewGuid(),
                CreatorId = Guid.NewGuid(),
                Description = null
            },
            new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.url,
                Question = "Question 2",
                AnswerType = QAType.url,
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
        
        Assert.Equal(QAType.text, result[0].QuestionType);
        Assert.Equal(QAType.url, result[1].QuestionType);
        
        Assert.Equal(QAType.text, result[0].AnswerType);
        Assert.Equal(QAType.url, result[1].AnswerType);
        
        repositoryMock.Verify(r => r.GetAll(null, null, 1, 10), Times.Once);
    }
    
    [Fact]
    public void GetAll_EmptyRepositoryReturnsEmptyList()
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
   public void GetById_ReturnsMappedCard()
   {
       var repositoryMock = new Mock<ICardRepository>();
       var mapper = new CardMapper();

       var entity = new CardEntity
       {
           Id = Guid.NewGuid(),
           QuestionType = QAType.text,
           Question = "Sample Question",
           AnswerType = QAType.text,
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
   public void GetById_EmptyRepositoryReturnsNull()
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
   public void Search_ReturnsMappedCards()
   {
       var repositoryMock = new Mock<ICardRepository>();
       var mapper = new CardMapper();

       var entities = new List<CardEntity>
       {
           new CardEntity
           {
               Id = Guid.NewGuid(),
               QuestionType = QAType.text,
               Question = "Question 1",
               AnswerType = QAType.text,
               Answer = "Answer 1",
               CollectionId = Guid.NewGuid(),
               CreatorId = Guid.NewGuid(),
               Description = null
           },
           
           new CardEntity
           {
               Id = Guid.NewGuid(),
               QuestionType = QAType.url,
               Question = "Question 2",
               AnswerType = QAType.url,
               Answer = "answer.url",
               CollectionId = Guid.NewGuid(),
               CreatorId = Guid.NewGuid(),
               Description = null
           },
           
           new CardEntity
           {
               Id = Guid.NewGuid(),
               QuestionType = QAType.url,
               Question = "Question 3",
               AnswerType = QAType.text,
               Answer = "Answer 3",
               CollectionId = Guid.NewGuid(),
               CreatorId = Guid.NewGuid(),
               Description = null
           }
       };
        
       repositoryMock.Setup(r => r.GetAll(null, null, 1, 10)).Returns(entities);

       var facade = new CardFacade(repositoryMock.Object, mapper);
       var searchResult = facade.Search(" ");


       Assert.Single(searchResult);
       Assert.Equal(entities[2].Id, searchResult[0].Id);
    
       Assert.DoesNotContain(searchResult, card => card.Id == entities[0].Id);
       Assert.DoesNotContain(searchResult, card => card.Id == entities[1].Id);
        
       repositoryMock.Verify(r => r.GetAll(null, null, 1, 10), Times.Once);
   }
}
