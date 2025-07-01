using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IModifierGroupRepository
{
    Task<ModifierGroup?> GetByIdAsync(int id);
    
    Task<IEnumerable<ModifierGroup>> GetAllAsync();

    Task AddAsync(ModifierGroup modifierGroup);

    Task UpdateAsync(ModifierGroup modifierGroup);

    Task<bool> IsExistsAsync(Expression<Func<ModifierGroup, bool>> predicate);
}
