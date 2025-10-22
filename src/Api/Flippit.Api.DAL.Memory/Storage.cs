using Flippit.Api.DAL.Common.Entities;
using Flippit.Common.Enums;
using System;
using System.Collections.Generic;

namespace Flippit.Api.DAL.Memory
{
    public class Storage
    {
        private readonly IList<Guid> cardGuids = new List<Guid>
        {
            new("a1b2c3d4-e5f6-7890-abcd-1234567890ab"),
            new("b2c3d4e5-f6a7-8901-bcde-2345678901bc"),
            new("c3d4e5f6-a7b8-9012-cdef-3456789012cd")
        };

        private readonly IList<Guid> userGuids = new List<Guid>
        {
            new("d4e5f6a7-b890-1234-defa-4567890123de"),
            new("e5f6a7b8-9012-3456-efab-5678901234ef")
        };

        private readonly IList<Guid> collectionGuids = new List<Guid>
        {
            new("f6a7b890-1234-5678-fabc-6789012345fa"),
            new("a7b89012-3456-7890-abcd-7890123456ab")
        };

        public IList<CardEntity> Cards { get; } = new List<CardEntity>();
        public IList<UserEntity> Users { get; } = new List<UserEntity>();
        public IList<CollectionEntity> Collections { get; } = new List<CollectionEntity>();

        public Storage(bool seedData = true)
        {
            if (seedData)
            {
                SeedUsers();
                SeedCards();
                SeedCollections();
            }
        }

        private void SeedUsers()
        {
            Users.Add(new UserEntity
            {
                Id = userGuids[0],
                Name = "John Doe",
                photoUrl = "https://example.com/john.jpg",
                Role = Role.user
            });

            Users.Add(new UserEntity
            {
                Id = userGuids[1],
                Name = "Admin Jane",
                photoUrl = "https://example.com/jane.jpg",
                Role = Role.admin
            });
        }

        private void SeedCards()
        {
            Cards.Add(new CardEntity
            {
                Id = cardGuids[0],
                QuestionType = QAType.text,
                AnswerType = QAType.text,
                Question = "What is the capital of France?",
                Answer = "Paris",
                Description = "Basic geography question",
                CreatorId = userGuids[0],
                CollectionId = collectionGuids[0]
            });

            Cards.Add(new CardEntity
            {
                Id = cardGuids[1],
                QuestionType = QAType.url,
                AnswerType = QAType.text,
                Question = "https://example.com/eiffel.jpg",
                Answer = "Eiffel Tower",
                Description = "Landmark identification",
                CreatorId = userGuids[0],
                CollectionId = collectionGuids[0]
            });

            Cards.Add(new CardEntity
            {
                Id = cardGuids[2],
                QuestionType = QAType.text,
                AnswerType = QAType.text,
                Question = "What is 2 + 2?",
                Answer = "4",
                CreatorId = userGuids[1],
                CollectionId = collectionGuids[1]
            });
        }

        private void SeedCollections()
        {
            Collections.Add(new CollectionEntity
            {
                Id = collectionGuids[0],
                Name = "Geography Basics",
                StartTime = DateTime.UtcNow.AddDays(-1),
                EndTime = DateTime.UtcNow.AddDays(7),
                CreatorId = userGuids[0]
            });

            Collections.Add(new CollectionEntity
            {
                Id = collectionGuids[1],
                Name = "Math Fundamentals",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(14),
                CreatorId = userGuids[1]
            });
        }
    }
}
