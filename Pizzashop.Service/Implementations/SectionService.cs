using Microsoft.AspNetCore.Http;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class SectionService : ISectionService
{
    private readonly ISectionRepository _sectionRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ITokenEmailService _tokenEmailService;
    private readonly ITableOrderMappingRepository _tableOrderMapping;


    public SectionService(ISectionRepository sectionRepository, ITableRepository tableRepository, ITokenEmailService tokenEmailService, ITableOrderMappingRepository tableOrderMapping)
    {
        _sectionRepository = sectionRepository;
        _tableRepository = tableRepository;
        _tokenEmailService = tokenEmailService;
        _tableOrderMapping = tableOrderMapping;

    }

    public async Task<Response<IEnumerable<SectionVM>>> GetAllSectionAsync()
    {

        IEnumerable<Section> section = await _sectionRepository.GetAllAsync();

        IEnumerable<SectionVM> sectionList = section.Select(c => new SectionVM()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            TokenCount = c.WaitingTokens.Count(c => !(bool)c.IsAssigned && !(bool)c.IsDeleted)

        }).ToList();

        return Response<IEnumerable<SectionVM>>.SuccessResponse(sectionList, "section Fetched Successfully!");

    }
    public async Task<Response<IEnumerable<TableVM>>> GetAllTableVM(int sectionId)
    {

        IEnumerable<Table> table = await _tableRepository.GetTableBySectionIdForAssign(sectionId);

        IEnumerable<TableVM> tableList = table.Select(c => new TableVM()
        {
            Id = c.Id,
            Name = c.Name,
            Capacity = c.Capacity
            
        }).ToList();

        return Response<IEnumerable<TableVM>>.SuccessResponse(tableList, "table Fetched Successfully!");

    }

    public async Task<Response<PagedResult<TableListVM>>> GetPagedItemsAsync(int SectionId, string searchString, int page, int pagesize, bool isASC)
    {

        (IEnumerable<Table> items, int totalRecords) = await _tableRepository.GetPagedItemsAsync(SectionId, searchString, page, pagesize, isASC);

        IEnumerable<TableListVM> itemlist = items.Select(i => new TableListVM()
        {
            Id = i.Id,
            SectionId = i.SectionId,
            Name = i.Name,
            Capacity = i.Capacity,
            IsAvailable = i.IsAvailable

        }).ToList();

        PagedResult<TableListVM> pagedResult = new()
        {
            PagedList = itemlist
        };

        pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

        return Response<PagedResult<TableListVM>>.SuccessResponse(pagedResult, "Item list fetched successfully!");

    }

    public async Task<Response<SectionVM?>> AddSection(SectionVM model, HttpRequest request)
    {
        var token = request.Cookies["AuthToken"];
        var id = _tokenEmailService.GetIdFromToken(token!);

        var IsExist = await _sectionRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<SectionVM?>.FailureResponse("Section Already Exists");
        }

        Section section = new()
        {
            Name = model.Name,
            Description = model.Description,
            CreatedAt = DateTime.Now,
            CreatedBy = id
        };

        await _sectionRepository.AddAsync(section);
        return Response<SectionVM?>.SuccessResponse(model, "Section Added Successfully!");

    }

    public async Task<Response<SectionVM?>> EditSection(SectionVM model, HttpRequest request)
    {

        var token = request.Cookies["AuthToken"];
        var id = _tokenEmailService.GetIdFromToken(token!);

        var IsExist = await _sectionRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && u.Id != model.Id && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<SectionVM?>.FailureResponse("Section Already Exists");
        }

        var section = await _sectionRepository.GetByIdAsync(model.Id);
        if (section == null)
        {
            return Response<SectionVM?>.FailureResponse("Section is not Exists");
        }

        section.Id = model.Id;
        section.Name = model.Name;
        section.Description = model.Description;
        section.ModifiedBy = id;
        section.ModifiedAt = DateTime.Now;

        await _sectionRepository.UpdateAsync(section);
        return Response<SectionVM?>.SuccessResponse(model, "Section Edited Successfully!");

    }


    public async Task<Response<bool>> DeleteSection(int id, HttpRequest request)
    {
        var token = request.Cookies["AuthToken"];
        var deletetorId = _tokenEmailService.GetIdFromToken(token!);

        var section = await _sectionRepository.GetByIdAsync(id);
        if (section == null)
        {
            return Response<bool>.FailureResponse("Section is not Exists");
        }

        IEnumerable<Table> itemsToDelete = await _tableRepository.GetTablesBySectionId(id);

        foreach (Table item in itemsToDelete)
        {
            if (!item.IsAvailable.GetValueOrDefault())
            {
                return Response<bool>.FailureResponse("There Is Atleasst One Table is Occupid In This Section");
            }
        }

        foreach (Table item in itemsToDelete)
        {
            item.IsDeleted = true;
        }

        await _tableRepository.UpdateRangeAsync(itemsToDelete);

        section.IsDeleted = true;
        section.ModifiedBy = deletetorId;
        section.ModifiedAt = DateTime.Now;

        await _sectionRepository.UpdateAsync(section);
        return Response<bool>.SuccessResponse(true, "Section Deleted Successfully!");
    }

    public async Task<TableVM?> GetTableByIdAsync(int id)
    {

        var item = await _tableRepository.GetByIdAsync(id);
        if (item == null)
        {
            return null;
        }

        TableVM tableVM = new()
        {
            Id = item.Id,
            SectionId = item.SectionId,
            Name = item.Name,
            Capacity = item.Capacity,
            IsAvailable = item.IsAvailable.GetValueOrDefault(),
        };

        return tableVM;

    }

    public async Task<Response<TableVM>> AddTables(TableVM model, HttpRequest request)
    {
        var token = request.Cookies["AuthToken"];
        var id = _tokenEmailService.GetIdFromToken(token!);

        var IsExist = await _tableRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && u.SectionId == model.SectionId && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<TableVM>.FailureResponse("Table Already Exists");
        }

        Table item = new()
        {
            Id = model.Id,
            SectionId = model.SectionId.GetValueOrDefault(),
            Name = model.Name,
            Capacity = model.Capacity,
            IsAvailable = model.IsAvailable,
            CreatedAt = DateTime.Now,
            CreatedBy = id,


        };

        await _tableRepository.AddAsync(item);

        return Response<TableVM>.SuccessResponse(model, "Table Added Successfully!");

    }

    public async Task<Response<TableVM>> EditTables(TableVM model, HttpRequest request)
    {

        var token = request.Cookies["AuthToken"];
        var id = _tokenEmailService.GetIdFromToken(token!);

       

        var IsExist = await _tableRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && u.Id != model.Id && u.SectionId == model.SectionId && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<TableVM>.FailureResponse("Table Already Exists");
        }

        var item = await _tableRepository.GetByIdAsync(model.Id);

        if (!(bool)item.IsAvailable)
        {
            return Response<TableVM>.FailureResponse("Table is Currently Occupied ! you can't Edit");
        }
        if (item == null)
        {
            return Response<TableVM>.FailureResponse("Table Does Not Exists");
        }

        item.SectionId = model.SectionId.GetValueOrDefault();
        item.Name = model.Name;
        item.Capacity = model.Capacity;
        item.IsAvailable = model.IsAvailable;
        item.ModifiedAt = DateTime.Now;
        item.ModifiedBy = id;


        await _tableRepository.UpdateAsync(item);
        return Response<TableVM>.SuccessResponse(model, "Table Edited Successfully!");

    }

    public async Task<Response<bool>> DeleteTable(int id, HttpRequest request)
    {

        var token = request.Cookies["AuthToken"];
        var deletetorId = _tokenEmailService.GetIdFromToken(token!);

        var item = await _tableRepository.GetByIdAsync(id);

        if (item == null)
        {
            return Response<bool>.FailureResponse("Table Does Not Exists");
        }

        if (!item.IsAvailable.GetValueOrDefault())
        {
            return Response<bool>.FailureResponse("Table is occupid currently");
        }

        item.IsDeleted = true;
        item.ModifiedBy = deletetorId;
        item.ModifiedAt = DateTime.Now;

        await _tableRepository.UpdateAsync(item);
        return Response<bool>.SuccessResponse(true, "Table Deleted Successfully!");


    }

    public async Task<Response<bool>> DeleteManyTableAsync(IEnumerable<int> ids, HttpRequest request)
    {
        var token = request.Cookies["AuthToken"];
        var deletetorId = _tokenEmailService.GetIdFromToken(token!);

        List<Table> itemsToDelete = new();

        foreach (int id in ids)
        {
            var item = await _tableRepository.GetByIdAsync(id);

            if (item != null)
            {
                if (!item.IsAvailable.GetValueOrDefault())
                {
                    return Response<bool>.FailureResponse("There Is Atleasst One Table is Occupid In Selected Tables");
                }

                item.IsDeleted = true;
                item.ModifiedBy = deletetorId;
                item.ModifiedAt = DateTime.Now;

                itemsToDelete.Add(item);

            }

        }
        await _tableRepository.UpdateRangeAsync(itemsToDelete);
        return Response<bool>.SuccessResponse(true, "Tables Deleted Successfully!");

    }
}
