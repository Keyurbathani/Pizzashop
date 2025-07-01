using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IMenuService
{

    Task<Response<IEnumerable<CategoryVM>>> GetAllCategoriesAsync();

    Task<CategoryVM> GetCategoryByIdAsync(int id);

    Task<Response<CategoryVM>> AddCategoryAsync(CategoryVM model);

    Task<Response<CategoryVM>> EditCategoryAsync(CategoryVM model);

    Task<Response<bool>> DeleteCategoryAsync(int id);

    Task<IEnumerable<ItemListVM>> GetItemsByCategoryId(int categoryId);
    Task<Response<PagedResult<ItemListVM>>> GetPagedItemsAsync(int categoryId, string searchString, int page, int pagesize, bool isASC);

    Task<ItemVM?> GetItemByIdAsync(int id);

    Task<Response<ItemVM>> AddItemAsync(ItemVM model);

    Task<Response<ItemVM>> EditItemAsync(ItemVM model);

    Task<Response<bool>> DeleteItemAsync(int id);
    Task<Response<IEnumerable<UnitVM>>> GetAllUnitsAsync();
    Task<bool> DeleteManyItemAsync(IEnumerable<int> ids);

    Task<Response<IEnumerable<ModifierGroupVM>>> GetAllModifierGroupsAsync();
    Task<Response<ModifierGroupVM?>> GetModifierGroupByIdAsync(int id);
    Task<Response<IEnumerable<ModifierGroupVM>>> GetModifierGroupsByModifierIdAsync(int modifierId);
    Task<Response<ModifierGroupVM?>> AddModifierGroupAsync(ModifierGroupVM model, List<int> modifierIds);
    Task<Response<ModifierGroupVM?>> EditModifierGroupAsync(ModifierGroupVM model, List<int> modifierIds);
    Task<Response<bool>> DeleteModifierGroupAsync(int id);


    Task<Response<IEnumerable<ModifierListVM>>> GetModifiersByGroupIdAsync(int modifierGroupId);
    Task<Response<IEnumerable<ModifierListVM>>> GetAllModifiersAsync();

    Task<Response<PagedResult<ModifierListVM>>> GetAllExistingModifiers(string searchString, int page, int pagesize);

    Task<Response<ModifierVM?>> GetModifierByIdAsync(int id);
    Task<Response<ModifierVM?>> AddModifierAsync(ModifierVM model, List<int> modifierGroups);
    Task<Response<ModifierVM?>> EditModfierAsync(ModifierVM model, List<int> modifierGroups);
    Task<Response<bool>> DeleteModifierAsync(int id);
    Task<Response<bool>> DeleteManyModifierAsync(IEnumerable<int> ids);

    Task<Response<PagedResult<ModifierListVM>>> GetPagedMofiersAsync(int modifierGroupId, string searchString, int page, int pagesize, bool isASC);

    Task<Response<IEnumerable<ModifierGroupForItemVM>>> GetApplicableModifiersForItem(int itemId);

    Task<Response<IEnumerable<ItemListVM>>> GetFavoriteItemsAsync();

    Task<Response<bool>> ToggleFavoriteItemAsync(int itemId);

    Task<Response<IEnumerable<ItemListVM>>> GetAvailableItemsByCategoryIdAsync(int categoryId);


}
