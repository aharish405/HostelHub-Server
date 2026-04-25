namespace HostelHub.Application.Common.Interfaces;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(string userId, string email, IList<string> roles, string? tenantId);
}
