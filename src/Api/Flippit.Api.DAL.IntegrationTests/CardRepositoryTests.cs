using Flippit.Common.Enums;
using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.IntegrationTests;

public class CardRepositoryTests : IClassFixture<InMemoryTestDataProvider>
{
    private readonly InMemoryTestDataProvider database;

    public CardRepositoryTests(InMemoryTestDataProvider database)
    {
        this.database = database;
        database.ResetStorage();
    }

    [Fact]
    public void GetById_ExistingCard_ReturnsCard()
    {
        var repository = database.GetCardRepository();
        var cardId = database.CardGuids[0];

        var card = database.GetCardDirectly(cardId);
        var result = repository.GetById(cardId);

        Assert.NotNull(result);
        Assert.Equal(card, result);
        Assert.Equal(cardId, result.Id);
    }
    
    [Fact]
    public void GetById_NonExistingCard_ReturnsNull()
    {
        var repository = database.GetCardRepository();
        var nonExistingId = Guid.NewGuid();

        var result = repository.GetById(nonExistingId);

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllCards()
    {
        var repository = database.GetCardRepository();

        var result = repository.GetAll();

        Assert.Equal(3, result.Count);
    }
    
    [Fact]
    public void GetAll_WithPagination_ReturnsPagedResults()
    {
        var repository = database.GetCardRepository();

        var result = repository.GetAll(null, null, 1, 2);

        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public void Search_ReturnsCorrectCards()
    {
        var repository = database.GetCardRepository();
        
        var result = repository.Search("What");
        
        Assert.NotNull(result);
        Assert.Contains(result, c => c.Id == database.CardGuids[0]);
        Assert.Contains(result, c => c.Id == database.CardGuids[1]);
        Assert.Contains(result, c => c.Id == database.CardGuids[2]);
    }
    
    [Fact]
    public void Search_NotFound_ReturnsEmpty()
    {
        var repository = database.GetCardRepository();
        
        var result = repository.Search("Java");
        
        Assert.Empty(result);
    }
    
    [Fact]
    public void SearchByCreatorId_ReturnsCorrectCards()
    {
        var repository = database.GetCardRepository();
        var creatorId = database.UserGuids[0];
        var wantedCardId = database.CardGuids[0];
        
        var result = repository.SearchByCreatorId(creatorId);
        
        Assert.NotNull(result);
        Assert.Equal(result.First().Id, wantedCardId);
    }
    
    [Fact]
    public void SearchByCollectionId_ReturnsCorrectCards()
    {
        var repository = database.GetCardRepository();
        var collectionId = database.CollectionGuids[0];
        
        var result = repository.SearchByCollectionId(collectionId);
        
        var wantedCardId = database.CardGuids[0];
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Id == wantedCardId);
        Assert.Contains(result, c => c.Question == "What is C#?");
    }

    [Fact]
    public void Insert_NewCard_CardIsInserted()
    {
        var repository = database.GetCardRepository();
        var newCardId = Guid.NewGuid();

        var newCard = new CardEntity
        {
            Id = newCardId,
            QuestionType = QAType.Text,
            Question = "Test Question",
            AnswerType = QAType.Text,
            Answer = "Test Answer",
            CollectionId = database.CollectionGuids[0],
            CreatorId = database.UserGuids[0]
        };

        repository.Insert(newCard);

        var insertedCard = database.GetCardDirectly(newCardId);
        Assert.NotNull(insertedCard);
        Assert.Equal("Test Question", insertedCard.Question);
    }
    
    [Fact]
    public void Update_ExistingCard_CardIsUpdated()
    {
        var repository = database.GetCardRepository();
        var cardId = database.CardGuids[0];
        
        var card = repository.GetById(cardId);
        Assert.NotNull(card);
        
        var updatedCard = new CardEntity()
        {
            Id = cardId,
            QuestionType = QAType.Text,
            Question = "Updated question",
            AnswerType = QAType.Text,
            Answer = "Updated answer",
            CreatorId = database.UserGuids[0],
            CollectionId = database.CollectionGuids[0]
        };
        
        repository.Update(updatedCard);
        
        var result = database.GetCardDirectly(cardId);
        Assert.NotNull(result);
        Assert.Equal("Updated question", result.Question);
        Assert.Equal("Updated answer", result.Answer);
    }
    
    [Fact]
    public void Insert_NonExistingCard_CardIsInserted()
    {
        var repository = database.GetCardRepository();
        var cardId = Guid.NewGuid();
        
        var newCard = new CardEntity
        {
            Id = cardId,
            QuestionType = QAType.Text,
            Question = "Inserted question",
            AnswerType = QAType.Text,
            Answer = "Inserted answer",
            CreatorId = database.UserGuids[0],
            CollectionId = database.CollectionGuids[0]
        };
        
        repository.Insert(newCard);
        
        var result = database.GetCardDirectly(cardId);
        
        Assert.NotNull(result);
        Assert.Equal("Inserted question", result.Question);
    }
    
    [Fact]
    public void Delete_ExistingCard_CardIsDeleted()
    {
        var repository = database.GetCardRepository();
        var cardToDeleteId = database.CardGuids[0];
        
        repository.Remove(cardToDeleteId);
        
        var result = database.GetCardDirectly(cardToDeleteId);
        
        Assert.Null(result);
    }
    
    [Fact]
    public void Exists_ExistingCard_ReturnsTrue()
    {
        var repository = database.GetCardRepository();
        var cardId = database.CardGuids[0];
        
        var result = repository.Exists(cardId);
        
        Assert.True(result);
    }
    
    [Fact]
    public void Exists_NonExistentCard_ReturnsFalse()
    {
        var repository = database.GetCardRepository();
        var cardId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        
        var result = repository.Exists(cardId);
        
        Assert.False(result);
    }
}
