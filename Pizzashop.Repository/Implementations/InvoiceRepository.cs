using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly PizzaShopContext _context;

    public InvoiceRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task AddInvoice(Invoice invoice)
    {
           _context.Invoices.Add(invoice);
           await _context.SaveChangesAsync();
    }

    public async Task<Invoice?> GetByOrderId(int orderId)
    {
        return await _context.Invoices.FirstOrDefaultAsync(i => i.OrderId == orderId);
    }
}
