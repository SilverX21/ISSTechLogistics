using System.Net;
using ISSTechLogistics.Data;
using ISSTechLogistics.Models.Base;
using ISSTechLogistics.Models.Orders;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace ISSTechLogistics.Repository.OrdersDetails;

public class OrdersDetailsStatisticsRepository : IOrdersDetailsStatisticsRepository
{
    #region Interfaces and constructors

    private readonly ILogger _logger;
    private readonly ApplicationDbContext _database;

    public OrdersDetailsStatisticsRepository(
        ILogger logger,
        ApplicationDbContext database)
    {
        _logger = logger;
        _database = database;
    }

    #endregion Interfaces and constructors

    #region Public methods

    /// <inheritdoc/>
    public async Task<BaseOutput<OrdersDetailsStatistics>> CalculateOrdersStatistics(IEnumerable<Order> orders)
    {
        try
        {
            if (orders.Count() <= 0)
                return new BaseOutput<OrdersDetailsStatistics>()
                {
                    Output = null,
                    ErrorMessage = "There are no orders in the given file, please check if there's data missing. Please try again.",
                    Success = false,
                    Status = HttpStatusCode.BadRequest
                };

            var ordersDetails = new OrdersDetailsStatistics
            {
                AverageDeliveryTime = Math.Round((decimal)orders.Average(x => x.DeliveryTime), 2),
                AverageWeight = Math.Round(orders.Average(x => x.Weight), 3),
                CalculationDate = DateTime.Now,
                IsLatest = true,
                TotalOrders = orders.Count(),
            };

            var latestCalculation = await _database.OrdersDetailsStatistics.FirstOrDefaultAsync(x => x.IsLatest);

            if (latestCalculation is not null)
            {
                latestCalculation.IsLatest = false;
            }

            _database.Add(ordersDetails);
            _database.SaveChanges();

            return new BaseOutput<OrdersDetailsStatistics>()
            {
                Output = ordersDetails,
                ErrorMessage = string.Empty,
                Status = HttpStatusCode.OK,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.Error($"There was an error while trying to process the orders file in {nameof(CalculateOrdersStatistics)} at {nameof(OrdersDetailsStatisticsRepository)}. Error: {ex.Message}", ex);
            return new BaseOutput<OrdersDetailsStatistics>()
            {
                Output = null,
                ErrorMessage = "There was an error while trying to make the calculations. Please try again.",
                Success = false,
                Status = System.Net.HttpStatusCode.InternalServerError
            };
        }
    }

    #endregion Public methods
}