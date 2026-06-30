using System.Net;
using System.Text.Json;
using Polly.CircuitBreaker;
using Polly.Timeout;
using TraineeManagement.WebCommons.CoorealationIdMiddlewares;
using TraineeManagement.WebCommons.CoorealationIdServices;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;

namespace TraineeManagement.Api.HttpServices;

public interface IInterServiceHttpClient
{
    Task<TResponse?> GetAsync<TResponse>(
        string clientName,
        string requestUri,
        CancellationToken cancellationToken);
}

public class InterServiceHttpClient : IInterServiceHttpClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;

    private readonly ILogger<InterServiceHttpClient> _logger;

    public InterServiceHttpClient(
        IHttpClientFactory httpClientFactory,
        ICorrelationIdAccessor correlationIdAccessor,
        ILogger<InterServiceHttpClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _correlationIdAccessor = correlationIdAccessor;
        _logger = logger;
    }

    public async Task<TResponse?> GetAsync<TResponse>(
        string clientName,
        string requestUri,
        CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient(clientName);

        using HttpRequestMessage request = new(HttpMethod.Get, requestUri);

        string? correlationId = _correlationIdAccessor.CorrelationId;
        if (!string.IsNullOrEmpty(correlationId))
        {
            request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, correlationId);
        }

        try
        {
            using HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web),
                cancellationToken);
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex,"Timeout occurred while making inter-service request to {ClientName} at {RequestUri}", clientName, requestUri);
            throw new InterServiceOperationExeception(ErrorCodes.INTER_SERVICE_FAILED);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit breaker is open for inter-service request to {ClientName} at {RequestUri}", clientName, requestUri);
            throw new InterServiceOperationExeception(ErrorCodes.INTER_SERVICE_FAILED);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error occurred while making inter-service request to {ClientName} at {RequestUri}", clientName, requestUri);
            throw new InterServiceOperationExeception(ErrorCodes.INTER_SERVICE_FAILED);
        }
    }
}