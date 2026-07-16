using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace recipe_suggestions.Data;

public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    public ApplicationUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        var displayName = !string.IsNullOrWhiteSpace(user.DisplayName)
            ? user.DisplayName.Trim()
            : user.UserName;

        if (!string.IsNullOrWhiteSpace(displayName))
        {
            identity.AddClaim(new Claim("DisplayName", displayName));

            // Keep the primary name claim as the friendly username (not email).
            var existingName = identity.FindFirst(ClaimTypes.Name);
            if (existingName != null)
                identity.RemoveClaim(existingName);

            identity.AddClaim(new Claim(ClaimTypes.Name, displayName));
        }

        return identity;
    }
}
