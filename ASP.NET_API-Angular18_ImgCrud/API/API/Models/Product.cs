using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string Description { get; set; } = "";
    public string? ProductImage { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; } = 0;
    public int SellerId { get; set; }
}
