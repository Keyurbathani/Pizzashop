using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class ModifierAndGroup
{
    public int Id { get; set; }

    public int? ModifierId { get; set; }

    public int? ModifiergroupId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Modifier? Modifier { get; set; }

    public virtual ModifierGroup? Modifiergroup { get; set; }
}
