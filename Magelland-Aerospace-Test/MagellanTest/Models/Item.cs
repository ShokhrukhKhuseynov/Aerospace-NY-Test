using System.ComponentModel.DataAnnotations;

namespace MagellanTest.Models;

public class Item
{
    [Required]
    public string Name { get; set; }
    
    public int? ParentItemId { get; set; }
    
    [Required]
    public int Cost { get; set; }
    
    [Required]
    public DateTime ReqDate { get; set; }
}