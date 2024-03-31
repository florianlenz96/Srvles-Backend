using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SrvlesBackend.Database.Models;

namespace SrvlesBackend.Functions.ShortenUrl;

public class ShortenUrlResponse
{
    public HttpResponseData Response { get; set; }
    
    [TableOutput("ShortUrlTable", Connection = "AzureWebJobsStorage")]
    public ShortUrlModel ShortUrl { get; set; }
}