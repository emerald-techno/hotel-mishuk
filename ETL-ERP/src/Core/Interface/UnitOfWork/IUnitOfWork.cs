using Microsoft.EntityFrameworkCore.Storage;

namespace Interface.UnitOfWork
{
    public interface IUnitOfWork
    {
        long CurrentUserId { get; set; }
        bool Complete();
        Task<bool> CompleteAsync();
        void Dispose();
        Task<IDbContextTransaction> BeginTransaction();
    }
}
