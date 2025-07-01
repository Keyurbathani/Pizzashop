using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class City
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int StateId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual State State { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
