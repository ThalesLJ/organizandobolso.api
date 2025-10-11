using OrganizandoBolso.Domain.Models.Base;

namespace OrganizandoBolso.Domain.Interfaces;

public interface IBaseService<T> where T : BaseEntity
{
    Task<ServiceResponse<T>> GetByIdAsync(string id);
    Task<ServiceResponse<IEnumerable<T>>> GetAllAsync();
    Task<ServiceResponse<T>> CreateAsync(T entity, string user);
    Task<ServiceResponse<T>> UpdateAsync(T entity, string user);
    Task<ServiceResponse<bool>> DeleteAsync(string id, string user);
}
