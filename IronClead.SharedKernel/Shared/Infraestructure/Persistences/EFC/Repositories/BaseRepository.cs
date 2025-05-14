using IronClead.SharedKernel.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;


namespace IronClead.SharedKernel.Shared.Infraestructure.Persistences.EFC.Repositories;

public abstract class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    protected readonly TContext Context;

    protected BaseRepository(TContext context)
    {
        Context = context;
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await Context.Set<TEntity>().AddAsync(entity);
    }

    public virtual async Task<TEntity?> FindByIdAsync(int id)
    {
        return await Context.Set<TEntity>().FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> ListAsync()
    {
        return await Context.Set<TEntity>().ToListAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await FindByIdAsync(id);
        if (entity != null)
        {
            Context.Set<TEntity>().Remove(entity);
        }
    }

    public virtual void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }
}