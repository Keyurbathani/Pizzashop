using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModels;

public class TableVM
{
    public int Id { get; set; }
    
    [StringLength(50)]
    [Required(ErrorMessage = "Name is Required")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Section is Required")]
    public int? SectionId { get; set; } = null!;

    [Required(ErrorMessage = "Capacity is Required")]
    [Range(1, 50, ErrorMessage = "Table Capacity should be from 1 To 50")]
    public int? Capacity { get; set; } = null!;

    public bool? IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }
}