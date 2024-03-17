using Azure;
using Azure.Data.Tables;

namespace SrvlesBackend.Database.Models;

public class ShortUrlModel : ITableEntity
{
    public ShortUrlModel()
    {
    }
    
    public ShortUrlModel(string originalUrl, string shortUrl)
    {
        this.OriginalUrl = originalUrl;
        this.ShortUrl = shortUrl;
        this.PartitionKey = shortUrl;
        this.RowKey = shortUrl;
    }
    
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string OriginalUrl { get; init; }
    public string ShortUrl { get; init; }
}