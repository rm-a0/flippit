using Flippit.Common.Enums;
using Flippit.Api.DAL.Common.Entities;
using Xunit;
using System.Linq;
using Flippit.Api.DAL.Common.Repositories;

namespace Flippit.Api.DAL.IntegrationTests;

public class CardRepositoryTests : IClassFixture<InMemoryTestDataProvider>
{
    private readonly InMemoryTestDataProvider _database;
    private readonly ICardRepository _repository;

    public CardRepositoryTests(InMemoryTestDataProvider database)
    {
        _database = database;
        _database.ResetStorage();
        _repository = database.GetCardRepository();
    }

    [Fact]
    public void GetById_ExistingCard_ReturnsCard()
    {
        var cardId = _database.CardGuids[0];
        var card = _database.GetCardDirectly(cardId);
        var result = _repository.GetById(cardId);

        Assert.NotNull(result);
        Assert.Equal(card, result);
        Assert.Equal(cardId, result.Id);
    }

    [Fact]
    public void GetById_NonExistingCard_ReturnsNull()
    {
        var result = _repository.GetById(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllCards()
    {
        var result = _repository.GetAll();
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetAll_WithPagination_ReturnsPagedResults()
    {
        var result = _repository.GetAll(null, null, 1, 2);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Search_ReturnsCorrectCards()
    {
        var result = _repository.Search("What");

        Assert.NotNull(result);
        Assert.Contains(result, c => c.Id == _database.CardGuids[0]);
        Assert.Contains(result, c => c.Id == _database.CardGuids[1]);
        Assert.Contains(result, c => c.Id == _database.CardGuids[2]);
    }

    [Fact]
    public void Search_NotFound_ReturnsEmpty()
    {
        var result = _repository.Search("Java");
        Assert.Empty(result);
    }

    [Fact]
    public void SearchByCreatorId_ReturnsCorrectCards()
    {
        var creatorId = _database.UserGuids[0];
        var result = _repository.SearchByCreatorId(creatorId);

        Assert.NotNull(result);
        Assert.Equal(_database.CardGuids[0], result.First().Id);
    }

    [Fact]
    public void SearchByCollectionId_ReturnsCorrectCards()
    {
        var collectionId = _database.CollectionGuids[0];
        var result = _repository.SearchByCollectionId(collectionId);

        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Id == _database.CardGuids[0]);
        Assert.Contains(result, c => c.Question == "What is C#?");
    }

    [Fact]
    public void Insert_NewCard_CardIsInserted()
    {
        var newCardId = Guid.NewGuid();
        var newCard = new CardEntity
        {
            Id = newCardId,
            QuestionType = QAType.Text,
            Question = "Test Question",
            AnswerType = QAType.Text,
            Answer = "Test Answer",
            CollectionId = _database.CollectionGuids[0],
            CreatorId = _database.UserGuids[0]
        };

        _repository.Insert(newCard);

        var insertedCard = _database.GetCardDirectly(newCardId);

        Assert.NotNull(insertedCard);
        Assert.Equal(newCardId, insertedCard.Id);
        Assert.Equal("Test Question", insertedCard.Question);
        Assert.Equal("Test Answer", insertedCard.Answer);
    }

    [Fact]
    public void Update_ExistingCard_CardIsUpdated()
    {
        var cardId = _database.CardGuids[0];
        var card = _repository.GetById(cardId);
        Assert.NotNull(card);

        var updatedCard = new CardEntity
        {
            Id = cardId,
            QuestionType = QAType.Text,
            Question = "Updated question",
            AnswerType = QAType.Text,
            Answer = "Updated answer",
            CreatorId = _database.UserGuids[0],
            CollectionId = _database.CollectionGuids[0]
        };

        _repository.Update(updatedCard);

        var result = _database.GetCardDirectly(cardId);
        Assert.Equal("Updated question", result!.Question);
        Assert.Equal("Updated answer", result.Answer);
    }

    [Fact]
    public void Insert_NonExistingCard_CardIsInserted()
    {
        var cardId = Guid.NewGuid();
        var newCard = new CardEntity
        {
            Id = cardId,
            QuestionType = QAType.Text,
            Question = "Inserted question",
            AnswerType = QAType.Text,
            Answer = "Inserted answer",
            CreatorId = _database.UserGuids[0],
            CollectionId = _database.CollectionGuids[0]
        };

        _repository.Insert(newCard);

        var result = _database.GetCardDirectly(cardId);
        Assert.NotNull(result);
        Assert.Equal("Inserted question", result.Question);
    }

    [Fact]
    public void Delete_ExistingCard_CardIsDeleted()
    {
        var cardToDeleteId = _database.CardGuids[0];

        _repository.Remove(cardToDeleteId);

        var result = _database.GetCardDirectly(cardToDeleteId);
        Assert.Null(result);
    }

    [Fact]
    public void Exists_ExistingCard_ReturnsTrue()
    {
        var result = _repository.Exists(_database.CardGuids[0]);
        Assert.True(result);
    }

    [Fact]
    public void Exists_NonExistentCard_ReturnsFalse()
    {
        var result = _repository.Exists(Guid.Parse("00000000-0000-0000-0000-000000000000"));
        Assert.False(result);
    }
}
