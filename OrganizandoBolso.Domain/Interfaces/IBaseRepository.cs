using OrganizandoBolso.Domain.Models.Base;

namespace OrganizandoBolso.Domain.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetByFilterAsync(Func<T, bool> filter);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);
}
