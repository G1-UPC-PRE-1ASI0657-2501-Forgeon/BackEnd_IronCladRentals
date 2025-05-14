using Microsoft.EntityFrameworkCore.Storage;
using IronClead.SharedKernel.Shared.Infraestructure.Persistences.EFC.Configuration;
using IronClead.SharedKernel.Shared.Domain.Repositories;
using IronClead.SharedKernel.Shared.Infraestructure.Persistences.EFC.Configuration;
using IronClead.SharedKernel.Shared.Infraestructure.Persistences.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace IronClead.SharedKernel.Shared.Infraestructure.Persistences.EFC.Repositories;

public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context;

    public UnitOfWork(TContext context)
    {
        _context = context;
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}