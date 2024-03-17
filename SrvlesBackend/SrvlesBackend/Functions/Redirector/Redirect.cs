using System.Net;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SrvlesBackend.Database.Models;

namespace SrvlesBackend.Functions.Redirector;

public class Redirect
{
    private readonly ILogger _logger;

    public Redirect(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Redirect>();
    }

    [Function("Redirect")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{id}")] HttpRequestData req, string id,
        [TableInput("ShortUrlTable", Connection = "AzureWebJobsStorage")] TableClient shortUrlTable,
        FunctionContext executionContext)
    {
        var shortUrl = await shortUrlTable.GetEntityIfExistsAsync<ShortUrlModel>(id, id);
        
        if (!shortUrl.HasValue)
        {
            _logger.LogInformation("Short URL not found: {Id}", id);
            return req.CreateResponse(HttpStatusCode.NotFound);
        }
        
        _logger.LogInformation("Redirecting to: {ShortUrlOriginalUrl}", shortUrl.Value.OriginalUrl);
        var response = req.CreateResponse(HttpStatusCode.PermanentRedirect);
        response.Headers.Add("Location", shortUrl.Value.OriginalUrl);

        return response;
    }
}