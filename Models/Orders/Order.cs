using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ISSTechLogistics.Models.Orders;

public class Order
{
    [Key]
    public int ShipmentId { get; set; }

    [Required]
    [MaxLength(250)]
    public string Origin { get; set; }

    [Required]
    [MaxLength(250)]
    public string Destination { get; set; }

    [Required]
    [Precision(6, 3)]
    public decimal Weight { get; set; }

    [Required]
    public int DeliveryTime { get; set; }
}