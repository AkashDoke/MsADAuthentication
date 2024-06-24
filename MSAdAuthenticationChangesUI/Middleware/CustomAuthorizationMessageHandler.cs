using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;

namespace MSAdAuthenticationChangesUI.Middleware
{
    public class CustomAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly ILogger<CustomAuthorizationMessageHandler> _logger;

        public CustomAuthorizationMessageHandler(IAccessTokenProvider tokenProvider, ILogger<CustomAuthorizationMessageHandler> logger)
        {
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tokenResult = await _tokenProvider.RequestAccessToken(new AccessTokenRequestOptions { Scopes = new[] { "api://c4fa3702-09f5-484a-80d0-36ee63593e95/user_impersonation" } });

            if (tokenResult.TryGetToken(out var token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            }
            else
            {
                _logger.LogWarning("Failed to acquire access token.");
            }

            var response = await base.SendAsync(request, cancellationToken);

            //if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //{
            //    _logger.LogInformation("Access token expired. Attempting to refresh token.");

            //    tokenResult = await _tokenProvider.ForceRefresh(new AccessTokenRequestOptions { Scopes = new[] { "api://c4fa3702-09f5-484a-80d0-36ee63593e95/user_impersonation" }, ForceRefresh = true });

            //    if (tokenResult.TryGetToken(out token))
            //    {
            //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            //        response = await base.SendAsync(request, cancellationToken);
            //    }
            //    else
            //    {
            //        _logger.LogError("Failed to refresh access token.");
            //    }
            //}

            return response;
        }
    }
}
