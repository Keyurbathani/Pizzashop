using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IModifierAndGroupRepository
{
    Task<IEnumerable<Modifier>> GetModifiersByGroupIdAsync(int modifierGroupId);
    Task<IEnumerable<ModifierGroup>> GetModifierGroupsByModifierIdAsync(int modifierId);
    Task<IEnumerable<ModifierAndGroup>> GetModifiersAndGroupByGroupIdAsync(int modifierGroupId);
    Task<IEnumerable<ModifierAndGroup>> GetModifiersAndGroupByModifierIdAsync(int modifierId);
    Task<(IEnumerable<Modifier> list, int totalRecords)> GetPagedModifiersAsync(int modifierGroupId, string searchString, int page, int pagesize, bool isASC);
    Task AddRangeAsync(IEnumerable<ModifierAndGroup> modifiersAndGroups);
    Task UpdateRangeAsync(IEnumerable<ModifierAndGroup> modifiersAndGroups);
    Task DeleteRangeAsync(IEnumerable<ModifierAndGroup> modifiersAndGroups);
}