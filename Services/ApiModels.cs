using System.Text.Json.Serialization;

namespace AdminNeco.Services
{
    // ===== AUTH =====
    public sealed class WebLoginRequest
    {
        [JsonPropertyName("UserName")] public string UserName { get; set; } = "";
        [JsonPropertyName("Password")] public string Password { get; set; } = "";
        [JsonPropertyName("SeqKey")] public string SeqKey { get; set; } = "";
    }

    public sealed class AppSetting
    {
        [JsonPropertyName("ID")] public int ID { get; set; }
        [JsonPropertyName("Name")] public string Name { get; set; } = "";
        [JsonPropertyName("LetterBtns")] public string? LetterBtns { get; set; }
    }

    public sealed class MyUser
    {
        [JsonPropertyName("ID")] public string ID { get; set; } = "";
        [JsonPropertyName("TTKK")] public string TTKK { get; set; } = "";
        [JsonPropertyName("Username")] public string Username { get; set; } = "";
        [JsonPropertyName("Name")] public string Name { get; set; } = "";
        [JsonPropertyName("Email")] public string Email { get; set; } = "";
        [JsonPropertyName("userType")] public int? userType { get; set; }
    }

    public sealed class WebLoginResponse
    {
        [JsonPropertyName("data")] public object? data { get; set; }
        [JsonPropertyName("AppSetting")] public AppSetting? AppSetting { get; set; }
        [JsonPropertyName("MyUser")] public MyUser? MyUser { get; set; }
        [JsonPropertyName("Token")] public string? Token { get; set; }
    }

    public sealed class SendOtpRequest
    {
        [JsonPropertyName("UserName")] public string UserName { get; set; } = "";
        [JsonPropertyName("Password")] public string Password { get; set; } = "";
        [JsonPropertyName("SeqKey")] public string SeqKey { get; set; } = "";
    }
    public sealed class SendOtpResponse
    {
        [JsonPropertyName("success")] public bool success { get; set; }
        [JsonPropertyName("message")] public string message { get; set; } = "";
    }

    public sealed class LoginWithOtpRequest
    {
        [JsonPropertyName("UserName")] public string UserName { get; set; } = "";
        [JsonPropertyName("Password")] public string Password { get; set; } = "";
        [JsonPropertyName("SeqKey")] public string SeqKey { get; set; } = "";
    }
    public sealed class LoginWithOtpResponse
    {
        [JsonPropertyName("AppSetting")] public AppSetting? AppSetting { get; set; }
        [JsonPropertyName("MyUser")] public MyUser? MyUser { get; set; }
    }

    public sealed class TokenSetupResponse
    {
        [JsonPropertyName("data")] public int data { get; set; }
    }

    // ===== PROGRAM TYPE =====
    public sealed class ProgramTypeDto
    {
        // «ID» سرور روی «Id» مپ می‌شود
        [JsonPropertyName("ID")] public int Id { get; set; }

        [JsonPropertyName("Name")] public string? Name { get; set; }

        // سرور «Describtion» می‌فرستد، کلاینت «Description»
        [JsonPropertyName("Describtion")] public string? Description { get; set; }

        [JsonPropertyName("IsVisible")] public bool IsVisible { get; set; }
        [JsonPropertyName("LastModified")] public string? LastModified { get; set; }
        [JsonPropertyName("ModifiedById")] public string? ModifiedById { get; set; }
    }
}
