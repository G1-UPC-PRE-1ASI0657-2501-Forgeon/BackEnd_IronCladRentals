namespace IronClead.SharedKernel.Shared.Domain.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<TEntity?> FindByIdAsync(int id);
    Task<IEnumerable<TEntity>> ListAsync();
    
    Task AddAsync(TEntity entity);
    Task DeleteAsync(int id);

    void Update(TEntity entity);
    void Remove(TEntity entity);
}