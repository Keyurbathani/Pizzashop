using System;
using System.Collections.Generic;

namespace Pizzashop.Entity.Data;

public partial class Feedback
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string? Comments { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public decimal? Food { get; set; }

    public decimal? Service { get; set; }

    public decimal? Ambience { get; set; }

    public decimal? AvgRating { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;
}
