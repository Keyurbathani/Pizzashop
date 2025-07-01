using Microsoft.AspNetCore.Http;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class TaxService : ITaxService
{
    private readonly ITaxRepository _taxRepository;
    private readonly ITokenEmailService _tokenEmailService;

    public TaxService(ITaxRepository taxRepository, ITokenEmailService tokenEmailService)
    {

        _taxRepository = taxRepository;
        _tokenEmailService = tokenEmailService;
    }

    public async Task<Response<PagedResult<TaxListVM>>> GetPagedItemsAsync(string searchString, int page, int pagesize, bool isASC)
    {

        (IEnumerable<TaxAndFee> items, int totalRecords) = await _taxRepository.GetPagedItemsAsync(searchString, page, pagesize, isASC);

        IEnumerable<TaxListVM> itemlist = items.Select(i => new TaxListVM()
        {
            Id = i.Id,
            Name = i.Name,
            TaxType = i.TaxType.GetValueOrDefault(),
            TaxValue = i.TaxValue,
            IsActive = i.IsActive.GetValueOrDefault(),
            IsDefault = i.IsDefault.GetValueOrDefault(),
              
        }).ToList();

        PagedResult<TaxListVM> pagedResult = new()
        {
            PagedList = itemlist
        };

        pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

        return Response<PagedResult<TaxListVM>>.SuccessResponse(pagedResult, "Tax list fetched successfully!");

    }

    public async Task<TaxVM?> GetTaxByIdAsync(int id)
    {

        var item = await _taxRepository.GetByIdAsync(id);
        if (item == null)
        {
            return null;
        }

        TaxVM taxVM = new()
        {
            Id = item.Id,
            Name = item.Name,
            TaxType = item.TaxType.GetValueOrDefault(),
            TaxValue = item.TaxValue,
            IsActive = item.IsActive.GetValueOrDefault(),
            IsDefault = item.IsDefault.GetValueOrDefault()

        };

        return taxVM;


    }

    public async Task<Response<bool>> AddTax(TaxVM model, HttpRequest request)
    {
        var token = request.Cookies["AuthToken"];
        var id = _tokenEmailService.GetIdFromToken(token!);

        var IsExist = await _taxRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<bool>.FailureResponse("Tax Already Exists");
        }

        TaxAndFee item = new()
        {
            Id = model.Id,
            Name = model.Name,
            TaxValue = model.TaxValue,
            IsActive = model.IsActive,
            IsDefault = model.IsDefault,
            TaxType = model.TaxType,
            CreatedAt = DateTime.Now,
            CreatedBy = id,
            ModifiedAt = DateTime.Now,
            ModifiedBy = id
        };

        await _taxRepository.AddAsync(item);

        return Response<bool>.SuccessResponse(true, "Tax Added Succesfully!");

    }

    public async Task<Response<bool>> EditTax(TaxVM model, HttpRequest request)
    {

        var token = request.Cookies["AuthToken"];
        var id = _tokenEmailService.GetIdFromToken(token!);

        var IsExist = await _taxRepository.IsExistsAsync(u => u.Name.ToLower().Trim() == model.Name.ToLower().Trim() && u.Id != model.Id && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<bool>.FailureResponse("Tax Already Exists");
        }

        var item = await _taxRepository.GetByIdAsync(model.Id);

        if (item == null)
        {
            return Response<bool>.FailureResponse("Tax Does Not Exists");
        }


        item.Id = model.Id;
        item.Name = model.Name;
        item.TaxValue = model.TaxValue;
        item.IsActive = model.IsActive;
        item.IsDefault = model.IsDefault;
        item.TaxType = model.TaxType;
        item.ModifiedAt = DateTime.Now;
        item.ModifiedBy = id;

        await _taxRepository.UpdateAsync(item);
        return Response<bool>.SuccessResponse(true, "Tax Updated Succesfully!");

    }

    public async Task<bool> DeleteTax(int id, HttpRequest request)
    {

        var token = request.Cookies["AuthToken"];
        var deletetorId = _tokenEmailService.GetIdFromToken(token!);

        var item = await _taxRepository.GetByIdAsync(id);

        if (item == null)
        {
            return false;
        }

        item.IsDeleted = true;
        item.ModifiedBy = deletetorId;
        item.ModifiedAt = DateTime.Now;

        await _taxRepository.UpdateAsync(item);
        return true;

    }
}
