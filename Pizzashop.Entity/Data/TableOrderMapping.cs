using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class TableOrderMapping
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int TableId { get; set; }

    public int NoOfPersons { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
