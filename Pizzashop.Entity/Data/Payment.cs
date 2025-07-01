using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class Payment
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Status { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual User? ModifiedByNavigation { get; set; }
}
