using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class OrderTaxMapping
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int TaxId { get; set; }

    public decimal? TaxValue { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public string? TaxName { get; set; }

    public bool? TaxType { get; set; }

    public decimal? TotalTax { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual TaxAndFee Tax { get; set; } = null!;
}
