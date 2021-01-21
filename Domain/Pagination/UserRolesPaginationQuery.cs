using System.Collections.Generic;

namespace Domain.Pagination
{
    public class UserRolesPaginationQuery : PaginationQuery
    {
        public string ServiceCode { get; set; }
        public List<string> RoleCodes { get; set; }
        public bool IncludeEmptyGroups { get; set; }
    }
}