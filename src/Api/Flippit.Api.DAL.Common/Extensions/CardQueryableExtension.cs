using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.Common.Extensions
{
    public static class CardQueryableExtensions
    {
        public static IQueryable<CardEntity> ApplyFilterSortAndPage(
            this IQueryable<CardEntity> query,
            string? filter = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(c =>
                    c.Question.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    (c.Description != null && c.Description.Contains(filter, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "question" => query.OrderBy(c => c.Question),
                    "questionDesc" => query.OrderByDescending(c => c.Question),
                    "id" => query.OrderBy(c => c.Id),
                    "idDesc" => query.OrderByDescending(c => c.Id),
                    _ => query
                };
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
