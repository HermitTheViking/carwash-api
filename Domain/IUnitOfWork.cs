namespace Domain
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ITransactionRepository Transactions { get; }
        IWashRepository Wash { get; }
    }
}