using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AdminNeco.Services
{
    public sealed class ApiService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly AppState _state;

        public ApiService(IHttpClientFactory httpFactory, AppState state)
        {
            _httpFactory = httpFactory;
            _state = state;
        }

        private HttpClient CreateClient()
        {
            var c = _httpFactory.CreateClient("api");
            // همیشه همین‌جا هدر Authorization را ست کن
            c.DefaultRequestHeaders.Authorization = null;

            var auth = _state.AuthorizationValue; // "Bearer token[:username]"
            if (!string.IsNullOrWhiteSpace(auth))
            {
                var param = auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? auth.Substring("Bearer ".Length)
                    : auth;
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", param);
            }
            return c;
        }

        private static async Task<T?> ReadOrNull<T>(HttpResponseMessage res, CancellationToken ct)
        {
            if (!res.IsSuccessStatusCode) return default;
            return await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
        }

        // ===== Auth =====
        public async Task<WebLoginResponse?> WebLoginAsync(WebLoginRequest req, CancellationToken ct = default)
        {
            var client = CreateClient();
            var res = await client.PostAsJsonAsync(ApiConstants.Auth.WebLogin, req, ct);
            var data = await ReadOrNull<WebLoginResponse>(res, ct);

            // اگر توکن برگشت، همون لحظه ذخیره کنیم تا بعدش APIها هدر داشته باشند
            if (data is not null)
            {
                var token = data.Token ?? (data.data is string s ? s : null);
                var user = data.MyUser?.Username ?? data.MyUser?.Name ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(token))
                    await _state.SetIdentityAsync(token, user);
            }
            return data;
        }

        public async Task LogoutAsync(CancellationToken ct = default)
        {
            var client = CreateClient();
            await client.PostAsync(ApiConstants.Auth.WebLogout, content: null, ct);
            await _state.ClearIdentityAsync();
        }

        public async Task<TokenSetupResponse?> TokenSetupAsync(CancellationToken ct = default)
        {
            var client = CreateClient();
            var res = await client.PostAsync(ApiConstants.Auth.TokenSetup, content: null, ct);
            return await ReadOrNull<TokenSetupResponse>(res, ct);
        }

        public async Task<LoginWithOtpResponse?> LoginWithOtpAsync(LoginWithOtpRequest req, CancellationToken ct = default)
        {
            var client = CreateClient();
            var res = await client.PostAsJsonAsync(ApiConstants.Auth.LoginWithOtp, req, ct);
            var data = await ReadOrNull<LoginWithOtpResponse>(res, ct);

            if (data?.MyUser is not null && !string.IsNullOrWhiteSpace(_state.Token))
                await _state.SetIdentityAsync(_state.Token, data.MyUser.Username);
            return data;
        }

        public async Task<SendOtpResponse?> SendOtpAsync(SendOtpRequest req, CancellationToken ct = default)
        {
            var client = CreateClient();
            var res = await client.PostAsJsonAsync(ApiConstants.Auth.SendOtp, req, ct);
            return await ReadOrNull<SendOtpResponse>(res, ct);
        }

        // ===== ProgramType =====
        public async Task<List<ProgramTypeDto>?> GetAllProgramTypesAsync(CancellationToken ct = default)
        {
            var client = CreateClient();
            var res = await client.PostAsync(ApiConstants.ProgramType.GetAll, content: null, ct);
            return await ReadOrNull<List<ProgramTypeDto>>(res, ct);
        }

        public async Task<ProgramTypeDto?> InsertProgramTypeAsync(ProgramTypeDto dto, CancellationToken ct = default)
        {
            var client = CreateClient();
            var res = await client.PostAsJsonAsync(ApiConstants.ProgramType.Insert, dto, ct);
            return await ReadOrNull<ProgramTypeDto>(res, ct);
        }

        public async Task<ProgramTypeDto?> UpdateProgramTypeAsync(ProgramTypeDto dto, CancellationToken ct = default)
        {
            var client = CreateClient();
            var res = await client.PostAsJsonAsync(ApiConstants.ProgramType.Update, dto, ct);
            return await ReadOrNull<ProgramTypeDto>(res, ct);
        }

        public async Task<bool> DeleteProgramTypeAsync(int id, CancellationToken ct = default)
        {
            var client = CreateClient();
            var res = await client.PostAsJsonAsync(ApiConstants.ProgramType.Delete, new { ID = id }, ct);
            return res.IsSuccessStatusCode;
        }
    }
}
