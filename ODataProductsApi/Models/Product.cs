using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ODataProductsApi.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 999999.99, ErrorMessage = "Price must be between 0 and 999999.99")]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "CategoryId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }
}