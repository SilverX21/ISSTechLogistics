using ISSTechLogistics.Models.Base;
using ISSTechLogistics.Models.Orders;

namespace ISSTechLogistics.Repository.Orders;

public interface IOrdersRepository
{
    /// <summary>
    /// Saves the orders details in the database
    /// </summary>
    /// <param name="file">CSV file with the orders details</param>
    /// <returns>Details of the process</returns>
    Task<BaseOutput<OrderFileOutput>> ProcessOrdersDetails(IFormFile file);

    /// <summary>
    /// Gets the orders statistics like: Total shipments, average weight and average delivery time
    /// </summary>
    /// <returns>Statistics of calculations for the order details file</returns>
    Task<BaseOutput<OrdersDetailsStatistics>> GetOrdersStatistics();

    /// <summary>
    /// Re-process the statistics to recalculate the new data inserted in the file
    /// </summary>
    /// <returns>Statistics of the current order details file</returns>
    Task<BaseOutput<OrdersDetailsStatistics>> ReProcessOrdersStatistics();
}