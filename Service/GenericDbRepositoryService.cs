using EMS.DTOs;
using EMS.Repositories;
using System.Linq.Expressions;

namespace EMS.Service
{
    public interface IGenericDBService<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        //Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetByMultipleConditionsAsync(List<FilterDTO> filters);  // Updated to handle dynamic filters
        Task<PagedResultDTO<T>> GetPaginatedAsync(int pageNumber, int pageSize);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }

    public class GenericDBService<T> : IGenericDBService<T> where T : class
    {
        private readonly IGenericDBRepository<T> _repository;

        public GenericDBService(IGenericDBRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDTO<T>> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            return await _repository.GetPaginatedAsync(pageNumber, pageSize);
        }

        //public async Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate)
        //{
        //    return await _repository.GetByConditionAsync(predicate);
        //}
        // In EMS.Service
        public async Task<IEnumerable<T>> GetByMultipleConditionsAsync(List<FilterDTO> filters)
        {
            return await _repository.GetByMultipleConditionsAsync(filters);
        }


        public async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<bool> AddAsync(T entity)
        {
            return await _repository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
