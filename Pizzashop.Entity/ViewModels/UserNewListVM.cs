using Microsoft.AspNetCore.Http;

namespace Pizzashop.Entity.ViewModels;

public class UserNewListVM
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public long? Phone { get; set; }
    public int RoleId { get; set; }
    public string? Role { get; set; }
    public bool IsActive { get; set; }
    public string? Imageurl { get; set; }
    public IFormFile? ProfileImage { get; set; }
}
