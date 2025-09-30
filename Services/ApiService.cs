using System.Net.Http.Json;

namespace AdminNeco.Services;

public sealed class ApiService
{
    private readonly IHttpClientFactory _httpFactory;

    public ApiService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    private HttpClient Client => _httpFactory.CreateClient("api");

    // --- Helpers ---
    private static async Task<T?> ReadOrNull<T>(HttpResponseMessage res, CancellationToken ct)
    {
        if (!res.IsSuccessStatusCode) return default;
        // اگر سرور JSON برگرداند، این متد کفایت می‌کند
        return await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }


    // --- Auth endpoints (ساده و سرراست) ---

    public async Task<WebLoginResponse?> WebLoginAsync(WebLoginRequest req, CancellationToken ct = default)
    {
        var res = await Client.PostAsJsonAsync(ApiConstants.Auth.WebLogin, req, ct);
        return await ReadOrNull<WebLoginResponse>(res, ct);
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        // نیازی به مقدار برگشتی نیست
        await Client.PostAsync(ApiConstants.Auth.WebLogout, content: null, ct);
    }

    public async Task<TokenSetupResponse?> TokenSetupAsync(CancellationToken ct = default)
    {
        var res = await Client.PostAsync(ApiConstants.Auth.TokenSetup, content: null, ct);
        return await ReadOrNull<TokenSetupResponse>(res, ct);
    }

    public async Task<LoginWithOtpResponse?> LoginWithOtpAsync(LoginWithOtpRequest req, CancellationToken ct = default)
    {
        var res = await Client.PostAsJsonAsync(ApiConstants.Auth.LoginWithOtp, req, ct);
        return await ReadOrNull<LoginWithOtpResponse>(res, ct);
    }

    public async Task<SendOtpResponse?> SendOtpAsync(SendOtpRequest req, CancellationToken ct = default)
    {
        var res = await Client.PostAsJsonAsync(ApiConstants.Auth.SendOtp, req, ct);
        return await ReadOrNull<SendOtpResponse>(res, ct);
    }
}
