
using Microsoft.AspNetCore.Http;

namespace Pizzashop.Entity.ViewModels;

    public class UserListModel
    {
        public List<UserDto> Users { get; set; }
        public string SearchQuery { get; set; }
        
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }

        public bool IsDeleted {get; set;}
        public string Name { get; set; }
        public string Email { get; set; }
        public long? Phone { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }

        public string? Imageurl { get; set; }
        public IFormFile? ProfileImage {get; set;} 


    }


