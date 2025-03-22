using EMS.Data;
using EMS.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EMS.Repositories
{
    public interface IGenericDBRepository<T> where T : class
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

    public class GenericDBRepository<T> : IGenericDBRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public GenericDBRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // In EMS.Repositories
        public async Task<IEnumerable<T>> GetByMultipleConditionsAsync(List<FilterDTO> filters)
        {
            if (filters == null || !filters.Any())
            {
                return await _context.Set<T>().ToListAsync();
            }

            var entityType = typeof(T);
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression combinedExpression = null;

            foreach (var filter in filters)
            {
                var property = entityType.GetProperty(filter.PropertyName);
                if (property == null)
                    throw new ArgumentException($"Property '{filter.PropertyName}' does not exist on '{entityType.Name}'");

                var propertyAccess = Expression.Property(parameter, property);

                // Check if the property type matches the type of the value being passed
                var propertyType = property.PropertyType;
                var constantValue = Expression.Constant(Convert.ChangeType(filter.Value, propertyType));

                // Handle different comparison types (Equality for now)
                Expression filterExpression = Expression.Equal(propertyAccess, constantValue);

                // Combine expressions using 'AND' logic
                combinedExpression = combinedExpression == null
                    ? filterExpression
                    : Expression.AndAlso(combinedExpression, filterExpression);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            return await _context.Set<T>().Where(lambda).ToListAsync();
        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<bool> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            var result = await _context.SaveChangesAsync();
            return result > 0; // Return true if 1 or more rows are affected
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            var result = await _context.SaveChangesAsync();
            return result > 0; // Return true if any rows were updated
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                var result = await _context.SaveChangesAsync();
                return result > 0; // Return true if deletion is successful
            }
            return false;  // Entity not found, deletion failed
        }

        public async Task<PagedResultDTO<T>> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Set<T>().AsQueryable();

            var totalRecords = await query.CountAsync();
            var pagedItems = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDTO<T>
            {
                Items = pagedItems,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
