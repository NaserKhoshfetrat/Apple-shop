using AppleShop.Services.Identity;
using AppleShop.Services.Identity.Models;
using AppleShop.Services.Identity.Pages.Account.Login;
using Duende.IdentityModel;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AppleShop.Services.Identity.Pages.Account.Register
{
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public partial class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityCustomUser> _userManager;
        private readonly SignInManager<IdentityCustomUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IIdentityProviderStore _identityProviderStore;
        private readonly IClientStore _clientStore;

        [BindProperty]
        public RegisterViewModel View { get; set; } = default!;

        public RegisterModel(UserManager<IdentityCustomUser> userManager, SignInManager<IdentityCustomUser> signInManager, RoleManager<IdentityRole> roleManager, IIdentityServerInteractionService interaction, IEventService events, IAuthenticationSchemeProvider schemeProvider, IIdentityProviderStore identityProviderStore, IClientStore clientStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _interaction = interaction;
            _events = events;
            _schemeProvider = schemeProvider;
            _identityProviderStore = identityProviderStore;
            _clientStore = clientStore;
        }

        public async Task<IActionResult> OnGetAsync(string? returnUrl, CancellationToken ct)
        {
            // build a model so we know what to show on the reg page
            View = await BuildRegisterViewModelAsync(returnUrl, ct);

            return Page();
        }

        //Register
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(CancellationToken ct)
        {
            ViewData["ReturnUrl"] = View.ReturnUrl;
            if (ModelState.IsValid)
            {

                var user = new IdentityCustomUser
                {
                    UserName = View.Username,
                    Email = View.Email,
                    EmailConfirmed = true,
                    FirstName = View.FirstName,
                    LastName = View.LastName
                };

                var result = await _userManager.CreateAsync(user, View.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(View.RoleName).GetAwaiter().GetResult())
                    {
                        var userRole = new IdentityRole
                        {
                            Name = View.RoleName,
                            NormalizedName = View.RoleName,

                        };
                        await _roleManager.CreateAsync(userRole);
                    }

                    await _userManager.AddToRoleAsync(user, View.RoleName);

                    await _userManager.AddClaimsAsync(user, new Claim[]{
                            new Claim(JwtClaimTypes.Name, View.Username),
                            new Claim(JwtClaimTypes.Email, View.Email),
                            new Claim(JwtClaimTypes.FamilyName, View.FirstName),
                            new Claim(JwtClaimTypes.GivenName, View.LastName),
                            new Claim(JwtClaimTypes.WebSite, "http://"+View.Username+".com"),
                            new Claim(JwtClaimTypes.Role,"User") });

                    var context = await _interaction.GetAuthorizationContextAsync(View.ReturnUrl, ct);
                    var loginresult = await _signInManager.PasswordSignInAsync(View.Username, View.Password, false, lockoutOnFailure: true);
                    if (loginresult.Succeeded)
                    {
                        var checkuser = await _userManager.FindByNameAsync(View.Username);
                        await _events.RaiseAsync(new UserLoginSuccessEvent(checkuser.UserName, checkuser.Id, checkuser.UserName, clientId: context?.Client.ClientId), ct);

                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                // The client is native, so this change in how to
                                // return the response is for better UX for the end user.
                                return Redirect(View.ReturnUrl ?? "~/");
                                //return this.LoadingPage("Redirect", model.ReturnUrl);
                            }

                            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                            return Redirect(View.ReturnUrl);
                        }

                        // request for a local page
                        if (Url.IsLocalUrl(View.ReturnUrl))
                        {
                            return Redirect(View.ReturnUrl);
                        }
                        else if (string.IsNullOrEmpty(View.ReturnUrl))
                        {
                            return Redirect("~/");
                        }
                        else
                        {
                            // user might have clicked on a malicious link - should be logged
                            throw new Exception("invalid return URL");
                        }
                    }

                } else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form

            // Rebuild the View model to show the form again with validation errors
            View = await BuildRegisterViewModelAsync(View.ReturnUrl, ct);
            // Repopulate the role list for the dropdown
            //List<string> roles = new() { ProjectConfigurations.Role.Admin, ProjectConfigurations.Role.Customer };
            //ViewData["message"] = roles;
            return Page();
        }

        private async Task<RegisterViewModel> BuildRegisterViewModelAsync(string returnUrl, CancellationToken ct)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl, ct);
            List<string> roles = [ProjectConfigurations.Role.Admin, ProjectConfigurations.Role.Customer];
            //ViewBag.message = roles;
            ViewData["message"] = roles;
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new RegisterViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { 
                        new ViewModel.ExternalProvider(authenticationScheme: context.IdP)
                    };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ViewModel.ExternalProvider(authenticationScheme: x.Name, displayName: x.DisplayName ?? x.Name)
                ).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId, ct);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new RegisterViewModel
            {
                AllowRememberLogin = LoginOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }
    }
}
