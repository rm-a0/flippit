using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.Common.Extensions
{
    public static class CollectionQueryableExtensions
    {
        public static IQueryable<CollectionEntity> ApplyFilterSortAndPage(
            this IQueryable<CollectionEntity> query,
            string? filter = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(c => c.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "name" => query.OrderBy(c => c.Name),
                    "nameDesc" => query.OrderByDescending(c => c.Name),
                    "id" => query.OrderBy(c => c.Id),
                    "idDesc" => query.OrderBy(c => c.Id),
                    _ => query
                };
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
