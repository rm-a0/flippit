using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.Common.Extensions
{
    public static class UserQueryableExtensions
    {
        public static IQueryable<UserEntity> ApplyFilterSortAndPage(
            this IQueryable<UserEntity> query,
            string? filter = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(u =>
                    u.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "name" => query.OrderBy(u => u.Name),
                    "nameDesc" => query.OrderByDescending(u => u.Name),
                    "id" => query.OrderBy(u => u.Id),
                    "idDesc" => query.OrderByDescending(u => u.Id),
                    _ => query
                };
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
