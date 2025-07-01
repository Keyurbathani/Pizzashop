namespace Pizzashop.Entity.ViewModels;

public class RolesPermissionVM
{
    public long RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public List<PagePermission>? PagePermission { get; set; }
}


public class PagePermission
{
    public string Name { get; set; } = null!;
    public long Id { get; set; }
    public bool View { get; set; }
    public bool Add { get; set; }
    public bool Delete { get; set; }
}



