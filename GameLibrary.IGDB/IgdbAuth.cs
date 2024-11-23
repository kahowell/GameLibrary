using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text.Json;
using GameLibrary.Core;

namespace GameLibrary.IGDB;

public class IgdbAuth(HttpClient client, Configuration config) : DelegatingHandler
{
    private readonly IgdbOptions _options = config.Get<IgdbOptions>();
    private string ClientId => _options.ClientId;
    private string ClientSecret => _options.ClientSecret;
    private volatile OauthToken? _token = config.Get<IgdbOptions>().Token;
    private readonly object _tokenLock = new();

    public OauthToken GetToken()
    {
        lock (_tokenLock)
        {
            if (_token != null && !_token.IsExpired())
            {
                return _token;
            }

            var task = Task.Run(async () =>
            {
                var response = await client.PostAsync(
                    $"oauth2/token?client_id={ClientId}&client_secret={ClientSecret}&grant_type=client_credentials", null);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<OauthToken>(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });
            });
            task.Wait();
            var token = task.Result;

            if (token is null)
            {
                throw new IOException("Failed to get oauth token");
            }
            _options.Token = token;
            config.Write();
            _token = token;
        }

        return _token;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("Authorization", $"Bearer {GetToken().AccessToken}");
        request.Headers.Add("Client-ID", ClientId);
        return base.SendAsync(request, cancellationToken);
    }
}
