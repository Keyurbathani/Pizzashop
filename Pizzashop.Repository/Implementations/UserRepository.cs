using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class UserRepository : IUserRepository
{
    private readonly PizzaShopContext _context;

    public UserRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
    }
   
    public async Task<User?> GetUserByPhone(long phone)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
    }
    

    public async Task<User?> GetUserByUserName(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
   
    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> IsExistsAsync(Expression<Func<User, bool>> predicate)
    {
        return await _context.Users.AnyAsync(predicate);
    }

    public async Task UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task Adduser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<User> list, int totalRecords)> GetPagedUsersAsync(string searchString, int page, int pagesize, bool isASC, string sortColumn)
    {

        IQueryable<User> query = _context.Users.Where(i => !(bool)i.IsDeleted!).Include(i => i.Role);

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => (i.FisrtName + " " + i.LastName).ToLower().Contains(searchString));
        }


        if (isASC)
        {

            switch (sortColumn)
            {
                case "user":
                    query = query.OrderBy(u => u.FisrtName);
                    break;

                case "role":
                    query = query.OrderBy(u => u.Role.Name);
                    break;
            }

        }
        else
        {
            switch (sortColumn)
            {
                case "user":
                    query = query.OrderByDescending(u => u.FisrtName);
                    break;

                case "role":
                    query = query.OrderByDescending(u => u.Role.Name);
                    break;

            }

        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());

    }

}
