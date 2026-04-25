using Microsoft.AspNetCore.Identity;

namespace HostelHub.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // Explicit Tenant isolation binding on the User account directly
    public string? TenantId { get; set; } 
}
