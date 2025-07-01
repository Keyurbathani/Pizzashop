using Microsoft.AspNetCore.Mvc.RazorPages;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class MenuService : IMenuService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IModifierAndGroupRepository _modifierAndGroupRepository;
    private readonly IMappingMenuItemWithModifierRepository _itemAndModifierGroupRepository;
    private readonly IModifierGroupRepository _modifierGroupRepository;
    private readonly IModifierRepository _modifierRepository;
    private readonly IUnitRepository _unitRepository;

    public MenuService(ICategoryRepository categoryRepository, IItemRepository itemRepository, IModifierAndGroupRepository modifierAndGroupRepository, IModifierGroupRepository modifierGroupRepository, IModifierRepository modifierRepository, IUnitRepository unitRepository, IMappingMenuItemWithModifierRepository itemAndModifierGroupRepository)
    {
        _categoryRepository = categoryRepository;
        _itemRepository = itemRepository;
        _modifierAndGroupRepository = modifierAndGroupRepository;
        _modifierGroupRepository = modifierGroupRepository;
        _modifierRepository = modifierRepository;
        _unitRepository = unitRepository;
        _itemAndModifierGroupRepository = itemAndModifierGroupRepository;

    }

    public async Task<Response<IEnumerable<CategoryVM>>> GetAllCategoriesAsync()
    {

        IEnumerable<MenuCategory> categories = await _categoryRepository.GetAllAsync();

        IEnumerable<CategoryVM> categoryList = categories.Select(c => new CategoryVM()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList();

        return Response<IEnumerable<CategoryVM>>.SuccessResponse(categoryList, "Category Fetched Successfully!");

    }

    public async Task<CategoryVM> GetCategoryByIdAsync(int id)
    {

        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return null!;
        }

        CategoryVM CategoryVM = new()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,

        };

        return CategoryVM;

    }

    public async Task<Response<CategoryVM>> AddCategoryAsync(CategoryVM model)
    {

        var IsExist = await _categoryRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<CategoryVM>.FailureResponse("Category Already Exists");
        }

        MenuCategory category = new()
        {
            Name = model.Name,
            Description = model.Description,

        };
        await _categoryRepository.AddAsync(category);
        return Response<CategoryVM>.SuccessResponse(model, "Category Added Successfully!");

    }

    public async Task<Response<CategoryVM>> EditCategoryAsync(CategoryVM model)
    {

        var IsExist = await _categoryRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && u.Id != model.Id && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<CategoryVM>.FailureResponse("Category Already Exists");
        }

        var category = await _categoryRepository.GetByIdAsync(model.Id);
        if (category == null)
        {
            return Response<CategoryVM>.FailureResponse("Category is not Exists");
        }

        category.Id = model.Id;
        category.Name = model.Name;
        category.Description = model.Description;

        await _categoryRepository.UpdateAsync(category);

        return Response<CategoryVM>.SuccessResponse(model, "Category Edited Successfully!");

    }

    public async Task<Response<bool>> DeleteCategoryAsync(int id)
    {

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return Response<bool>.FailureResponse("Category is not Exists");
        }

        IEnumerable<MenuItem> itemsToDelete = await _itemRepository.GetAllByCategoryIdAsync(id);
        foreach (MenuItem item in itemsToDelete)
        {
            item.IsDeleted = true;
        }
        await _itemRepository.UpdateRangeAsync(itemsToDelete);

        category.IsDeleted = true;

        await _categoryRepository.UpdateAsync(category);

        return Response<bool>.SuccessResponse(true, "Category Deleted Successfully!");


    }

    public async Task<IEnumerable<ItemListVM>> GetItemsByCategoryId(int categoryId)
    {

        IEnumerable<MenuItem> items = await _itemRepository.GetAllByCategoryIdAsync(categoryId);

        IEnumerable<ItemListVM> itemlist = items.Select(i => new ItemListVM()
        {
            Id = i.Id,
            categoryId = i.CategoryId,
            Name = i.Name,
            Type = i.ItemType,
            Rate = i.Rate,
            Quantity = i.Quantity,
            Imageurl = i.ProfileImage,
            IsAvailable = i.IsAvailable.GetValueOrDefault(),
            IsFavourite = i.IsFavourite

        }).ToList();

        return itemlist;

    }

    public async Task<Response<PagedResult<ItemListVM>>> GetPagedItemsAsync(int categoryId, string searchString, int page, int pagesize, bool isASC)
    {

        (IEnumerable<MenuItem> items, int totalRecords) = await _itemRepository.GetPagedItemsAsync(categoryId, searchString, page, pagesize, isASC);

        IEnumerable<ItemListVM> itemlist = items.Select(i => new ItemListVM()
        {
            Id = i.Id,
            Name = i.Name,
            Type = i.ItemType,
            Rate = i.Rate,
            Quantity = i.Quantity,
            Imageurl = i.ProfileImage,
            IsAvailable = i.IsAvailable.GetValueOrDefault()
        }).ToList();

        PagedResult<ItemListVM> pagedResult = new()
        {
            PagedList = itemlist
        };

        pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

        return Response<PagedResult<ItemListVM>>.SuccessResponse(pagedResult, "Item list fetched successfully!");

    }

    public async Task<ItemVM?> GetItemByIdAsync(int id)
    {

        var item = await _itemRepository.GetByIdAsync(id);
        if (item == null)
        {
            return null;
        }

        ItemVM itemVM = new()
        {
            Id = item.Id,
            CategoryId = item.CategoryId,
            Name = item.Name,
            Type = item.ItemType,
            Rate = item.Rate,
            Quantity = item.Quantity,
            UnitId = item.UnitId,
            Shortcode = item.ShortCode,
            Description = item.Description,
            IsDefaultTax = item.IsDefaultTax.GetValueOrDefault(),
            TaxPercentage = item.TaxPercentage,
            IsFavourite = item.IsFavourite,
            IsAvailable = item.IsAvailable.GetValueOrDefault(),
            Imageurl = item.ProfileImage,

        };

        IEnumerable<MappingMenuItemWithModifier> modifierGroupsForItem = await _itemRepository.GetByItemId(item.Id);

        itemVM.SelectedModifierGroups = modifierGroupsForItem.Select(mg => new ModifierGrouopForItem()
        {
            Id = mg.ModifierGroupId.GetValueOrDefault(),
            MinSelection = mg.MinSelectionRequired.GetValueOrDefault(),
            MaxSelection = mg.MaxSelectionRequired.GetValueOrDefault()
        }).ToList();

        return itemVM;


    }

    public async Task<Response<ItemVM>> AddItemAsync(ItemVM model)
    {

        var IsExist = await _itemRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && u.CategoryId == model.CategoryId && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<ItemVM>.FailureResponse("Item Already Exists");
        }

        if (model.ProfileImage != null)
        {
            var fileName = Path.GetFileNameWithoutExtension(model.ProfileImage.FileName);
            var extension = Path.GetExtension(model.ProfileImage.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            var path = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                model.ProfileImage.CopyTo(fileStream);
            }

            model.Imageurl = $"uploads/{uniqueFileName}";

        }

        MenuItem item = new()
        {
            CategoryId = model.CategoryId.GetValueOrDefault(),
            Name = model.Name,
            ItemType = model.Type,
            Rate = model.Rate.GetValueOrDefault(),
            Quantity = model.Quantity.GetValueOrDefault(),
            UnitId = model.UnitId.GetValueOrDefault(),
            ShortCode = model.Shortcode,
            Description = model.Description,
            IsDefaultTax = model.IsDefaultTax,
            TaxPercentage = model.TaxPercentage.GetValueOrDefault(),
            IsFavourite = model.IsFavourite,
            IsAvailable = model.IsAvailable,
            ProfileImage = model.Imageurl,

        };

        await _itemRepository.AddAsync(item);

        List<MappingMenuItemWithModifier> modifierGroupsForItem = model.SelectedModifierGroups.Select(mg => new MappingMenuItemWithModifier()
        {
            MenuItemId = item.Id,
            ModifierGroupId = mg.Id,
            MinSelectionRequired = mg.MinSelection,
            MaxSelectionRequired = mg.MaxSelection,
        }).ToList();

        await _itemRepository.AddRangeAsync(modifierGroupsForItem);

        return Response<ItemVM>.SuccessResponse(model, "Item Added Successfully!");


    }

    public async Task<Response<ItemVM>> EditItemAsync(ItemVM model)
    {

        var IsExist = await _itemRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && u.Id != model.Id && u.CategoryId == model.CategoryId && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<ItemVM>.FailureResponse("Item Already Exists");
        }

        var item = await _itemRepository.GetByIdAsync(model.Id);

        if (item == null)
        {
            return Response<ItemVM>.FailureResponse("Item Does Not Exists");

        }

        if (model.ProfileImage != null)
        {
            var fileName = Path.GetFileNameWithoutExtension(model.ProfileImage.FileName);
            var extension = Path.GetExtension(model.ProfileImage.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";


            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            var path = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                model.ProfileImage.CopyTo(fileStream);
            }

            // Save the relative path to the model property
            model.Imageurl = $"uploads/{uniqueFileName}";
        }

        item.CategoryId = model.CategoryId.GetValueOrDefault();
        item.Name = model.Name;
        item.ItemType = model.Type;
        item.Rate = model.Rate.GetValueOrDefault();
        item.Quantity = model.Quantity.GetValueOrDefault();
        item.UnitId = model.UnitId.GetValueOrDefault();
        item.ShortCode = model.Shortcode;
        item.Description = model.Description;
        item.IsAvailable = model.IsAvailable;
        item.IsDefaultTax = model.IsDefaultTax;
        item.TaxPercentage = model.TaxPercentage.GetValueOrDefault();
        item.IsFavourite = model.IsFavourite;
        item.ProfileImage = model.Imageurl;
        item.ModifiedAt = DateTime.Now;

        IEnumerable<MappingMenuItemWithModifier> existingModifierGroupsForItem = await _itemRepository.GetByItemId(model.Id);

        List<MappingMenuItemWithModifier> modifierGroupsForItemToAdd = new();
        List<MappingMenuItemWithModifier> modifierGroupsForItemToRemove = new();
        List<MappingMenuItemWithModifier> modifierGroupsForItemToUpdate = new();

        foreach (ModifierGrouopForItem modifierGroupForItem in model.SelectedModifierGroups)
        {
            if (!existingModifierGroupsForItem.Any(mg => modifierGroupForItem.Id == mg.ModifierGroupId))
            {
                MappingMenuItemWithModifier m = new()
                {
                    MenuItemId = item.Id,
                    ModifierGroupId = modifierGroupForItem.Id,
                    MinSelectionRequired = modifierGroupForItem.MinSelection,
                    MaxSelectionRequired = modifierGroupForItem.MaxSelection,
                };

                modifierGroupsForItemToAdd.Add(m);
            }
        }

        foreach (MappingMenuItemWithModifier itemsAndModifierGroup in existingModifierGroupsForItem)
        {
            if (!model.SelectedModifierGroups.Any(mg => mg.Id == itemsAndModifierGroup.ModifierGroupId))
            {
                modifierGroupsForItemToRemove.Add(itemsAndModifierGroup);
            }
            else
            {
                ModifierGrouopForItem updatedData = model.SelectedModifierGroups.Single(mg => mg.Id == itemsAndModifierGroup.ModifierGroupId);
                itemsAndModifierGroup.MinSelectionRequired = updatedData.MinSelection;
                itemsAndModifierGroup.MaxSelectionRequired = updatedData.MaxSelection;

                modifierGroupsForItemToUpdate.Add(itemsAndModifierGroup);
            }
        }

        await _itemRepository.AddRangeAsync(modifierGroupsForItemToAdd);
        await _itemRepository.RemoveRangeAsync(modifierGroupsForItemToRemove);
        await _itemRepository.UpdateRangeOfMapping(modifierGroupsForItemToUpdate);


        await _itemRepository.UpdateAsync(item);
        return Response<ItemVM>.SuccessResponse(model, "Item Updated Successfully!");


    }


    public async Task<Response<bool>> DeleteItemAsync(int id)
    {

        var item = await _itemRepository.GetByIdAsync(id);

        if (item == null)
        {
            return Response<bool>.FailureResponse("Item Does Not Exists");
        }

        item.IsDeleted = true;
        await _itemRepository.UpdateAsync(item);
        return Response<bool>.SuccessResponse(true, "Item Deleted Successfully!");


    }

    public async Task<Response<IEnumerable<UnitVM>>> GetAllUnitsAsync()
    {

        IEnumerable<Unit> units = await _unitRepository.GetAllAsync();
        IEnumerable<UnitVM> unitlist = units.Select(u => new UnitVM()
        {
            Id = u.Id,
            Name = u.Name,

        }).ToList();

        return Response<IEnumerable<UnitVM>>.SuccessResponse(unitlist, "Unit list fetched successfully!");

    }

    public async Task<bool> DeleteManyItemAsync(IEnumerable<int> ids)
    {

        List<MenuItem> itemsToDelete = new();
        foreach (int id in ids)
        {
            var item = await _itemRepository.GetByIdAsync(id);

            if (item != null)
            {
                item.IsDeleted = true;
                itemsToDelete.Add(item);
            }

        }
        await _itemRepository.UpdateRangeAsync(itemsToDelete);
        return true;

    }


    public async Task<Response<ModifierGroupVM?>> GetModifierGroupByIdAsync(int id)
    {

        ModifierGroup? modifierGroup = await _modifierGroupRepository.GetByIdAsync(id);
        if (modifierGroup == null)
        {
            return Response<ModifierGroupVM?>.FailureResponse("Modifier group not found!");
        }

        ModifierGroupVM modifierGroupVM = new()
        {
            Id = modifierGroup.Id,
            Name = modifierGroup.Name,
            Description = modifierGroup.Description,
        };

        return Response<ModifierGroupVM?>.SuccessResponse(modifierGroupVM, "Modifier Group Fetched Successfully!");

    }

    public async Task<Response<IEnumerable<ModifierGroupVM>>> GetModifierGroupsByModifierIdAsync(int modifierId)
    {

        IEnumerable<ModifierGroup> modifierGroups = await _modifierAndGroupRepository.GetModifierGroupsByModifierIdAsync(modifierId);
        IEnumerable<ModifierGroupVM> modifierGroupList = modifierGroups.Select(mg => new ModifierGroupVM()
        {
            Id = mg.Id,
            Name = mg.Name,
            Description = mg.Description,

        });
        return Response<IEnumerable<ModifierGroupVM>>.SuccessResponse(modifierGroupList, "Modifier List fetched successfully!");

    }

    public async Task<Response<IEnumerable<ModifierGroupVM>>> GetAllModifierGroupsAsync()
    {

        IEnumerable<ModifierGroup> modifierGroups = await _modifierGroupRepository.GetAllAsync();
        IEnumerable<ModifierGroupVM> modifierGroupList = modifierGroups.Select(c => new ModifierGroupVM()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList();

        return Response<IEnumerable<ModifierGroupVM>>.SuccessResponse(modifierGroupList, "Category Fetched Successfully!");

    }

    public async Task<Response<ModifierGroupVM?>> AddModifierGroupAsync(ModifierGroupVM model, List<int> modifierIds)
    {

        var IsExist = await _modifierGroupRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<ModifierGroupVM>.FailureResponse("Modifier Group Already Exists");
        }

        ModifierGroup modifierGroup = new()
        {
            // Id = model.Id,
            Name = model.Name,
            Description = model.Description,

        };

        await _modifierGroupRepository.AddAsync(modifierGroup);

        List<ModifierAndGroup> modifiersAndGroups = new();

        foreach (int id in modifierIds)
        {
            ModifierAndGroup modifierAndGroup = new()
            {
                ModifiergroupId = modifierGroup.Id,
                ModifierId = id,
            };

            modifiersAndGroups.Add(modifierAndGroup);
        }

        await _modifierAndGroupRepository.AddRangeAsync(modifiersAndGroups);
        return Response<ModifierGroupVM?>.SuccessResponse(model, "Modifier Group Added Successfully!");

    }

    public async Task<Response<ModifierGroupVM?>> EditModifierGroupAsync(ModifierGroupVM model, List<int> modifierIds)
    {
        var IsExist = await _modifierGroupRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && !(bool)u.IsDeleted! && u.Id != model.Id);
        if (IsExist)
        {
            return Response<ModifierGroupVM>.FailureResponse("Modifier Group Already Exists");
        }

        ModifierGroup? modifierGroup = await _modifierGroupRepository.GetByIdAsync(model.Id);
        if (modifierGroup == null)
        {
            return Response<ModifierGroupVM?>.FailureResponse("Modifier Group Not Found");
        }

        modifierGroup.Name = model.Name;
        modifierGroup.Description = model.Description;


        IEnumerable<ModifierAndGroup> existingModifiers = await _modifierAndGroupRepository.GetModifiersAndGroupByGroupIdAsync(model.Id);

        List<ModifierAndGroup> modifiersToAddInGroup = new();
        List<ModifierAndGroup> modifiersToDeleteInGroup = new();

        foreach (int id in modifierIds)
        {

            if (!existingModifiers.Any(mg => mg.ModifierId == id))
            {
                ModifierAndGroup modifiersAndGroup = new()
                {
                    ModifiergroupId = modifierGroup.Id,
                    ModifierId = id,
                };

                modifiersToAddInGroup.Add(modifiersAndGroup);
            }
        }

        foreach (ModifierAndGroup modifier in existingModifiers)
        {
            if (!modifierIds.Any(id => id == modifier.ModifierId))
            {
                modifiersToDeleteInGroup.Add(modifier);
            }
        }

        await _modifierAndGroupRepository.AddRangeAsync(modifiersToAddInGroup);
        await _modifierAndGroupRepository.DeleteRangeAsync(modifiersToDeleteInGroup);

        await _modifierGroupRepository.UpdateAsync(modifierGroup);

        return Response<ModifierGroupVM?>.SuccessResponse(model, "Modifier Edited Successfylly!");


    }

    public async Task<Response<bool>> DeleteModifierGroupAsync(int id)
    {

        ModifierGroup? modifierGroup = await _modifierGroupRepository.GetByIdAsync(id);
        if (modifierGroup == null)
        {
            return Response<bool>.FailureResponse("Modifier Group Not Found!");
        }

        IEnumerable<ModifierAndGroup> modifiersAndGroups = await _modifierAndGroupRepository.GetModifiersAndGroupByGroupIdAsync(id);

        await _modifierAndGroupRepository.DeleteRangeAsync(modifiersAndGroups);

        modifierGroup.IsDeleted = true;

        await _modifierGroupRepository.UpdateAsync(modifierGroup);

        return Response<bool>.SuccessResponse(true, "Modifier Group Successfylly Deleted!");

    }


    public async Task<Response<IEnumerable<ModifierListVM>>> GetModifiersByGroupIdAsync(int modifierGroupId)
    {

        IEnumerable<Modifier> modifiers = await _modifierAndGroupRepository.GetModifiersByGroupIdAsync(modifierGroupId);
        IEnumerable<ModifierListVM> modifierlist = modifiers.Select(m => new ModifierListVM()
        {
            Id = m.Id,
            Name = m.Name,
            Rate = m.Rate,
            Quantity = m.Quantity.GetValueOrDefault(),
            Unit = m.Unit.Name
        });
        return Response<IEnumerable<ModifierListVM>>.SuccessResponse(modifierlist, "Modifier List fetched successfully!");

    }

    public async Task<Response<IEnumerable<ModifierListVM>>> GetAllModifiersAsync()
    {

        IEnumerable<Modifier> modifiers = await _modifierRepository.GetAllAsync();
        IEnumerable<ModifierListVM> modifierLists = modifiers.Select(m => new ModifierListVM()
        {
            Id = m.Id,
            Name = m.Name,
            Unit = m.Unit.Name,
            Rate = m.Rate,
            Quantity = m.Quantity.GetValueOrDefault()
        }).ToList();

        return Response<IEnumerable<ModifierListVM>>.SuccessResponse(modifierLists, "modifier Fetched Successfully!");

    }

    public async Task<Response<PagedResult<ModifierListVM>>> GetAllExistingModifiers(string searchStringFor, int page, int pagesize)
    {

        (IEnumerable<Modifier> modifiers, int totalRecords) = await _modifierRepository.GetPagedExistingModifiers(searchStringFor, page, pagesize);
        IEnumerable<ModifierListVM> modifierLists = modifiers.Select(m => new ModifierListVM()
        {
            Id = m.Id,
            Name = m.Name,
            Unit = m.Unit.Name,
            Rate = m.Rate,
            Quantity = m.Quantity.GetValueOrDefault()

        }).ToList();

        PagedResult<ModifierListVM> pagedResult = new()
        {
            PagedList = modifierLists
        };

        pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

        return Response<PagedResult<ModifierListVM>>.SuccessResponse(pagedResult, "Modifier list fetched successfully!");

    }



    public async Task<Response<PagedResult<ModifierListVM>>> GetPagedMofiersAsync(int modifierGroupId, string searchString, int page, int pagesize, bool isASC)
    {

        (IEnumerable<Modifier> modifiers, int totalRecords) = await _modifierAndGroupRepository.GetPagedModifiersAsync(modifierGroupId, searchString, page, pagesize, isASC);

        IEnumerable<ModifierListVM> modifierlist = modifiers.Select(m => new ModifierListVM()
        {
            Id = m.Id,
            Name = m.Name,
            Rate = m.Rate,
            Quantity = m.Quantity.GetValueOrDefault(),
            Unit = m.Unit?.Name
        }).ToList();

        PagedResult<ModifierListVM> pagedResult = new()
        {
            PagedList = modifierlist
        };

        pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

        return Response<PagedResult<ModifierListVM>>.SuccessResponse(pagedResult, "Modifier list fetched successfully!");

    }
    public async Task<Response<ModifierVM?>> GetModifierByIdAsync(int id)
    {

        Modifier? modifier = await _modifierRepository.GetByIdAsync(id);
        if (modifier == null)
        {
            return Response<ModifierVM?>.FailureResponse("Modifier Not Found!");
        }

        ModifierVM modifierVM = new()
        {
            Id = modifier.Id,
            Name = modifier.Name,
            Rate = modifier.Rate,
            Quantity = modifier.Quantity,
            UnitId = modifier.UnitId,
            Description = modifier.Description
        };

        return Response<ModifierVM?>.SuccessResponse(modifierVM, "Modifier Fetched Successfully!");

    }

    public async Task<Response<ModifierVM?>> AddModifierAsync(ModifierVM model, List<int> modifierGroups)
    {

        Modifier modifier = new()
        {
            Name = model.Name,
            Rate = model.Rate.GetValueOrDefault(),
            Quantity = model.Quantity.GetValueOrDefault(),
            UnitId = model.UnitId.GetValueOrDefault(),
            Description = model.Description,

        };

        await _modifierRepository.AddAsync(modifier);

        List<ModifierAndGroup> modifiersAndGroupsToAdd = new();
        foreach (int id in modifierGroups)
        {
            ModifierAndGroup modifiersAndGroup = new()
            {
                ModifiergroupId = id,
                ModifierId = modifier.Id,

            };

            modifiersAndGroupsToAdd.Add(modifiersAndGroup);
        }

        await _modifierAndGroupRepository.AddRangeAsync(modifiersAndGroupsToAdd);

        return Response<ModifierVM?>.SuccessResponse(model, "Modifier Added Successfylly!");

    }

    public async Task<Response<ModifierVM?>> EditModfierAsync(ModifierVM model, List<int> modifierGroups)
    {

        Modifier? modifier = await _modifierRepository.GetByIdAsync(model.Id);
        if (modifier == null)
        {
            return Response<ModifierVM?>.FailureResponse("Modifier Not Found!");
        }

        modifier.Name = model.Name;
        modifier.Rate = model.Rate.GetValueOrDefault();
        modifier.Quantity = model.Quantity.GetValueOrDefault();
        modifier.UnitId = model.UnitId.GetValueOrDefault();
        modifier.Description = model.Description;


        IEnumerable<ModifierAndGroup> existingModifierGroups = await _modifierAndGroupRepository.GetModifiersAndGroupByModifierIdAsync(model.Id);

        List<ModifierAndGroup> modifierGroupsToAddInGroup = new();
        List<ModifierAndGroup> modifierGroupsToDeleteInGroup = new();

        foreach (int id in modifierGroups)
        {

            if (!existingModifierGroups.Any(mg => mg.ModifiergroupId == id))
            {
                ModifierAndGroup modifiersAndGroup = new()
                {
                    ModifiergroupId = id,
                    ModifierId = modifier.Id,

                };

                modifierGroupsToAddInGroup.Add(modifiersAndGroup);
            }
        }

        foreach (ModifierAndGroup modifierAndGroup in existingModifierGroups)
        {
            if (!modifierGroups.Any(id => id == modifierAndGroup.ModifiergroupId))
            {
                modifierGroupsToDeleteInGroup.Add(modifierAndGroup);
            }
        }

        await _modifierAndGroupRepository.AddRangeAsync(modifierGroupsToAddInGroup);
        await _modifierAndGroupRepository.DeleteRangeAsync(modifierGroupsToDeleteInGroup);

        await _modifierRepository.UpdateAsync(modifier);

        return Response<ModifierVM?>.SuccessResponse(model, "Modifier Edited Successfylly!");

    }

    public async Task<Response<bool>> DeleteModifierAsync(int id)
    {

        Modifier? modifier = await _modifierRepository.GetByIdAsync(id);
        if (modifier == null)
        {
            return Response<bool>.FailureResponse("Modifier Not Found!");
        }

        IEnumerable<ModifierAndGroup> modifiersAndGroups = await _modifierAndGroupRepository.GetModifiersAndGroupByModifierIdAsync(id);

        await _modifierAndGroupRepository.DeleteRangeAsync(modifiersAndGroups);

        modifier.IsDeleted = true;




        await _modifierRepository.UpdateAsync(modifier);

        return Response<bool>.SuccessResponse(true, "Modifier Successfylly Deleted!");

    }

    public async Task<Response<bool>> DeleteManyModifierAsync(IEnumerable<int> ids)
    {

        foreach (int id in ids)
        {
            Response<bool> response = await DeleteModifierAsync(id);
        }

        return Response<bool>.SuccessResponse(true, "Selected Modifiers deleted successfully!!");

    }



    public async Task<Response<IEnumerable<ItemListVM>>> GetAvailableItemsByCategoryIdAsync(int categoryId)
    {

        IEnumerable<MenuItem> items;
        if (categoryId != 0)
        {
            items = await _itemRepository.GetAvailableByCategoryIdAsync(categoryId);
        }
        else
        {
            items = await _itemRepository.GetAllAvailableAsync();
        }

        IEnumerable<ItemListVM> itemlist = items.Select(i => new ItemListVM()
        {
            Id = i.Id,
            Name = i.Name,
            Type = i.ItemType,
            Rate = i.Rate,
            Quantity = i.Quantity,
            Imageurl = i.ProfileImage,
            IsAvailable = i.IsAvailable.GetValueOrDefault(),
            IsFavourite = i.IsFavourite,
            TaxPercentage = i.TaxPercentage.GetValueOrDefault()
        }).ToList();
        return Response<IEnumerable<ItemListVM>>.SuccessResponse(itemlist, "Item list fetched");

    }

    public async Task<Response<bool>> ToggleFavoriteItemAsync(int itemId)
    {

        MenuItem? item = await _itemRepository.GetByIdAsync(itemId);
        if (item == null)
        {
            return Response<bool>.FailureResponse("Item does not found");
        }

        item.IsFavourite = !item.IsFavourite.GetValueOrDefault();
        await _itemRepository.UpdateAsync(item);

        return Response<bool>.SuccessResponse(true, "Item Status changed");

    }

    public async Task<Response<IEnumerable<ItemListVM>>> GetFavoriteItemsAsync()
    {

        IEnumerable<MenuItem> items = await _itemRepository.GetFavoriteItemsAsync();

        IEnumerable<ItemListVM> itemlist = items.Select(i => new ItemListVM()
        {
            Id = i.Id,
            Name = i.Name,
            Type = i.ItemType,
            Rate = i.Rate,
            Quantity = i.Quantity,
            Imageurl = i.ProfileImage,
            IsAvailable = i.IsAvailable.GetValueOrDefault(),
            IsFavourite = i.IsFavourite,
            TaxPercentage = i.TaxPercentage.GetValueOrDefault()
        }).ToList();
        return Response<IEnumerable<ItemListVM>>.SuccessResponse(itemlist, "Favorite Items fetched");

    }

    public async Task<Response<IEnumerable<ModifierGroupForItemVM>>> GetApplicableModifiersForItem(int itemId)
    {

        IEnumerable<MappingMenuItemWithModifier> itemsAndModifierGroups = await _itemAndModifierGroupRepository.GetApplicableModifierByItemId(itemId);

        IEnumerable<ModifierGroupForItemVM> modifierGroupsForItem = itemsAndModifierGroups.Select(mg => new ModifierGroupForItemVM()
        {
            Id = mg.ModifierGroupId.GetValueOrDefault(),
            Name = mg.ModifierGroup.Name,
            MinSelection = mg.MinSelectionRequired.GetValueOrDefault(),
            MaxSelection = mg.MaxSelectionRequired.GetValueOrDefault(),
            Modifiers = mg.ModifierGroup.ModifierAndGroups.Select(m => new ModifierListVM()
            {
                Id = m.Modifier.Id,
                Name = m.Modifier.Name,
                Rate = m.Modifier.Rate
            }).ToList(),
        });

        return Response<IEnumerable<ModifierGroupForItemVM>>.SuccessResponse(modifierGroupsForItem, "Modifier Groups For Item fetched");

    }




}






