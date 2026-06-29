
using Microsoft.AspNetCore.Http;
using TraineeManagement.Contracts.CoorealationIdMiddlewares;
namespace TraineeManagement.Contracts.CoorealationIdServices;

public interface ICorrelationIdAccessor
{
    string? CorrelationId { get; }
}

public class CorrelationIdAccessor : ICorrelationIdAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? CorrelationId =>
        _httpContextAccessor.HttpContext?.Items[CorrelationIdMiddleware.HeaderName] as string;
}