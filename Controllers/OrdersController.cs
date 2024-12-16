using ISSTechLogistics.Repository.Orders;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ISSTechLogistics.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class OrdersController : ControllerBase
{
    #region Interfaces and constructor

    private readonly IOrdersRepository _ordersRepository;

    public OrdersController(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    #endregion Interfaces and constructor

    /// <summary>
    /// Gets the statistics of the orders file
    /// </summary>
    /// <returns>Statistics of the orders</returns>
    [HttpGet]
    public async Task<IActionResult> GetOrderStatistics()
    {
        var result = await _ordersRepository.GetOrdersStatistics();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Process the details of a given .csv file
    /// </summary>
    /// <param name="file">.csv file type</param>
    /// <returns>details about the file</returns>
    [HttpPost]
    public async Task<IActionResult> ProcessOrders(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Please insert a .csv file to process the orders details." });

        var result = await _ordersRepository.ProcessOrdersDetails(file);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Recalculates the orders statistics
    /// </summary>
    /// <returns>current orders statistics</returns>
    [HttpGet]
    public async Task<IActionResult> RecalculateOrdersStatistics()
    {
        var result = await _ordersRepository.ReProcessOrdersStatistics();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}