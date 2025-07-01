using Pizzashop.Entity;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class OrderAppTableService : IOrderAppTableService
{
    private readonly IOrderAppTableRepository _orderAppTableRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly ITableOrderMappingRepository _tableMapping;
    private readonly ITableRepository _tableRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IFeedbackRepository _feedbackRepository;

    private readonly ISectionRepository _sectionRepository;


    public OrderAppTableService(IOrderAppTableRepository orderAppTableRepository, IOrdersRepository ordersRepository, ITableOrderMappingRepository tableMapping, ITableRepository tableRepository, IInvoiceRepository invoiceRepository, IPaymentRepository paymentRepository, IFeedbackRepository feedbackRepository, ISectionRepository sectionRepository)
    {
        _orderAppTableRepository = orderAppTableRepository;
        _ordersRepository = ordersRepository;
        _tableMapping = tableMapping;
        _tableRepository = tableRepository;
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _feedbackRepository = feedbackRepository;
        _sectionRepository = sectionRepository;
    }

    public async Task<Response<IEnumerable<OrderAppSectionListVM>>> GetTablesForOrderApp()
    {

        IEnumerable<Section> sections = await _sectionRepository.GetAllSectionForApp();

        if (sections == null)
        {
            return Response<IEnumerable<OrderAppSectionListVM>>.FailureResponse("Sections does not exists");
        }

        IEnumerable<OrderAppSectionListVM?> sectionsList = sections.Select(i => new OrderAppSectionListVM
        {
            SectionId = i.Id,
            SectionName = i.Name,
            IsAvailableCount = i.Tables.Where(t => t.IsDeleted == false && t.IsAvailable == true).Count(),
            IsAssignedCount = i.Tables.Where(t => t.IsDeleted == false && t.IsAvailable == false).SelectMany(t => t.TableOrderMappings).Count(m => m.Order.OrderStatus == "Pending"),
            IsRunningCount = i.Tables.Where(t => t.IsDeleted == false && t.IsAvailable == false).SelectMany(t => t.TableOrderMappings).Count(m => m.Order.OrderStatus == "InProgress" ||m.Order.OrderStatus == "Served"),

            TableLists = i.Tables.Select(t => new TableListNew
            {
                TableId = t.Id,
                TableName = t.Name,
                SectionId = t.SectionId,
                Capacity = t.Capacity,
                Time = t.IsAvailable.GetValueOrDefault() ? null : t.TableOrderMappings.Where(i => i.Order.OrderStatus != "Completed" && i.Order.OrderStatus != "Cancelled").FirstOrDefault()?.CreatedAt ,
                Status = t.TableOrderMappings.Where(i => i.Order.OrderStatus != "Completed" && i.Order.OrderStatus != "Cancelled").FirstOrDefault()?.Order.OrderStatus ?? "Available",
                TotalAmount = t.TableOrderMappings.Where(i => i.Order.OrderStatus != "Completed" && i.Order.OrderStatus != "Cancelled").FirstOrDefault()?.Order.TotalAmount ?? 0,
                OrderId = t.TableOrderMappings.Where(i => i.Order.OrderStatus != "Completed" && i.Order.OrderStatus != "Cancelled").FirstOrDefault()?.Order.Id,

            }).ToList()

        }).ToList();


        return Response<IEnumerable<OrderAppSectionListVM>>.SuccessResponse(sectionsList!, "Fetched");
    }

    
}
