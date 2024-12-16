namespace ISSTechLogistics.Models.Orders;

public class OrderFile
{
    public int Id { get; set; }

    public string FileName { get; set; }

    public string FilePath { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public bool IsLatest { get; set; }
}