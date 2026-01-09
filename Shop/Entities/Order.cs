using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam2.Backend.Entities;

public class Order
{
    public int Id { get; set; }
    public int? UserId { get; set; } 
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "New"; 
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    public List<OrderItem> Items { get; set; } = new();
}
