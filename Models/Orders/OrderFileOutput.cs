namespace ISSTechLogistics.Models.Orders;

public class OrderFileOutput
{
    public string FileName { get; set; }

    public string FilePath { get; set; }

    public string Message { get; set; }

    public bool Success { get; set; }
}