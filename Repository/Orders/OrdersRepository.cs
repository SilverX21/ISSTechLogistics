using System.Globalization;
using System.Net;
using System.Text;
using CsvHelper;
using ISSTechLogistics.Data;
using ISSTechLogistics.Models.Base;
using ISSTechLogistics.Models.Orders;
using ISSTechLogistics.Models.Services;
using ISSTechLogistics.Repository.OrdersDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace ISSTechLogistics.Repository.Orders;

public class OrdersRepository : IOrdersRepository
{
    #region Interfaces and constructors

    private readonly ILogger _logger;
    private readonly ApplicationDbContext _database;
    private readonly IOrdersDetailsStatisticsRepository _ordersDetails;
    private readonly LogisticsTechSettings _settings;

    public OrdersRepository(
        ILogger logger,
        ApplicationDbContext database,
        IOrdersDetailsStatisticsRepository ordersDetails,
        IOptions<LogisticsTechSettings> settings)
    {
        _logger = logger;
        _database = database;
        _ordersDetails = ordersDetails;
        _settings = settings?.Value;
    }

    #endregion Interfaces and constructors

    #region Public methods

    /// <inheritdoc/>
    public async Task<BaseOutput<OrderFileOutput>> ProcessOrdersDetails(IFormFile file)
    {
        try
        {
            var records = new List<Order>();

            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Order>().ToList();
            }

            var ordersResult = await _ordersDetails.CalculateOrdersStatistics(records);

            if (!ordersResult.Success)
                return new BaseOutput<OrderFileOutput>
                {
                    Output = new OrderFileOutput
                    {
                        FileName = file.FileName,
                        Message = ordersResult.ErrorMessage
                    },
                    Status = HttpStatusCode.BadRequest,
                    Success = false,
                    ErrorMessage = "It wasn't possible to calculate the orders statistics. Please try again."
                };

            var fileSaveResult = await SaveOrdersDocument(file);

            return new BaseOutput<OrderFileOutput>
            {
                Output = new OrderFileOutput
                {
                    FileName = fileSaveResult.FileName,
                    FilePath = fileSaveResult.FilePath,
                    Success = fileSaveResult.Success,
                    Message = fileSaveResult.Message
                },
                Status = fileSaveResult.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest,
                Success = fileSaveResult.Success
            };
        }
        catch (Exception ex)
        {
            _logger.Error($"There was an error while trying to process the orders file in {nameof(ProcessOrdersDetails)} at {nameof(OrdersRepository)}. Error: {ex.Message}", ex);
            return new BaseOutput<OrderFileOutput>()
            {
                Output = null,
                Status = HttpStatusCode.InternalServerError,
                Success = false,
                ErrorMessage = "There was an error while trying to process the given document. Please try again."
            };
        }
    }

    /// <inheritdoc/>
    public async Task<BaseOutput<OrdersDetailsStatistics>> GetOrdersStatistics()
    {
        try
        {
            var orderStatistics = _database.OrdersDetailsStatistics.FirstOrDefault(x => x.IsLatest);

            if (orderStatistics is null)
            {
                return new BaseOutput<OrdersDetailsStatistics>()
                {
                    Output = null,
                    Status = HttpStatusCode.BadRequest,
                    Success = false,
                    ErrorMessage = "There was an error while trying to get the statistics for the orders. Please try again."
                };
            }

            return new BaseOutput<OrdersDetailsStatistics>()
            {
                Output = orderStatistics,
                Status = HttpStatusCode.OK,
                Success = true,
                ErrorMessage = "Successfuly retrieved orders statistics."
            };
        }
        catch (Exception ex)
        {
            _logger.Error($"There was an error while trying to process the orders file in {nameof(GetOrdersStatistics)} at {nameof(OrdersRepository)}. Error: {ex.Message}", ex);
            return new BaseOutput<OrdersDetailsStatistics>()
            {
                Output = null,
                Status = HttpStatusCode.InternalServerError,
                Success = false,
                ErrorMessage = "There was an error while trying to get the statistics for the orders. Please try again."
            };
        }
    }

    /// <inheritdoc/>
    public async Task<BaseOutput<OrdersDetailsStatistics>> ReProcessOrdersStatistics()
    {
        try
        {
            var ordersDocument = _database.OrderFiles.FirstOrDefault(x => x.IsLatest);
            var path = Path.Combine(_settings.FileSettings?.FilesSavePath, ordersDocument.FileName);
            var directory = Path.GetDirectoryName(path);
            var fileExists = Directory.GetFiles(directory, "*.csv");

            if (ordersDocument is null || fileExists is null)
                return new BaseOutput<OrdersDetailsStatistics>()
                {
                    ErrorMessage = "The file doesn't exist in the directory.",
                    Output = null,
                    Status = HttpStatusCode.BadRequest,
                    Success = false
                };

            var records = new List<Order>();

            using (var reader = new StreamReader(Path.Combine(_settings.FileSettings?.FilesSavePath, ordersDocument.FileName)))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Order>().ToList();
            }

            var recalculationResult = await _ordersDetails.CalculateOrdersStatistics(records);

            if (recalculationResult is null)
                return new BaseOutput<OrdersDetailsStatistics>()
                {
                    ErrorMessage = "There was an error while trying to calculate the orders statistics. Please try again.",
                    Output = null,
                    Status = HttpStatusCode.BadRequest,
                    Success = false
                };

            return new BaseOutput<OrdersDetailsStatistics>()
            {
                ErrorMessage = "The statistics where recalculated!.",
                Output = recalculationResult.Output,
                Status = HttpStatusCode.OK,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.Error($"There was an error while trying to process the orders file in {nameof(ReProcessOrdersStatistics)} at {nameof(OrdersRepository)}. Error: {ex.Message}", ex);
            return new BaseOutput<OrdersDetailsStatistics>()
            {
                Output = null,
                Status = HttpStatusCode.InternalServerError,
                Success = false,
                ErrorMessage = "There was an error while trying to recalculate the statistics for the orders. Please try again."
            };
        }
    }

    #endregion Public methods

    #region Private methods

    private async Task<OrderFileOutput> SaveOrdersDocument(IFormFile file)
    {
        var fileName = file.FileName.Split(".")[0] + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
        var path = Path.Combine(_settings.FileSettings?.FilesSavePath, fileName);

        try
        {
            var directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var csvFiles = Directory.GetFiles(directory, "*.csv");

            if (csvFiles.Count() > 0)
            {
                foreach (var csvFile in csvFiles)
                {
                    File.Delete(csvFile);
                }
            }

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if (!File.Exists(path))
                return new OrderFileOutput
                {
                    FilePath = path,
                    FileName = fileName,
                    Message = "There was an error while trying to save the orders file!",
                    Success = false
                };

            var latestFile = await _database.OrderFiles.FirstOrDefaultAsync(x => x.IsLatest);

            if (latestFile is not null)
            {
                latestFile.IsLatest = false;
                latestFile.DateUpdated = DateTime.Now;
            }

            _database.Add(new OrderFile
            {
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                FileName = fileName,
                FilePath = path,
                IsLatest = true,
            });
            _database.SaveChanges();

            return new OrderFileOutput
            {
                FilePath = path,
                FileName = fileName,
                Message = "File saved successfuly!",
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.Error($"There was an error while trying to save the orders file in {nameof(SaveOrdersDocument)} at {nameof(OrdersRepository)}. Error: {ex.Message}", ex);
            return new OrderFileOutput
            {
                FilePath = path,
                FileName = fileName,
                Message = "There was an error while trying to save the file!",
                Success = false
            };
        }
    }

    #endregion Private methods
}