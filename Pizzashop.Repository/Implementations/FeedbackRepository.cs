using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly PizzaShopContext _context;

    public FeedbackRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task AddFeedback(Feedback feedback)
    {
           _context.Feedbacks.Add(feedback);
           await _context.SaveChangesAsync();
    }
    public async Task UpdateFeedback(Feedback feedback)
    {
           _context.Feedbacks.Update(feedback);
           await _context.SaveChangesAsync();
    }

    public async Task<Feedback?> GetFeedbackByOrderId(int OrderId){
         
         return await _context.Feedbacks.FirstOrDefaultAsync(i => i.OrderId == OrderId);
    }
}
