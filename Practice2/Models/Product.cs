using System.ComponentModel.DataAnnotations.Schema;

namespace Practice2.Models;

public class Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public int Count { get; set; }
}
