using System.Linq;
using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.Common.Extensions
{
    public static class CompletedLessonQueryableExtensions
    {
        public static IQueryable<CompletedLessonEntity> ApplySortAndPage(
            this IQueryable<CompletedLessonEntity> query,
            string? sortBy = null,
            int page = 1,
            int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "id" => query.OrderBy(l => l.Id),
                    "userid" => query.OrderBy(l => l.UserId),
                    "collectionid" => query.OrderBy(l => l.CollectionId),
                    _ => query 
                };
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
