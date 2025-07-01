using ClosedXML.Excel;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IOrderedItemRepository _orderedItemRepository;
    private readonly ITaxAndFeeRepository _taxAndFeeRepository;

    public OrderService(IOrdersRepository ordersRepository, ITaxAndFeeRepository taxAndFeeRepository, IOrderedItemRepository orderedItemRepository)
    {
        _ordersRepository = ordersRepository;
        _taxAndFeeRepository = taxAndFeeRepository;
        _orderedItemRepository = orderedItemRepository;
    }


    public async Task<Response<PagedResult<OrderListVM>>> GetPagedOrdersAsync(DateOnly? fromDate, DateOnly? toDate, string searchString, int page, int pagesize, bool isASC, string sortColumn, string status)
    {

        (IEnumerable<Order> items, int totalRecords) = await _ordersRepository.GetPagedOrdersAsync(searchString, page, pagesize, isASC, fromDate, toDate, sortColumn, status);

        IEnumerable<OrderListVM> itemlist = items.Select(u => new OrderListVM()
        {
            Order = u.Id,
            Date = DateOnly.FromDateTime(u.CreatedAt),
            CustomerName = u.Customer.Name,
            Status = u.OrderStatus,
            PaymentMode = u.Invoices.FirstOrDefault()?.Payments.FirstOrDefault()?.PaymentMethod,
            Rating = u.Feedbacks.FirstOrDefault()?.AvgRating,
            Totalamount = u.TotalAmount

        }).ToList();



        PagedResult<OrderListVM> pagedResult = new()
        {
            PagedList = itemlist
        };


        pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

        return Response<PagedResult<OrderListVM>>.SuccessResponse(pagedResult, "Orders list fetched successfully!");

    }


    public async Task<Response<MemoryStream>> ExportOrdersAsync(string searchString, string status, DateOnly? fromDate, DateOnly? toDate)
    {
        try
        {
            (IEnumerable<Order> orders, int totalRecords) = await _ordersRepository.GetOrders(searchString, status, fromDate, toDate);

            if (totalRecords == 0)
            {
                return Response<MemoryStream>.FailureResponse(" There isn't Any Orders To Export"); ;

            }


            IEnumerable<OrderListVM> orderLists = orders.Select(u => new OrderListVM()
            {
                Order = u.Id,
                Date = DateOnly.FromDateTime(u.CreatedAt),
                CustomerName = u.Customer.Name,
                Status = u.OrderStatus,
                PaymentMode = u.Invoices.FirstOrDefault()?.Payments.FirstOrDefault()?.PaymentMethod,
                Rating = u.Feedbacks.FirstOrDefault()?.AvgRating,
                Totalamount = u.TotalAmount

            }).ToList();


            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/OrderTemplate.xlsx");
            using var workbook = new XLWorkbook(path);
            IXLWorksheet worksheet = workbook.Worksheet("Orders");

            if (status == "")
            {
                worksheet.Cell(2, 3).Value = "All Status";
            }
            else
            {
                worksheet.Cell(2, 3).Value = status;
            }
            worksheet.Cell(2, 10).Value = searchString;
            if (fromDate != null && toDate != null)
            {
                worksheet.Cell(5, 3).Value = fromDate.ToString() + " to " + toDate.ToString();
            }
            else
            {
                worksheet.Cell(5, 3).Value = "All Time";
            }

            worksheet.Cell(5, 10).Value = totalRecords;

            // Insert Order Data
            int row = 10;
            foreach (var order in orderLists)
            {
                int col = 1;
                worksheet.Cell(row, col).Value = string.Concat("#", order.Order);
                worksheet.Cell(row, col++).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, col).Value = order.Date.ToShortDateString();
                worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, ++col).Value = order.CustomerName;
                worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, 8).Value = order.Status;
                worksheet.Range(worksheet.Cell(row, ++col), worksheet.Cell(row, col += 2)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, ++col).Value = order.PaymentMode;
                worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 1)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, ++col).Value = order.Rating;
                worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 1)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(row, ++col).Value = order.Totalamount;
                worksheet.Range(worksheet.Cell(row, col), worksheet.Cell(row, col += 1)).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                row++;
            }

            // Convert workbook to memory stream
            MemoryStream stream = new();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return Response<MemoryStream>.SuccessResponse(stream, "Orders Exported Successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Response<MemoryStream>.FailureResponse(" Error in Orders Export"); ;
        }

    }


    private async Task<List<OrderTax>> GetOrderTaxes(IEnumerable<OrderTaxMapping> orderTaxes)
    {
        if (orderTaxes == null || !orderTaxes.Any())
        {
            IEnumerable<TaxAndFee> enabledTaxes = await _taxAndFeeRepository.GetEnabledAsync();

            return enabledTaxes.Select(t => new OrderTax()
            {
                Id = t.Id,
                Name = t.Name,
                Rate = t.TaxValue.GetValueOrDefault(),
                Type = t.TaxType.GetValueOrDefault(),
                TotalTax = 0,

            }).ToList();
        }

        return orderTaxes.Select(t => new OrderTax()
        {
            Id = t.TaxId,
            Name = t.Tax.Name,
            Type = t.TaxType.GetValueOrDefault(),
            Rate = t.TaxValue.GetValueOrDefault(),
            TotalTax = t.TotalTax,

        }).ToList();
    }

    public async Task<Response<OrderDetailsVM?>> GetOrderByIdAsync(int id)
    {

        Order? order = await _ordersRepository.GetOrderDetails(id);
        if (order == null)
        {
            return Response<OrderDetailsVM?>.FailureResponse("Order not found");
        }

        OrderDetailsVM orderDetailsVM = new()
        {
            Id = order.Id,
            InvoiceNumber = order.Invoices.FirstOrDefault()?.Id,
            OrderStatus = order.OrderStatus,
            PlacedOn = order.CreatedAt,
            ModifiedOn = order.ModifiedAt,
            OrderDuration = order.ModifiedAt.GetValueOrDefault().Subtract(order.CreatedAt),
        };

        orderDetailsVM.CustomerDetails = new CustomerDetails()
        {
            Id = order.Customer.Id,
            Name = order.Customer.Name,
            Phone = order.Customer.Phone,
            NoOfPerson = order.Customer.WaitingTokens.FirstOrDefault()?.NoOfPersons,
            Email = order.Customer.Email,
            CustomerId = order.CustomerId,
            SectionId = order.Customer.WaitingTokens.FirstOrDefault()?.SectionId,
            IsWaiting = order.Customer.WaitingTokens.FirstOrDefault()?.IsAssigned,
            Capacity = order.TableOrderMappings.Sum(t => t.Table.Capacity)
        };

        orderDetailsVM.TablesName = order.TableOrderMappings.Select(t => t.Table.Name).ToList();
        orderDetailsVM.SectionName = order.TableOrderMappings.FirstOrDefault()?.Table.Section.Name;

        orderDetailsVM.OrderItems = order.OrderedItems.Select(i => new OrderItem()
        {
            Id = i.Id,
            ItemId = i.MenuItem.Id,
            Name = i.ItemName,
            Quantity = i.Quantity,
            Price = i.ItemRate.GetValueOrDefault(),
            ItemTotal = i.ItemTotal.GetValueOrDefault(),
            TotalModifierAmount = i.TotalModifierAmount,
            TaxPercentage = i.Tax,

            OrderModifiers = i.OrderedItemModifierMappings.Select(m => new OrderModifier()
            {
                Id = m.Id,
                ModifierId = m.ModifierId,
                Name = m.ModifierName,
                Quantity = m.QuantityOfModifier.GetValueOrDefault(),
                Price = m.RateOfModifier.GetValueOrDefault(),
                TotalAmount = (m.RateOfModifier * m.QuantityOfModifier).GetValueOrDefault()
            }).ToList(),

        }).ToList();

        orderDetailsVM.SubTotal = order.SubTotal.GetValueOrDefault();
        orderDetailsVM.OrderTaxes = await GetOrderTaxes(order.OrderTaxMappings);
        orderDetailsVM.PaymentMethod = order.Invoices.FirstOrDefault()?.Payments.FirstOrDefault().PaymentMethod ?? "Pending";
        orderDetailsVM.TotalAmount = order.TotalAmount.GetValueOrDefault();
        orderDetailsVM.Comment = order.Notes;

        return Response<OrderDetailsVM?>.SuccessResponse(orderDetailsVM, "Order Details fetched");

    }

    private static int GetQuantity(string status, int quantity, int readyQuantity)
    {
        if (status.Equals("InProgress"))
        {
            quantity = quantity - readyQuantity;
        }
        else if (status.Equals("Ready"))
        {
            quantity = readyQuantity;
        }

        return quantity;
    }

    private static List<OrderItems> GetOrderItems(IEnumerable<OrderedItem> orderItems, string status, int categoryId)
    {
        if (status.Equals("InProgress"))
        {
            orderItems = orderItems.Where(i => (i.Quantity - i.ReadyItemQuantity) > 0);

        }
        else if (status.Equals("Ready"))
        {
            orderItems = orderItems.Where(i => i.ReadyItemQuantity.GetValueOrDefault() > 0);
        }

        return orderItems.Where(i => categoryId == 0 || i.MenuItem.CategoryId == categoryId).Select(oti => new OrderItems()
        {
            Id = oti.Id,
            Name = oti.MenuItem.Name,
            Quantity = GetQuantity(status, oti.Quantity, oti.ReadyItemQuantity.GetValueOrDefault()),
            Instruction = oti.Instruction,

            OrderModifiers = oti.OrderedItemModifierMappings.Select(oim => new OrderModifiers
            {
                Id = oim.Modifier.Id,
                Name = oim.Modifier.Name,

            }).ToList()

        }).ToList();
    }

    public async Task<Response<IEnumerable<KotOrderVM>>> GetOrderByCategory(int categoryId, string status)
    {
        try
        {
            IEnumerable<Order> orders = await _ordersRepository.GetOrderByCategory(categoryId, status);

            if (orders == null)
            {
                return Response<IEnumerable<KotOrderVM>>.FailureResponse("Order Details Does not exist!");
            }

            IEnumerable<KotOrderVM?> orderList = orders.Select(order => new KotOrderVM
            {
                OrderNumber = order.Id,
                Date = order.CreatedAt,
                SectionName = order.TableOrderMappings.FirstOrDefault()?.Table.Section.Name,
                TablesName = order.TableOrderMappings.Select(t => t.Table.Name).ToList(),
                Instruction = order.Notes,
                OrderItems = GetOrderItems(order.OrderedItems, status, categoryId),

            }).ToList().Where(i => i.OrderItems.Count() > 0);

            return Response<IEnumerable<KotOrderVM>>.SuccessResponse(orderList!, "order details fetched Successfully!");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Response<IEnumerable<KotOrderVM>>.FailureResponse("order Details does not exist!");
        }
    }


    public async Task<Response<bool>> UpdateOrderItemStatusAsync(int orderId, string status, List<OrderItem> updatedOrderItems)
    {

        Order? order = await _ordersRepository.GetOrderDetails(orderId);
        if (order == null)
        {
            return Response<bool>.FailureResponse("Order Does Not Found");
        }

        IEnumerable<OrderedItem> existingOrderItems = await _orderedItemRepository.GetAllByOrderIdAsync(orderId);
        List<OrderedItem> orderItemsToUpdate = new();

        foreach (OrderItem updatedOrderItem in updatedOrderItems)
        {
            OrderedItem? orderItem = existingOrderItems.SingleOrDefault(oi => oi.Id == updatedOrderItem.Id);
            if (orderItem != null)
            {
                if (status.Equals("InProgress"))
                {
                    orderItem.ReadyItemQuantity = orderItem.ReadyItemQuantity.GetValueOrDefault() - updatedOrderItem.Quantity;
                }
                else
                {
                    orderItem.ReadyItemQuantity = orderItem.ReadyItemQuantity.GetValueOrDefault() + updatedOrderItem.Quantity;
                }

                if (orderItem.ReadyItemQuantity >= 0 && orderItem.ReadyItemQuantity <= orderItem.Quantity)
                {
                    orderItem.Status = (orderItem.ReadyItemQuantity == orderItem.Quantity) ? "Ready" : "InProgress";
                    orderItemsToUpdate.Add(orderItem);
                }
            }

        }

        await _orderedItemRepository.UpdateRangeAsync(orderItemsToUpdate);

        if (existingOrderItems.All(oi => oi.Status == "Ready"))
        {
            order.OrderStatus = "Served";
        }
        else
        {
            order.OrderStatus = "InProgress";
        }

        await _ordersRepository.UpdateOrder(order);

        return Response<bool>.SuccessResponse(true, "Order Item Quantity Updated");
    }

}


