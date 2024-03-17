using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SrvlesBackend.Database.Models;

namespace SrvlesBackend.Functions.CreateShortUrl;

public class CreateShortUrlResponse
{
    public HttpResponseData Response { get; set; }
    
    [TableOutput("ShortUrlTable", Connection = "AzureWebJobsStorage")]
    public ShortUrlModel? ShortUrl { get; set; }
}