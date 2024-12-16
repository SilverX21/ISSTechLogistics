using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ISSTechLogistics.Models.Orders;

public class OrdersDetailsStatistics
{
    public int Id { get; set; }

    public int TotalOrders { get; set; }

    [Precision(6, 3)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:G3}")]
    public decimal AverageWeight { get; set; }

    [Precision(4, 2)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:G2}")]
    public decimal AverageDeliveryTime { get; set; }

    public DateTime CalculationDate { get; set; }

    public bool IsLatest { get; set; }
}