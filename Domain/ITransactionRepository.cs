using Domain.Databse.Models;
using Domain.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
    public interface ITransactionRepository
    {
        void Add(TransactionDbModel transactionDbModel);

        PaginationResult<TransactionDbModel> GetByFilter(PaginationQuery paginationQuery);
        Task<IEnumerable<TransactionDbModel>> GetAllAsync();
    }
}