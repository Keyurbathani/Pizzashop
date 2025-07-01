using ClosedXML.Excel;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Response<PagedResult<CustomerListVM>>> GetPagedCustomerAsync(DateOnly? fromDate, DateOnly? toDate, string searchString, int page, int pagesize, bool isASC, string sortColumn)
    {

        (IEnumerable<Customer> items, int totalRecords) = await _customerRepository.GetPagedCustomerAsync(fromDate, toDate, searchString, page, pagesize, isASC, sortColumn);

        IEnumerable<CustomerListVM> itemlist = items.Select(i => new CustomerListVM()
        {
            Id = i.Id,
            Name = i.Name,
            Email = i.Email,
            Phone = i.Phone,
            Date = DateOnly.FromDateTime(i.CreatedAt.GetValueOrDefault()),
            TotalOrder = i.Orders.Count(cd => cd.CustomerId == i.Id)

        }).ToList();

        PagedResult<CustomerListVM> pagedResult = new()
        {
            PagedList = itemlist
        };

        pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

        return Response<PagedResult<CustomerListVM>>.SuccessResponse(pagedResult, "Customer list fetched successfully!");

    }

    public async Task<Response<CustomerHistoryVM>> GetCustomerHistory(int customerId)
    {

        var customer = await _customerRepository.GetCustomerHistory(customerId);

        if (customer == null)
        {
            return Response<CustomerHistoryVM>.FailureResponse("customer Details does not exist!");
        }

        var model = new CustomerHistoryVM
        {
            Id = customer.Id,
            Name = customer.Name,
            MobileNumber = customer.Phone,
            MaxOrder = customer.Orders.Max(i => i.TotalAmount),
            AverageBill = customer.Orders.Average(o => o.TotalAmount),
            CommingSince = customer.Orders.Where(cd => cd.CustomerId == customer.Id).Min(i => i.CreatedAt),
            Visits = customer.Orders.Count(cd => cd.CustomerId == customer.Id),

            customerHistories = customer.Orders.Select(o => new CustomerHistory
            {

                OrderDate = DateOnly.FromDateTime(o.CreatedAt),
                OrderType = o.OrderStatus,
                Payment = o.Invoices.FirstOrDefault()?.Payments.FirstOrDefault()?.PaymentMethod,
                Items = o.OrderedItems.Count(),
                Amount = o.TotalAmount,

            }).ToList(),

        };

        return Response<CustomerHistoryVM>.SuccessResponse(model, "customer details fetched Successfully!");

    }

    public async Task<Response<MemoryStream>> ExportCustomer(string searchQuery, string account, DateOnly? fromDate, DateOnly? toDate)
    {
        try
        {


            (IEnumerable<Customer> customers, int totalRecords) = await _customerRepository.GetCustomers(searchQuery, fromDate, toDate);

            if (totalRecords == 0)
            {
                return Response<MemoryStream>.FailureResponse(" There isn't Any Customers To Export"); ;

            }

            IEnumerable<CustomerListVM> customerLists = customers.Select(c => new CustomerListVM()
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Date = DateOnly.FromDateTime(c.CreatedAt.GetValueOrDefault()),
                TotalOrder = c.Orders.Where(cd => cd.CustomerId == c.Id).Count()
            }).ToList();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CustomerTemplate.xlsx");
            using var workbook = new XLWorkbook(path);
            IXLWorksheet worksheet = workbook.Worksheet("Customers");

            worksheet.Cell(2, 3).Value = account;
            worksheet.Cell(2, 10).Value = searchQuery;

            if (fromDate != null & toDate != null)
            {
                worksheet.Cell(5, 3).Value = fromDate.ToString() + " to " + toDate.ToString();
            }
            else
            {
                worksheet.Cell(5, 3).Value = "All Time";
            }

            worksheet.Cell(5, 10).Value = totalRecords;

            int row = 10;
            foreach (var customer in customerLists)
            {
                int col = 1;
                worksheet.Cell(row, col).Value = customer.Id;
                worksheet.Cell(row, col++).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, col).Value = customer.Name;
                worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, ++col).Value = customer.Email;
                worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 3)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, 9).Value = customer.Date.ToString();
                worksheet.Range(worksheet.Cell(row, ++col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, ++col).Value = customer.Phone;
                worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, ++col).Value = customer.TotalOrder;
                worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 1)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                row++;
            }

            MemoryStream stream = new();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return Response<MemoryStream>.SuccessResponse(stream, "Customers Exported Successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Response<MemoryStream>.FailureResponse(" Error in Customers Export"); ;
        }
    }

    
}
