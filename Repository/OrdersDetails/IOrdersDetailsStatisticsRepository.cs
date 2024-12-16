using ISSTechLogistics.Models.Base;
using ISSTechLogistics.Models.Orders;

namespace ISSTechLogistics.Repository.OrdersDetails;

public interface IOrdersDetailsStatisticsRepository
{
    /// <summary>
    /// Calculates the orders statistics of a given file
    /// </summary>
    /// <param name="orders">orders list for statistics calculations</param>
    /// <returns>The result of the calculations</returns>
    Task<BaseOutput<OrdersDetailsStatistics>> CalculateOrdersStatistics(IEnumerable<Order> orders);
}