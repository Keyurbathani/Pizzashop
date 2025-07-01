using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ISectionRepository
{
        Task<IEnumerable<Section>> GetAllAsync();
        Task<Section?> GetByIdAsync(int id);

        Task AddAsync(Section section);

        Task UpdateAsync(Section section);

        Task<bool> IsExistsAsync(Expression<Func<Section, bool>> predicate);

        Task<IEnumerable<Section>> GetAllSectionForApp();
}
