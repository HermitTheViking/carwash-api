using Domain.Pagination;
using System;
using System.Linq;

namespace Domain.Implementations
{
    public abstract class BaseRepository
    {
        protected PaginationResult<T> GetPaginatedResult<T>(
            PaginationQuery paginationQuery,
            IQueryable<T> sourceElements,
            Func<IQueryable<T>, string, IQueryable<T>> filterElements,
            Func<IQueryable<T>, IQueryable<T>> sort)
        {
            if (paginationQuery == null) { paginationQuery = new PaginationQuery(); }

            if (!string.IsNullOrWhiteSpace(paginationQuery.Filter)) { sourceElements = filterElements(sourceElements, paginationQuery.Filter); }

            var total = sourceElements.Count();

            sourceElements = sort(sourceElements);
            if (paginationQuery.Skip.HasValue) { sourceElements = sourceElements.Skip(paginationQuery.Skip.Value); }
            if (paginationQuery.Take.HasValue) { sourceElements = sourceElements.Take(paginationQuery.Take.Value); }

            return new PaginationResult<T>
            {
                MetaData = new PaginationResultMetaDataDto { TotalCount = total },
                Result = sourceElements.ToList()
            };

        }
    }
}