using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class WaitingTokensService : IWaitingTokensService
{
    private readonly IWaitingTokensRepository _waitingTokensRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly ITableOrderMappingRepository _tableMapping;
    private readonly ITableRepository _tableRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IFeedbackRepository _feedbackRepository;

    public WaitingTokensService(IWaitingTokensRepository waitingTokensRepository, ICustomerRepository customerRepository, IOrdersRepository ordersRepository, ITableOrderMappingRepository tableMapping, ITableRepository tableRepository, IInvoiceRepository invoiceRepository, IPaymentRepository paymentRepository, IFeedbackRepository feedbackRepository)
    {
        _waitingTokensRepository = waitingTokensRepository;
        _customerRepository = customerRepository;
        _ordersRepository = ordersRepository;
        _tableMapping = tableMapping;
        _tableRepository = tableRepository;
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _feedbackRepository = feedbackRepository;
    }

    public async Task<Response<IEnumerable<WaitingTokenList>>> GetAllTokensBySection(int sectionId)
    {

        IEnumerable<WaitingToken> token = await _waitingTokensRepository.GetAllTokensBySection(sectionId);

        IEnumerable<WaitingTokenList> tokenlist = token.Select(i => new WaitingTokenList()
        {
            Id = i.Id,
            CustomerId = i.CustomerId,
            Name = i.Customer.Name,
            NoOfPerson = i.NoOfPersons,
            Phone = i.Customer.Phone,
            Email = i.Customer.Email,
            CreatedAt = i.CreatedAt,
            SectionId = i.SectionId

        }).ToList();

        return Response<IEnumerable<WaitingTokenList>>.SuccessResponse(tokenlist, "waiting list fetched successfully!");


    }


    public async Task<Response<WaitingTokenList>> GetCustomerAsync(int customerId)
    {


        var customer = await _customerRepository.GetCustomerAsync(customerId);

        if (customer == null)
        {
            return Response<WaitingTokenList>.FailureResponse("customer not found");
        }

        var model = new WaitingTokenList()
        {
            Email = customer.Email,
            Name = customer.Name,
            Phone = customer.Phone,
            NoOfPerson = customer.WaitingTokens.FirstOrDefault()?.NoOfPersons,
            SectionName = customer.WaitingTokens.FirstOrDefault()?.Section.Name
        };

        return Response<WaitingTokenList>.SuccessResponse(model, "customer details found");
    }
    public async Task<Response<WaitingTokenList>> GetCustomerByEmail(string email)
    {


        var customer = await _customerRepository.GetCustomerByEmail(email);

        if (customer == null)
        {
            return Response<WaitingTokenList>.FailureResponse("customer not found");

        }

        var model = new WaitingTokenList()
        {
            Id = customer.WaitingTokens.FirstOrDefault()?.Id,
            Email = customer.Email,
            CustomerId = customer.Id,
            Name = customer.Name,
            Phone = customer.Phone,
            NoOfPerson = customer.WaitingTokens.FirstOrDefault()?.NoOfPersons,
            SectionName = customer.WaitingTokens.FirstOrDefault()?.Section.Name
        };

        return Response<WaitingTokenList>.SuccessResponse(model, "customer details found");
    }

    public async Task<WaitingTokenList?> GetTokenByIdAsync(int id)
    {

        var token = await _waitingTokensRepository.GetByIdAsync(id);
        if (token == null)
        {
            return null;
        }

        WaitingTokenList TokemVM = new()
        {
            Id = token.Id,
            CustomerId = token.CustomerId,
            Name = token.Customer.Name,
            NoOfPerson = token.NoOfPersons,
            Phone = token.Customer.Phone,
            Email = token.Customer.Email,
            SectionId = token.SectionId
        };

        return TokemVM;

    }


    public async Task<Response<bool>> CreateToken(WaitingTokenList model)
    {

        var totalCapacity = _tableRepository.GetTotalCapacityBySectionId(model.SectionId.GetValueOrDefault());

        if (totalCapacity < model.NoOfPerson)
        {
            return Response<bool>.FailureResponse("Total Capacity is less than No Of Person");
        }
        var customer = await _customerRepository.GetCustomerByEmail(model.Email!);

        if (customer == null)
        {
            var newCustomer = new Customer
            {
                Name = model.Name,
                Phone = model.Phone.GetValueOrDefault(),
                Email = model.Email!,
            };

            await _customerRepository.AddCustomer(newCustomer);
            customer = newCustomer;
        }

        customer.Name = model.Name;
        customer.Email = model.Email;
        customer.Phone = model.Phone.GetValueOrDefault();

        await _customerRepository.UpdateCustomer(customer);

        var newToken = new WaitingToken
        {
            CustomerId = customer.Id,
            NoOfPersons = model.NoOfPerson.GetValueOrDefault(),
            SectionId = model.SectionId.GetValueOrDefault(),
        };

        await _waitingTokensRepository.AddToken(newToken);

        return Response<bool>.SuccessResponse(true, "Waiting token created or updated successfully.");
    }


    public async Task<Response<bool>> EditToken(WaitingTokenList model)
    {

        var token = await _waitingTokensRepository.GetByIdAsync(model.Id.GetValueOrDefault());

        var totalCapacity = _tableRepository.GetTotalCapacityBySectionId(model.SectionId.GetValueOrDefault());

        if (totalCapacity < model.NoOfPerson)
        {
            return Response<bool>.FailureResponse("Total Capacity is less than No Of Person");
        }

        token.NoOfPersons = model.NoOfPerson.GetValueOrDefault();
        token.SectionId = model.SectionId.GetValueOrDefault();
        token.ModifiedAt = DateTime.Now;                                                

        await _waitingTokensRepository.UpdateToken(token);

        var customer = await _customerRepository.GetCustomerById(token.CustomerId);

         
        customer.Phone = model.Phone.GetValueOrDefault();
        customer.Email = model.Email;
        customer.Name = model.Name;

        await _customerRepository.UpdateCustomer(customer);
        return Response<bool>.SuccessResponse(true, "Waiting token  updated successfully.");
    }

    public async Task<Response<bool>> DeleteToken(int id)
    {

        var token = await _waitingTokensRepository.GetByIdAsync(id);

        if (token == null)
        {
            return Response<bool>.FailureResponse("Token isn't Exists");
        }

        token.IsDeleted = true;

        await _waitingTokensRepository.UpdateToken(token);

        return Response<bool>.SuccessResponse(true, "Token Deleteted Successfully");
    }

    public async Task<Response<Order>> AssignTableAddOrder(int customerId, IEnumerable<int> ids, int tokenId)
    {

        var order = new Order()
        {
            CustomerId = customerId,
            OrderStatus = "Pending",
            ModifiedAt = DateTime.Now,
        };

        await _ordersRepository.AddOrder(order);

        var invoice = new Invoice()
        {
            OrderId = order.Id,
        };

        await _invoiceRepository.AddInvoice(invoice);

        var payment = new Payment()
        {
            InvoiceId = invoice.Id,
            PaymentMethod = "pending",
        };

        await _paymentRepository.AddPayment(payment);

        var feedback = new Feedback()
        {
            OrderId = order.Id,
            AvgRating = 0,
        };
        await _feedbackRepository.AddFeedback(feedback);

        var neworder = await _ordersRepository.GetOrderDetails(order.Id);

        foreach (int id in ids)
        {
            var mapping = new TableOrderMapping()
            {
                OrderId = order.Id,
                TableId = id,
                NoOfPersons = neworder.Customer.WaitingTokens.FirstOrDefault().NoOfPersons,
            };

            await _tableMapping.AddMapping(mapping);

            var table = await _tableRepository.GetByIdAsync(id);
            if (table != null)
            {
                table.IsAvailable = false;
                await _tableRepository.UpdateAsync(table);
            }
        }

        var token = await _waitingTokensRepository.GetByIdAsync(tokenId);
        if (token != null)
        {
            token.IsAssigned = true;
            await _waitingTokensRepository.UpdateToken(token);
        }

        return Response<Order>.SuccessResponse(order, "Table Assigned Successfully");
    }


}
