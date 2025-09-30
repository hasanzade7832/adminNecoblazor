namespace AdminNeco.Services;

// --- Request/Response ها مشابه TypeScript شما ---
public sealed class WebLoginRequest
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string SeqKey { get; set; } = "";
}

public sealed class AppSetting
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public string? LetterBtns { get; set; }
}

public sealed class MyUser
{
    public string ID { get; set; } = "";
    public string TTKK { get; set; } = "";
    public string Username { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public int? userType { get; set; }
}

public sealed class WebLoginResponse
{
    public object? data { get; set; }
    public AppSetting? AppSetting { get; set; }
    public MyUser? MyUser { get; set; }
    public string? Token { get; set; }
}

public sealed class SendOtpRequest { public string UserName { get; set; } = ""; public string Password { get; set; } = ""; public string SeqKey { get; set; } = ""; }
public sealed class SendOtpResponse { public bool success { get; set; } public string message { get; set; } = ""; }

public sealed class LoginWithOtpRequest { public string UserName { get; set; } = ""; public string Password { get; set; } = ""; public string SeqKey { get; set; } = ""; }
public sealed class LoginWithOtpResponse
{
    public AppSetting? AppSetting { get; set; }
    public MyUser? MyUser { get; set; }
}

public sealed class TokenSetupResponse { public int data { get; set; } }
