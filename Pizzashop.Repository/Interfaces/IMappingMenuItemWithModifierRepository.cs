using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IMappingMenuItemWithModifierRepository
{
    Task<IEnumerable<MappingMenuItemWithModifier>> GetModifierForItem(int itemId);

    Task<IEnumerable<MappingMenuItemWithModifier>> GetApplicableModifierByItemId(int itemId);
}
