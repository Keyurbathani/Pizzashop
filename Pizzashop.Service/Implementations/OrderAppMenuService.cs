
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Entity.Constants;


namespace Pizzashop.Service.Implementations;

public class OrderAppMenuService : IOrderAppMenuService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrdersRepository _orderRepository;
    private readonly IOrderedItemRepository _orderItemRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ITableOrderMappingRepository _orderTableRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IModifierRepository _modifierRepository;
    private readonly IOrderItemModifierRepository _orderModifierRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOrderTaxMappingRepository _orderTaxRepository;
    private readonly ITaxAndFeeRepository _taxAndFeeRepository;
    private readonly IWaitingTokensRepository _waitingTokensRepository;


    public OrderAppMenuService(IOrdersRepository orderRepository, IOrderedItemRepository orderItemRepository, ITableRepository tableRepository, ISectionRepository sectionRepository, ICustomerRepository customerRepository, ITableOrderMappingRepository orderTableRepository, IItemRepository itemRepository, IModifierRepository modifierRepository, IOrderItemModifierRepository orderModifierRepository, IPaymentRepository paymentRepository, IInvoiceRepository invoiceRepository, IOrderTaxMappingRepository orderTaxRepository, ITaxAndFeeRepository taxAndFeeRepository, IFeedbackRepository feedbackRepository , IWaitingTokensRepository waitingTokensRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _tableRepository = tableRepository;
        _sectionRepository = sectionRepository;
        _customerRepository = customerRepository;
        _orderTableRepository = orderTableRepository;
        _itemRepository = itemRepository;
        _modifierRepository = modifierRepository;
        _orderModifierRepository = orderModifierRepository;
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
        _orderTaxRepository = orderTaxRepository;
        _taxAndFeeRepository = taxAndFeeRepository;
        _feedbackRepository = feedbackRepository;
        _waitingTokensRepository = waitingTokensRepository;

    }
    public async Task<Response<CustomerDetails?>> UpdateCustomerDetailsAsync(CustomerDetails model)
    {


        Customer? customer = await _customerRepository.GetCustomerById(model.CustomerId.GetValueOrDefault());

        if (customer == null)
        {
            return Response<CustomerDetails?>.FailureResponse("Customer not found");
        }

        bool isExist = await _customerRepository.IsExistsAsync(c => c.Email.ToLower().Trim() == model.Email.ToLower().Trim() && c.Id != model.CustomerId);

        if (isExist)
        {
            return Response<CustomerDetails?>.FailureResponse("Customer with email already exists");
        }

        customer.Email = model.Email;
        customer.Name = model.Name;
        customer.Phone = model.Phone.GetValueOrDefault();

        await _customerRepository.UpdateCustomer(customer);


        var token = await _waitingTokensRepository.GetByCustomerId(customer.Id);

        token.NoOfPersons = model.NoOfPerson.GetValueOrDefault();

        await _waitingTokensRepository.UpdateToken(token);

        return Response<CustomerDetails?>.SuccessResponse(model, "Customer Details updated");


    }

    public async Task<Response<string?>> EditOrderCommentAsync(int id, string? comment)
    {

        var order = await _orderRepository.GetOrderDetails(id);

        if (order == null)
        {
            return Response<string?>.FailureResponse("Order Does Not Found");
        }

        order.Notes = comment;


        await _orderRepository.UpdateOrder(order);

        return Response<string?>.SuccessResponse(comment, "Order Comment Added");

    }

    private static decimal GetTaxAmount(bool type, decimal taxValue, decimal subtotal)
    {
        if (type == false)
        {
            return taxValue;
        }
        else
        {
            return taxValue * subtotal * (decimal)0.01;
        }
    }

    public async Task<Response<bool>> SaveOrderAsync(OrderDetailsVM model)
    {

        try
        {
            Order? order = await _orderRepository.GetOrderDetails(model.Id);

            if (order == null)
            {
                return Response<bool>.FailureResponse("Order does not found");
            }

            List<OrderedItem> orderItemsToUpdate = new();
            List<OrderedItem> orderItemsToRemove = new();

            List<OrderedItemModifierMapping> orderModifiersToRemove = new();
            List<OrderedItemModifierMapping> orderModifiersToAdd = new();
            List<OrderedItemModifierMapping> orderModifiersToUpdate = new();
            decimal subtotal = 0;

            IEnumerable<OrderedItem> existingOrderItems = await _orderItemRepository.GetAllByOrderIdAsync(model.Id);

            #region  OrderItemRemove
            foreach (OrderedItem existingOrderItem in existingOrderItems)
            {
                if (!model.OrderItems.Any(oi => oi.Id == existingOrderItem.Id))
                {
                    orderItemsToRemove.Add(existingOrderItem);
                    IEnumerable<OrderedItemModifierMapping> orderModifiers = await _orderModifierRepository.GetAllByOrderItemIdAsync(existingOrderItem.Id);
                    orderModifiersToRemove.AddRange(orderModifiers);
                }
            }
            await _orderModifierRepository.RemoveRangeAsync(orderModifiersToRemove);
            await _orderItemRepository.RemoveRangeAsync(orderItemsToRemove);
            #endregion OrderItemRemove

            #region  Order Items Add and Update
            foreach (OrderItem orderItemToSave in model.OrderItems)
            {
                OrderedItem? orderItem = existingOrderItems.SingleOrDefault(oi => oi.Id == orderItemToSave.Id);

                decimal totalModifierAmount = 0;

                if (orderItem == null)
                {
                    MenuItem? item = await _itemRepository.GetByIdAsync(orderItemToSave.ItemId.GetValueOrDefault());

                    if (item == null)
                    {
                        return Response<bool>.FailureResponse("Item Does Not Found");
                    }

                    orderItem = new()
                    {
                        MenuItemId = item.Id,
                        OrderId = model.Id,
                        Quantity = orderItemToSave.Quantity,
                        Status = "InProgress",
                        Tax = item.TaxPercentage,
                        TotalModifierAmount = 0,
                        ReadyItemQuantity = 0,
                        ItemTotal = item.Rate * orderItemToSave.Quantity,
                        ItemName = item.Name,
                        ItemRate = item.Rate,
                        CreatedAt = DateTime.Now,
                    };

                    await _orderItemRepository.AddItemsDetails(orderItem);

                    foreach (int id in orderItemToSave.ModifierIds)
                    {
                        Modifier? modifier = await _modifierRepository.GetByIdAsync(id);

                        if (modifier == null)
                        {
                            return Response<bool>.FailureResponse("Modifier does not found");
                        }

                        OrderedItemModifierMapping orderModifier = new()
                        {
                            OrderItemId = orderItem.Id,
                            QuantityOfModifier = orderItem.Quantity,
                            ModifierId = modifier.Id,
                            TotalAmount = orderItem.Quantity * modifier.Rate,
                            ModifierName = modifier.Name,
                            RateOfModifier = modifier.Rate,
                            CreatedAt = DateTime.Now,
                        };

                        totalModifierAmount += orderModifier.TotalAmount.GetValueOrDefault();
                        orderModifiersToAdd.Add(orderModifier);
                    }
                }
                else
                {
                    orderItem.Quantity = orderItemToSave.Quantity;
                    orderItem.ItemTotal = orderItem.ItemRate * orderItemToSave.Quantity;
                    orderItem.Status = (orderItem.ReadyItemQuantity < orderItem.Quantity) ? "InProgress" : "Ready";
                    IEnumerable<OrderedItemModifierMapping> existingOrderModifiersForOrderItem = await _orderModifierRepository.GetAllByOrderItemIdAsync(orderItem.Id);

                    foreach (int id in orderItemToSave.ModifierIds)
                    {
                        OrderedItemModifierMapping? orderModifier = existingOrderModifiersForOrderItem.SingleOrDefault(om => om.ModifierId == id);

                        if (orderModifier == null)
                        {
                            Modifier? modifier = await _modifierRepository.GetByIdAsync(id);

                            if (modifier == null)
                            {
                                return Response<bool>.FailureResponse("Modifier does not found");
                            }

                            orderModifier = new()
                            {
                                OrderItemId = orderItem.Id,
                                QuantityOfModifier = orderItemToSave.Quantity,
                                ModifierId = modifier.Id,
                                TotalAmount = orderItemToSave.Quantity * modifier.Rate,
                                RateOfModifier = modifier.Rate,
                                ModifierName = modifier.Name,
                                CreatedAt = DateTime.Now,
                            };

                            totalModifierAmount += orderModifier.TotalAmount.GetValueOrDefault();
                            orderModifiersToAdd.Add(orderModifier);
                        }
                        else
                        {
                            orderModifier.QuantityOfModifier = orderItemToSave.Quantity;
                            orderModifier.TotalAmount = orderModifier.RateOfModifier * orderItemToSave.Quantity;

                            totalModifierAmount += orderModifier.TotalAmount.GetValueOrDefault();
                            orderModifiersToUpdate.Add(orderModifier);
                        }
                    }
                }

                orderItem.ItemTotal *= 1 + ((decimal)0.01 * orderItem.Tax);
                // orderItem.ItemTotal = (orderItem.ItemRate * orderItemToSave.Quantity ) + (orderItem.ItemRate * (decimal)0.01 * orderItem.Tax);

                orderItem.TotalModifierAmount = totalModifierAmount;
                orderItem.TotalAmount = orderItem.ItemTotal + orderItem.TotalModifierAmount;

                subtotal += orderItem.ItemTotal.GetValueOrDefault() + totalModifierAmount;
                orderItemsToUpdate.Add(orderItem);
            }

            await _orderModifierRepository.AddRangeAsync(orderModifiersToAdd);
            await _orderModifierRepository.UpdateRangeAsync(orderModifiersToUpdate);
            await _orderItemRepository.UpdateRangeAsync(orderItemsToUpdate);
            #endregion  Order Items Add and Update

            #region Order Tax Add and Update
            IEnumerable<OrderTaxMapping> orderTaxes = await _orderTaxRepository.GetAllByOrderId(model.Id);

            List<OrderTaxMapping> orderTaxesToAdd = new();
            List<OrderTaxMapping> orderTaxesToUpdate = new();
            decimal total = subtotal;

            foreach (int id in model.OrderTaxIds)
            {
                OrderTaxMapping? orderTax = orderTaxes.SingleOrDefault(ot => ot.TaxId == id);

                if (orderTax == null)
                {
                    TaxAndFee? tax = await _taxAndFeeRepository.GetByIdAsync(id);
                    if (tax == null)
                    {
                        return Response<bool>.FailureResponse("Tax not found");
                    }

                    orderTax = new()
                    {
                        OrderId = model.Id,
                        TaxId = id,
                        TaxValue = tax.TaxValue,
                        TaxName = tax.Name,
                        TaxType = tax.TaxType,
                        TotalTax = GetTaxAmount(tax.TaxType.GetValueOrDefault(), tax.TaxValue.GetValueOrDefault(), subtotal),
                    };
                    orderTaxesToAdd.Add(orderTax);
                }
                else
                {
                    orderTax.TotalTax = GetTaxAmount(orderTax.TaxType.GetValueOrDefault(), orderTax.TaxValue.GetValueOrDefault(), subtotal);

                    orderTaxesToUpdate.Add(orderTax);
                }

                total += orderTax.TotalTax.GetValueOrDefault();
            }

            await _orderTaxRepository.UpdateRangeAsync(orderTaxesToUpdate);
            await _orderTaxRepository.AddRangeAsync(orderTaxesToAdd);

            order.TotalAmount = total;
            order.SubTotal = subtotal;
            order.OrderStatus = "InProgress";
            order.ModifiedAt = DateTime.Now;

            existingOrderItems = await _orderItemRepository.GetAllByOrderIdAsync(model.Id);
            if (existingOrderItems.Any(i => i.ReadyItemQuantity != i.Quantity))
            {
                order.OrderStatus = "InProgress";
            }
            else if (!existingOrderItems.Any())
            {
                order.OrderStatus = "Pending";
            }
            else
            {
                order.OrderStatus = "Served";
            }


            await _orderRepository.UpdateOrder(order);

            #endregion Order Tax Add and Update


            return Response<bool>.SuccessResponse(true, "Order Saved Successfully");
        }
        catch
        {
            return Response<bool>.FailureResponse("Something went wrong");
        }
    }

    public async Task<Response<bool>> IsItemQuantityPrepared(int orderItemId, int quantity)
    {

        int readyQuantity = await _orderItemRepository.GetReadyQuantityAsync(orderItemId);

        if (readyQuantity < quantity)
        {
            return Response<bool>.FailureResponse("shown quantity already prepared");
        }

        return Response<bool>.SuccessResponse(true, "true");

    }

    public async Task<Response<bool>> CompleteOrderAsync(int orderId, string paymentMethod)
    {

        Order? order = await _orderRepository.GetOrderDetails(orderId);

        if (order == null)
        {
            return Response<bool>.FailureResponse("Order Does not found");
        }

        if (order.OrderStatus != "Served")
        {
            return Response<bool>.FailureResponse("order is not served yet");
        }

        order.OrderStatus = "Completed";

        IEnumerable<TableOrderMapping> orderTables = await _orderTableRepository.GetByOrderIdAsync(orderId);
        List<Table> tablesToUpdate = new();

        foreach (TableOrderMapping orderTable in orderTables)
        {
            Table table = orderTable.Table;
            // table.CurrentOrderId = null;
            table.IsAvailable = true;
            tablesToUpdate.Add(table);
        }

        var invoice = await _invoiceRepository.GetByOrderId(order.Id);

        var payment = await _paymentRepository.GetByInvoiceId(invoice!.Id);

        payment!.InvoiceId = invoice.Id;
        payment.PaymentMethod = paymentMethod;
        payment.Amount = order.TotalAmount;
        payment.Status = "pending";


        await _paymentRepository.UpdatePayment(payment);

        await _tableRepository.UpdateRangeAsync(tablesToUpdate);

        await _orderRepository.UpdateOrder(order);

        return Response<bool>.SuccessResponse(true, "Order complated");

    }
    public async Task<Response<bool>> IsOrderServedAsync(int orderId)
    {

        bool isOrderServed = await _orderRepository.IsOrderServedAsync(orderId);

        if (!isOrderServed)
        {
            return Response<bool>.FailureResponse("order is in progress now");
        }

        return Response<bool>.SuccessResponse(true, "order is served");

    }
    public async Task<Response<bool>> ReviewOrder(int orderId, decimal foodCount, decimal serviceCount, decimal ambienceCount, string reviewComment)
    {

        var orderfeedback = await _feedbackRepository.GetFeedbackByOrderId(orderId);

        if (orderfeedback == null)
        {
            Response<bool>.FailureResponse("feedback for order is not found");
        }

        orderfeedback.Food = foodCount;
        orderfeedback.Ambience = ambienceCount;
        orderfeedback.Service = serviceCount;
        orderfeedback.AvgRating = (foodCount + serviceCount + ambienceCount) / 3;
        orderfeedback.Comments = reviewComment;

        await _feedbackRepository.UpdateFeedback(orderfeedback);

        return Response<bool>.SuccessResponse(true, "review addedd");
    }
    public async Task<Response<bool>> CancelOrderAsync(int orderId)
    {

        Order? order = await _orderRepository.GetOrderDetails(orderId);
        if (order == null)
        {
            return Response<bool>.FailureResponse("Order not found");
        }

        bool isAnyItemPrepared = await _orderRepository.IsExistsAsync(o => o.Id == orderId && o.OrderedItems.Any(oi => oi.ReadyItemQuantity > 0));

        if (isAnyItemPrepared)
        {
            return Response<bool>.FailureResponse("order does not canceled due to some items has been already prepared");
        }

        order.OrderStatus = "Cancelled";


        IEnumerable<TableOrderMapping> orderTables = await _orderTableRepository.GetByOrderIdAsync(orderId);
        List<Table> tablesToUpdate = new();

        foreach (TableOrderMapping orderTable in orderTables)
        {
            Table table = orderTable.Table;
            table.IsAvailable = true;
            tablesToUpdate.Add(table);
        }

        await _tableRepository.UpdateRangeAsync(tablesToUpdate);
        await _orderRepository.UpdateOrder(order);
        return Response<bool>.SuccessResponse(true, "Order cancelled successfully");

    }
    public async Task<Response<bool>> SaveItemComment(int id, string ItemComment)
    {

        var orderItem = await _orderItemRepository.GetById(id);

        if (orderItem == null)
        {
            return Response<bool>.FailureResponse("OrderItem Not Found");
        }

        orderItem.Instruction = ItemComment;
        await _orderItemRepository.UpdateItemsDetails(orderItem);

        return Response<bool>.SuccessResponse(true, "Special Instruction Added");

    }
    public async Task<Response<string>> GetItemComment(int id)
    {

        var orderItem = await _orderItemRepository.GetById(id);

        if (orderItem == null)
        {
            return Response<string>.FailureResponse("OrderItem Not Found");
        }

        return Response<string>.SuccessResponse(orderItem.Instruction, "Special Instruction Added");

    }

}



