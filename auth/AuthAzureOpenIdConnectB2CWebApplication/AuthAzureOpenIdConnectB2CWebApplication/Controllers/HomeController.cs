using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Client;
using System.Security.Claims;
using WebApp_OpenIDConnect_DotNet.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace WebApp_OpenIDConnect_DotNet.Controllers
{
    public class HomeController : Controller
    {
        AzureAdB2COptions AzureAdB2COptions;
        public HomeController(IOptions<AzureAdB2COptions> azureAdB2COptions)
        {
            AzureAdB2COptions = azureAdB2COptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            var token = await GetAuthenticationTokenAsync();
            HttpContext.Response.Headers.Add("Authorization", $"Bearer {token}");
            return View();
        }

        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = String.Format("Claims available for the user {0}", (User.FindFirst("name")?.Value));
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Api()
        {
            string responseString = "";
            try
            {
                var token = await GetAuthenticationTokenAsync();

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, AzureAdB2COptions.ApiUrl);

                // Add token to the Authorization header and make the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        responseString = await response.Content.ReadAsStringAsync();
                        break;
                    case HttpStatusCode.Unauthorized:
                        responseString = $"Please sign in again. {response.ReasonPhrase}";
                        break;
                    default:
                        responseString = $"Error calling API. StatusCode=${response.StatusCode}";
                        break;
                }
            }
            catch (MsalUiRequiredException ex)
            {
                responseString = $"Session has expired. Please sign in again. {ex.Message}";
            }
            catch (Exception ex)
            {
                responseString = $"Error calling API: {ex.Message}";
            }

            ViewData["Payload"] = $"{responseString}";            
            return View();
        }

        public IActionResult Error(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        private async Task<string> GetAuthenticationTokenAsync()
        {
            if (!User.Identity.IsAuthenticated) return null;

            // get from cookie
            var identifier = User.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault()?.Value;
            var ckey = $".tmp_aspnet_jwtaccesstoken_{identifier}";
            if (HttpContext.Request.Cookies.TryGetValue(ckey, out var value))
            {
                return value;
            }
            else
            {
                // get from inmemory user cache
                var result = await GetAuthenticationResultAsync();
                if (result != null)
                {
                    return result.AccessToken;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve the token with the specified scopes
        /// </summary>
        /// <returns></returns>
        private async Task<AuthenticationResult> GetAuthenticationResultAsync()
        {
            var scope = AzureAdB2COptions.ApiScopes.Split(' ');
            string signedInUserID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            IConfidentialClientApplication cca = ConfidentialClientApplicationBuilder.Create(AzureAdB2COptions.ClientId)
                .WithRedirectUri(AzureAdB2COptions.RedirectUri)
                .WithClientSecret(AzureAdB2COptions.ClientSecret)
                .WithB2CAuthority(AzureAdB2COptions.Authority)
                .Build();
            new MSALStaticCache(signedInUserID, this.HttpContext).EnablePersistence(cca.UserTokenCache);

            var accounts = await cca.GetAccountsAsync();
            AuthenticationResult result = accounts != null && accounts.Any()
                ? await cca.AcquireTokenSilent(scope, accounts.FirstOrDefault()).ExecuteAsync()
                : null;
            return result;
        }
    }
}
