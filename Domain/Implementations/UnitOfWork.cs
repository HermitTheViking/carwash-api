using System;

namespace Domain.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(
            IUserRepository users,
            ITransactionRepository transactions,
            IWashRepository wash)
        {
            Users = users ?? throw new ArgumentNullException(nameof(users));
            Transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
            Wash = wash ?? throw new ArgumentNullException(nameof(wash));
        }

        public IUserRepository Users { get; private set; }
        public ITransactionRepository Transactions { get; private set; }
        public IWashRepository Wash { get; private set; }
    }
}