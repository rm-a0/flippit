using Flippit.Api.DAL.Common.Entities;
using Flippit.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json;

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

        private readonly IList<Guid> completedLessonGuids = new List<Guid>
        {
            new("123e4567-e89b-12d3-a456-426614174000"),
            new("223e4567-e89b-12d3-a456-426614174001")
        };

        public IList<CardEntity> Cards { get; } = new List<CardEntity>();
        public IList<UserEntity> Users { get; } = new List<UserEntity>();
        public IList<CollectionEntity> Collections { get; } = new List<CollectionEntity>();
        public IList<CompletedLessonEntity> CompletedLessons { get; } = new List<CompletedLessonEntity>();

        public Storage(bool seedData = true)
        {
            if (seedData)
            {
                SeedUsers();
                SeedCards();
                SeedCollections();
                SeedCompletedLessons();
            }
        }

        private void SeedUsers()
        {
            Users.Add(new UserEntity
            {
                Id = userGuids[0],
                Name = "John Doe",
                PhotoUrl = "https://example.com/john.jpg",
                Role = Role.User
            });

            Users.Add(new UserEntity
            {
                Id = userGuids[1],
                Name = "Admin Jane",
                PhotoUrl = "https://example.com/jane.jpg",
                Role = Role.Admin
            });
        }

        private void SeedCards()
        {
            Cards.Add(new CardEntity
            {
                Id = cardGuids[0],
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is the capital of France?",
                Answer = "Paris",
                Description = "Basic geography question",
                CreatorId = userGuids[0],
                CollectionId = collectionGuids[0]
            });

            Cards.Add(new CardEntity
            {
                Id = cardGuids[1],
                QuestionType = QAType.Url,
                AnswerType = QAType.Text,
                Question = "https://example.com/eiffel.jpg",
                Answer = "Eiffel Tower",
                Description = "Landmark identification",
                CreatorId = userGuids[0],
                CollectionId = collectionGuids[0]
            });

            Cards.Add(new CardEntity
            {
                Id = cardGuids[2],
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
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

        private void SeedCompletedLessons()
        {
            var answers1 = new Dictionary<Guid, bool>
            {
                { cardGuids[0], true },
                { cardGuids[1], false }
            };
            var stats1 = new { CorrectCount = 1, TotalCount = 2, Accuracy = 0.5 };
            CompletedLessons.Add(new CompletedLessonEntity
            {
                Id = completedLessonGuids[0],
                AnswersJson = JsonSerializer.Serialize(answers1),
                StatisticsJson = JsonSerializer.Serialize(stats1),
                UserId = userGuids[0],
                CollectionId = collectionGuids[0]
            });

            var answers2 = new Dictionary<Guid, bool>
            {
                { cardGuids[2], true }
            };
            var stats2 = new { CorrectCount = 1, TotalCount = 1, Accuracy = 1.0 };
            CompletedLessons.Add(new CompletedLessonEntity
            {
                Id = completedLessonGuids[1],
                AnswersJson = JsonSerializer.Serialize(answers2),
                StatisticsJson = JsonSerializer.Serialize(stats2),
                UserId = userGuids[1],
                CollectionId = collectionGuids[1]
            });
        }
    }
}
