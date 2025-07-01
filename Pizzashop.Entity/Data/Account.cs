using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class Account
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public bool? IsFirstLogin { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public Guid? Token { get; set; }

    public DateTime? TokenExpiry { get; set; }

    public string? Role { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? UserId { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual User? User { get; set; }
}
