using Azure;
using Azure.Data.Tables;

namespace SrvlesBackend.Database.Models;

public class ShortUrlModel : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string OriginUrl { get; init; }
    public string ShortUrl { get; init; }
}