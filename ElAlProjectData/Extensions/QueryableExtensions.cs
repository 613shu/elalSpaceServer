using Microsoft.EntityFrameworkCore;

namespace ElAlProjectData.Extensions
{
    public static class QueryableExtensions
    {
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 100;
        //pagination

        // Runs against the database: one COUNT query and one query with OFFSET/LIMIT.
        // The query must already be sorted (OrderBy) before calling this.
        public static async Task<(IEnumerable<T> Items, int TotalCount)> ToPagedAsync<T>(
            this IQueryable<T> query, int page, int pageSize, CancellationToken cancellationToken)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
