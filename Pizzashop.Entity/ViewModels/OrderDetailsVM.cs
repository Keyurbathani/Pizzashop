using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class OrderDetailsVM
{
    public int Id { get; set; }

    public int? InvoiceNumber { get; set; }

    public string? Comment { get; set; }

    public DateTime PlacedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public TimeSpan? OrderDuration { get; set; }
    public string? OrderStatus { get; set; }

    public CustomerDetails CustomerDetails { get; set; } = new();
    public List<string> TablesName { get; set; } = new();

    public string? SectionName { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();

    public decimal SubTotal { get; set; }

    public List<int>? OrderTaxIds { get; set; }

    public List<OrderTax> OrderTaxes { get; set; } = new();

    public decimal TotalAmount { get; set; }
    public string? PaymentMethod { get; set; }
}


public class CustomerDetails
{
    public int? Id { get; set; }
    public int? CustomerId { get; set; }
    public int? SectionId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot be longer than 100 characters")]
    [RegularExpression("^[a-zA-Z ]{1,}$", ErrorMessage = "Name should only contain alphabets!")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [Range(1000000000, 9999999999, ErrorMessage = "Phone must be a valid 10-digit number")]
    public long? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }

    [Range(1, 50, ErrorMessage = "Number of persons must be between 1 and 50")]
    [Required(ErrorMessage = "Number of Persons is Required")]
    public int? NoOfPerson { get; set; }

    public int? Capacity { get; set; }
    public bool? IsWaiting { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int? ItemId { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal? TaxPercentage { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ItemTotal { get; set; }
    public decimal? TotalModifierAmount { get; set; }
    public decimal CalcAmount { get; set; }
    public List<int> ModifierIds { get; set; } = new();
    public List<OrderModifier> OrderModifiers { get; set; } = new();
}

public class OrderModifier
{
    public int Id { get; set; }
    public int? ModifierId { get; set; }

    public string? Name { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TotalAmount { get; set; }
}

public class OrderTax
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool Type { get; set; }
    public decimal? TotalTax { get; set; }
    public decimal Rate { get; set; }


}
