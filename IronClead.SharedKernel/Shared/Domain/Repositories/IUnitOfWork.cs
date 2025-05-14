using Microsoft.EntityFrameworkCore.Storage;

namespace IronClead.SharedKernel.Shared.Domain.Repositories;

public interface IUnitOfWork
{

    Task CompleteAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();

}