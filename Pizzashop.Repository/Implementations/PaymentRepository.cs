using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class PaymentRepository : IPaymentRepository
{
    private readonly PizzaShopContext _context;

    public PaymentRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task AddPayment(Payment payment)
    {
           _context.Payments.Add(payment);
           await _context.SaveChangesAsync();
    }
    public async Task<Payment?> GetByInvoiceId(int invoiceId)
    {
        return await _context.Payments.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
    }

    public async Task UpdatePayment(Payment payment)
    {
         _context.Payments.Update(payment);
           await _context.SaveChangesAsync();
    }
}
