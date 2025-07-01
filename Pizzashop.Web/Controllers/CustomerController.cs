using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Attributes;

namespace Pizzashop.Web.Controllers;

[Authorize]
[PermissionAuthorize(Constants.CanView)]
public class CustomerController : Controller
{

    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {

        _customerService = customerService;
    }
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetPagedCustomers(DateOnly? fromDate, DateOnly? toDate, string searchString = "", int page = 1, int pagesize = 5, bool isASC = true, string sortColumn = " ")
    {

        Response<PagedResult<CustomerListVM>> response = await _customerService.GetPagedCustomerAsync(fromDate, toDate, searchString, page, pagesize, isASC, sortColumn);
        if (!response.Success)
        {
            return PartialView("_Customer");
        }

        PagedResult<CustomerListVM> model = response.Data!;
        return PartialView("_Customer", model);

    }

    public async Task<IActionResult> GetCustomerHistory(int id)
    {


        var response = await _customerService.GetCustomerHistory(id);

        if (!response.Success)
        {
            return PartialView("_CustomerHistory");
        }

        return PartialView("_CustomerHistory", response.Data);

    }

    public async Task<IActionResult> ExportCustomers(string pastDays = "all", string searchString = "", string account = "")
    {
        DateOnly? fromDate = null, toDate = null;
        DateTime today = DateTime.Today;

        if (pastDays.Equals("1"))
        {
            fromDate = DateOnly.FromDateTime(today);
            toDate = DateOnly.FromDateTime(today);
        }
        else if (pastDays.Equals("7"))
        {
            toDate = DateOnly.FromDateTime(today);
            fromDate = DateOnly.FromDateTime(today.AddDays(-7));
        }
        else if (pastDays.Equals("30"))
        {
            toDate = DateOnly.FromDateTime(today);
            fromDate = DateOnly.FromDateTime(today.AddDays(-30));
        }
        else if (pastDays.Equals("month"))
        {
            fromDate = new DateOnly(today.Year, today.Month, 1);
            toDate = DateOnly.FromDateTime(today);
        }
        else if (pastDays.Equals("customdate"))
        {
            // This case should be handled by the fromDate and toDate parameters
            // which will come from the custom date picker
        }


        Response<MemoryStream> fileStream = await _customerService.ExportCustomer(searchString, account, fromDate, toDate);

        if (fileStream.Success)
        {
            return File(fileStream.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customers.xlsx");
        }

        TempData["error"] = fileStream.Message;
        return View("Index");

    }

}
