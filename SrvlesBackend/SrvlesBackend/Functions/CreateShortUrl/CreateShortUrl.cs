using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SrvlesBackend.Database.Models;

namespace SrvlesBackend.Functions.CreateShortUrl;

public class CreateShortUrl
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public CreateShortUrl(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _logger = loggerFactory.CreateLogger<CreateShortUrl>();
    }

    [Function("CreateShortUrl")]
    public async Task<CreateShortUrlResponse> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "api/shortUrl")] HttpRequestData req,
        [TableInput("ShortUrlTable", Connection = "AzureWebJobsStorage")] TableClient shortUrlTable,
        FunctionContext executionContext)
    {
        var request = JsonSerializer.Deserialize<CreateShortUrlRequest>(req.Body);
        var host = _configuration["HostName"];
        
        var shortenId = GenerateRandomUrl();
        var shortUrl = await shortUrlTable.GetEntityIfExistsAsync<ShortUrlModel>(shortenId, shortenId);
        var i = 0;

        while (shortUrl.HasValue && i < 5)
        {
            _logger.LogInformation("Try: {Try} - ID {ShortenId} already exists, generating a new one", i++, shortenId);
            shortenId = GenerateRandomUrl();
            shortUrl = shortUrlTable.GetEntity<ShortUrlModel>(shortenId, shortenId);
        }
        
        if (i >= 5)
        {
            _logger.LogError("Failed to generate a unique ID for the shortened URL. Please try again");
            var internalServerErrorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            internalServerErrorResponse.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            await internalServerErrorResponse.WriteStringAsync("Failed to generate a unique ID for the shortened URL");
            return new CreateShortUrlResponse
            {
                Response = internalServerErrorResponse,
                ShortUrl = null,
            };
        }
        
        _logger.LogInformation($"Shortening URL: {request.OriginUrl} to {host}/{shortenId}");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        await response.WriteStringAsync($"{host}/{shortenId}");

        return new CreateShortUrlResponse
        {
            Response = response,
            ShortUrl = new ShortUrlModel(request.OriginUrl, shortenId),
        };
    }

    private string GenerateRandomUrl()
    {
        var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = new string(Enumerable.Range(0, 6).Select(_ => allowedChars[Random.Shared.Next(0, allowedChars.Length)]).ToArray());
        return result;
    }
}