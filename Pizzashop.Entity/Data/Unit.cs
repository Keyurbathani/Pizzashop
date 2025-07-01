using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class Unit
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
}
