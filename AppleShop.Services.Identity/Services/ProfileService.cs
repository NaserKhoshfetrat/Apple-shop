using AppleShop.Services.Identity.Models;
using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AppleShop.Services.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<IdentityCustomUser> _userClaimsPrincipalFactory;
        private readonly UserManager<IdentityCustomUser> _userMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public ProfileService(
            UserManager<IdentityCustomUser> userMgr,
            RoleManager<IdentityRole> roleMgr,
            IUserClaimsPrincipalFactory<IdentityCustomUser> userClaimsPrincipalFactory)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context, CancellationToken ct)
        {
            /*Add three roles 1-FamilyName, 2-GivenName and 3-Role to payload data*/
            
            //userId of logged in user
            string subject = context.Subject.GetSubjectId();
            IdentityCustomUser user = await _userMgr.FindByIdAsync(subject);
            ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);

            List<Claim> claims = userClaims.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
            if (_userMgr.SupportsUserRole)
            {
                IList<string> roles = await _userMgr.GetRolesAsync(user);
                foreach (var rolename in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, rolename));
                    if (_roleMgr.SupportsRoleClaims)
                    {
                        IdentityRole role = await _roleMgr.FindByNameAsync(rolename);
                        if (role != null)
                        {
                            claims.AddRange(await _roleMgr.GetClaimsAsync(role));
                        }
                    }
                }
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context, CancellationToken ct)
        {
            string sub = context.Subject.GetSubjectId();
            IdentityCustomUser user = await _userMgr.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
