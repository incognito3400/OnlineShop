using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Exam2.Backend.Entities;

public class ProductDetail
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    [JsonIgnore]
    public Product Product { get; set; } = null!;
    
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
