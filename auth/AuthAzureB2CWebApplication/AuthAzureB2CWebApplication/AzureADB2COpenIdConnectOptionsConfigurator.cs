using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthAzureB2CWebApplication
{
    public class AzureADB2COpenIdConnectOptionsConfigurator : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly AzureADB2CWithApiOptions _options;

        public AzureADB2COpenIdConnectOptionsConfigurator(IOptions<AzureADB2CWithApiOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            options.Events.OnAuthorizationCodeReceived = WrapOpenIdConnectEvent(options.Events.OnAuthorizationCodeReceived, OnAuthorizationCodeReceived);
            options.Events.OnRedirectToIdentityProvider = WrapOpenIdConnectEvent(options.Events.OnRedirectToIdentityProvider, OnRedirectToIdentityProvider);
        }

        public void Configure(OpenIdConnectOptions options)
        {
            Configure(Options.DefaultName, options);
        }

        private static Func<TContext, Task> WrapOpenIdConnectEvent<TContext>(Func<TContext, Task> baseEventHandler, Func<TContext, Task> thisEventHandler)
        {
            return new Func<TContext, Task>(async context =>
            {
                await baseEventHandler(context);
                await thisEventHandler(context);
            });
        }

        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            //var clientCredential = new ClientCredential(context.Options.ClientSecret);
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(context.Options.ClientId)
                .WithClientSecret(context.Options.ClientSecret)
                .WithAuthority(context.Options.Authority)
                .WithRedirectUri(_options.RedirectUri)
                .Build();
            //var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var tokenCache = new SessionTokenCache(context.HttpContext, userId);

            //var confidentialClientApplication = new ConfidentialClientApplication(
            //    context.Options.ClientId,
            //    context.Options.Authority,
            //    _options.RedirectUri,
            //    clientCredential,
            //    tokenCache.GetInstance(),
            //    null);

            try
            {
                var authenticationResult =  confidentialClientApplication.AcquireTokenByAuthorizationCode(_options.ApiScopes.Split(' '), context.ProtocolMessage.Code);
                //context.HandleCodeRedemption(authenticationResult.AccessToken, authenticationResult.IdToken);
            }
            catch (Exception ex)
            {
                // TODO: Handle
                throw;
            }
        }

        private Task OnRedirectToIdentityProvider(RedirectContext context)
        {
            context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
            context.ProtocolMessage.Scope += $" offline_access {_options.ApiScopes}";
            return Task.FromResult(0);
        }
    }
}
