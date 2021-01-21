using System.Collections.Generic;

namespace Domain.Pagination
{
    public class PaginationResult<T>
    {
        public PaginationResult()
        {
        }

        public PaginationResultMetaDataDto MetaData { get; set; }

        public IList<T> Result { get; set; }
    }

    public class PaginationResultMetaDataDto
    {
        public int TotalCount { get; set; }
    }
}