using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace AdminNeco.Services
{
    public class AppState
    {
        private readonly IJSRuntime _js;
        public AppState(IJSRuntime js) => _js = js;

        private const string KeyIsLoggedIn = "app.isLoggedIn";
        private const string KeyUserName = "app.userName";
        private const string KeyToken = "app.token";

        public bool IsLoggedIn { get; private set; }
        public string? Token { get; private set; }
        public string? Username { get; private set; }

        // برای سازگاری با Login.razor فعلی
        public string? AuthToken => Token;

        /// مقدار مناسب برای هدر Authorization → "Bearer {token}:{username}"
        public string AuthorizationValue =>
            string.IsNullOrWhiteSpace(Token)
                ? string.Empty
                : $"Bearer {Token}{(string.IsNullOrWhiteSpace(Username) ? "" : ":" + Username)}";

        public event Action? Changed;
        private void Notify() => Changed?.Invoke();

        public async Task InitializeAsync()
        {
            var v = await _js.InvokeAsync<string?>("localStorage.getItem", KeyIsLoggedIn);
            IsLoggedIn = v == "1";
            await TryLoadIdentityFromCookiesAsync();
            Notify();
        }

        public async Task SetLoggedInAsync(bool value)
        {
            IsLoggedIn = value;
            if (value)
                await _js.InvokeVoidAsync("localStorage.setItem", KeyIsLoggedIn, "1");
            else
                await _js.InvokeVoidAsync("localStorage.removeItem", KeyIsLoggedIn);
            Notify();
        }

        /// ذخیره هویت بعد از لاگین (storage + کوکی‌های قابل‌خواندن)
        public async Task SetIdentityAsync(string? token, string? username)
        {
            Token = string.IsNullOrWhiteSpace(token) ? null : token;
            Username = string.IsNullOrWhiteSpace(username) ? null : username;

            if (Token is null)
                await _js.InvokeVoidAsync("localStorage.removeItem", KeyToken);
            else
                await _js.InvokeVoidAsync("localStorage.setItem", KeyToken, Token);

            if (Username is null)
                await _js.InvokeVoidAsync("localStorage.removeItem", KeyUserName);
            else
                await _js.InvokeVoidAsync("localStorage.setItem", KeyUserName, Username);

            // کوکی (برای سازگاری با بک‌اند/پروکسی)
            if (Token is not null)
                await _js.InvokeVoidAsync("eval", $"document.cookie='token={Token}; path=/'");
            if (Username is not null)
                await _js.InvokeVoidAsync("eval", $"document.cookie='username={Username}; path=/'");

            Notify();
        }

        /// تلاش برای خواندن token/username از storage و سپس از cookie
        public async Task<bool> TryLoadIdentityFromCookiesAsync()
        {
            string? token = await _js.InvokeAsync<string?>("localStorage.getItem", KeyToken);
            string? user = await _js.InvokeAsync<string?>("localStorage.getItem", KeyUserName);

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(user))
            {
                try
                {
                    var all = await _js.InvokeAsync<string>("eval", "document.cookie ?? ''");
                    string? Read(string key) =>
                        all.Split(';', StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => s.Trim())
                           .FirstOrDefault(s => s.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                           ?.Substring(key.Length + 1);

                    token ??= Read("token");
                    user ??= Read("username") ?? Read("UserName") ?? Read("user");
                }
                catch { }
            }

            bool changed = false;
            if (!string.IsNullOrWhiteSpace(token) && token != Token) { Token = token; changed = true; }
            if (!string.IsNullOrWhiteSpace(user) && user != Username) { Username = user; changed = true; }
            if (changed) Notify();
            return changed;
        }

        public async Task ClearIdentityAsync()
        {
            Token = null; Username = null;
            await _js.InvokeVoidAsync("localStorage.removeItem", KeyToken);
            await _js.InvokeVoidAsync("localStorage.removeItem", KeyUserName);
            try { await _js.InvokeVoidAsync("eval", "document.cookie='token=; Max-Age=0; path=/'"); } catch { }
            try { await _js.InvokeVoidAsync("eval", "document.cookie='username=; Max-Age=0; path=/'"); } catch { }
            Notify();
        }

        // ALIAS ها برای سازگاری با کد فعلی
        public Task SetAuthIdentityAsync(string? token, string? username) => SetIdentityAsync(token, username);
        public Task<bool> TryLoadTokenFromCookiesAsync() => TryLoadIdentityFromCookiesAsync();
        public Task SetAuthTokenAsync(string? token) => SetIdentityAsync(token, Username);
    }
}
