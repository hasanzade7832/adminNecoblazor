using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace AdminNeco.Services
{
    public class AppState
    {
        private readonly IJSRuntime _js;
        public AppState(IJSRuntime js) => _js = js;

        private const string Key = "app.isLoggedIn";

        public bool IsLoggedIn { get; private set; }

        public event Action? Changed;
        private void Notify() => Changed?.Invoke();

        /// <summary>
        /// مقداردهی از localStorage (فقط بعد از اولین رندر کلاینتی صدا زده شود)
        /// </summary>
        public async Task InitializeAsync()
        {
            var val = await _js.InvokeAsync<string?>("localStorage.getItem", Key);
            IsLoggedIn = val == "1";
            Notify();
        }

        /// <summary>
        /// ست/پاک کردن وضعیت لاگین + ذخیره/حذف در localStorage
        /// </summary>
        public async Task SetLoggedInAsync(bool value)
        {
            IsLoggedIn = value;

            if (value)
                await _js.InvokeVoidAsync("localStorage.setItem", Key, "1");
            else
                await _js.InvokeVoidAsync("localStorage.removeItem", Key);

            Notify();
        }
    }
}
