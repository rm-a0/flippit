using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Mappers;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.DAL.Memory;
using Flippit.Api.DAL.Memory.Repositories;
using Flippit.Common.Enums;

namespace Flippit.Api.DAL.IntegrationTests;

public class InMemoryTestDataProvider : ITestDataProvider
{
    private readonly CardMapper cardMapper;
    private readonly CollectionMapper collectionMapper;
    private readonly CompletedLessonMapper completedLessonMapper;
    private readonly UserMapper userMapper;
    private Storage storage;

    public InMemoryTestDataProvider()
    {
        cardMapper = new CardMapper();
        collectionMapper = new CollectionMapper();
        completedLessonMapper = new CompletedLessonMapper();
        userMapper = new UserMapper();
        storage = new Storage(false);
        ResetStorage();
    }
    
    public void ResetStorage()
    {
        storage = new Storage(false);
        SeedStorage(storage);
    }
    
    public IList<Guid> CardGuids { get; } = new List<Guid>
    {
        new("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"),
        new("b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e"),
        new("c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f")
    };

    public IList<Guid> CollectionGuids { get; } = new List<Guid>
    {
        new("d4e5f6a7-b8c9-4d5e-1f2a-3b4c5d6e7f8a"),
        new("e5f6a7b8-c9d0-4e5f-2a3b-4c5d6e7f8a9b")
    };

    public IList<Guid> CompletedLessonGuids { get; } = new List<Guid>
    {
        new("f6a7b8c9-d0e1-4f5a-3b4c-5d6e7f8a9b0c"),
        new("a7b8c9d0-e1f2-4a5b-4c5d-6e7f8a9b0c1d")
    };

    public IList<Guid> UserGuids { get; } = new List<Guid>
    {
        new("b8c9d0e1-f2a3-4b5c-5d6e-7f8a9b0c1d2e"),
        new("c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f")
    };

    private void SeedStorage(Storage storage)
    {
        storage.Users.Add(new UserEntity
        {
            Id = UserGuids[0],
            Name = "Test User 1"
        });
        storage.Users.Add(new UserEntity
        {
            Id = UserGuids[1],
            Name = "Test User 2"
        });

        storage.Collections.Add(new CollectionEntity
        {
            Id = CollectionGuids[0],
            Name = "Search me",
            CreatorId = UserGuids[0],
            StartTime = DateTime.Now,
            EndTime = DateTime.Now
        });
        storage.Collections.Add(new CollectionEntity
        {
            Id = CollectionGuids[1],
            Name = "Test Collection 2",
            CreatorId = UserGuids[1],
            StartTime = DateTime.Now,
            EndTime = DateTime.Now
        });

        storage.Cards.Add(new CardEntity
        {
            Id = CardGuids[0],
            QuestionType = QAType.Text,
            Question = "What is C#?",
            AnswerType = QAType.Text,
            Answer = "A programming language",
            CreatorId = UserGuids[0],
            CollectionId = CollectionGuids[0]
        });
        storage.Cards.Add(new CardEntity
        {
            Id = CardGuids[1],
            QuestionType = QAType.Text,
            Question = "What is .NET?",
            AnswerType = QAType.Text,
            Answer = "A framework",
            CreatorId = UserGuids[1],
            CollectionId = CollectionGuids[0]
        });
        storage.Cards.Add(new CardEntity
        {
            Id = CardGuids[2],
            QuestionType = QAType.Text,
            Question = "What is Entity Framework?",
            AnswerType = QAType.Text,
            Answer = "An ORM",
            CreatorId = UserGuids[1],
            CollectionId = CollectionGuids[1]
        });

        storage.CompletedLessons.Add(new CompletedLessonEntity
        {
            Id = CompletedLessonGuids[0],
            AnswersJson =  "Answer",
            StatisticsJson = "Search me",
            UserId = UserGuids[0],
            CollectionId = CollectionGuids[0]
        });
        storage.CompletedLessons.Add(new CompletedLessonEntity
        {
            Id = CompletedLessonGuids[1],
            AnswersJson =  "Answer",
            StatisticsJson = "Statistics",
            UserId = UserGuids[1],
            CollectionId = CollectionGuids[1]
        });
    }

    private T? DeepClone<T>(T? input)
    {
        if (input == null) return default;
        var json = JsonSerializer.Serialize(input);
        return JsonSerializer.Deserialize<T>(json);
    }

    public CardEntity? GetCardDirectly(Guid id)
    {
        var card = storage.Cards.SingleOrDefault(c => c.Id == id);
        return DeepClone(card);
    }

    public CollectionEntity? GetCollectionDirectly(Guid id)
    {
        var collection = storage.Collections.SingleOrDefault(c => c.Id == id);
        return DeepClone(collection);
    }

    public CompletedLessonEntity? GetCompletedLessonDirectly(Guid id)
    {
        var completedLesson = storage.CompletedLessons.SingleOrDefault(cl => cl.Id == id);
        return DeepClone(completedLesson);
    }

    public UserEntity? GetUserDirectly(Guid id)
    {
        var user = storage.Users.SingleOrDefault(u => u.Id == id);
        return DeepClone(user);
    }

    public ICardRepository GetCardRepository()
    {
        return new CardRepository(storage, cardMapper);
    }

    public ICollectionRepository GetCollectionRepository()
    {
        return new CollectionRepository(storage, collectionMapper);
    }

    public ICompletedLessonRepository GetCompletedLessonRepository()
    {
        return new CompletedLessonRepository(storage, completedLessonMapper);
    }

    public IUserRepository GetUserRepository()
    {
        return new UserRepository(storage, userMapper);
    }
}
