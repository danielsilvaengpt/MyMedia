using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;

namespace MyMedia.Web.Client
{
 public class AuthTokenHandler : DelegatingHandler
 {
 private readonly ILocalStorageService _localStorage;
 private readonly ILogger<AuthTokenHandler> _logger;

 public AuthTokenHandler(ILocalStorageService localStorage, ILogger<AuthTokenHandler> logger)
 {
 _localStorage = localStorage;
 _logger = logger;
 }

 protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
 {
 var token = await _localStorage.GetItemAsStringAsync("authToken");
 if (!string.IsNullOrWhiteSpace(token))
 {
 token = token.Trim('"');
 request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

 // Log masked token for debugging (do not log full token)
 try
 {
 if (token.Length >12)
 {
 var masked = token.Substring(0,6) + "..." + token.Substring(token.Length -6);
 _logger.LogInformation("AuthTokenHandler: attached Authorization header. Token (masked)={Token}", masked);
 }
 else
 {
 _logger.LogInformation("AuthTokenHandler: attached Authorization header.");
 }
 }
 catch
 {
 // Swallow logging errors
 }
 }
 else
 {
 _logger.LogInformation("AuthTokenHandler: no token in local storage");
 }

 return await base.SendAsync(request, cancellationToken);
 }
 }
}
