using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AdminNeco.Services;

public sealed class FileService
{
    private readonly IHttpClientFactory _httpFactory;

    public FileService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    // کلاینت برای API فایل‌ها
    private HttpClient FileClient => _httpFactory.CreateClient("fileApi");

    // کلاینت برای API اصلی
    private HttpClient ApiClient => _httpFactory.CreateClient("api");

    // -------- User / Token --------
    public async Task<T?> GetIdByUserTokenAsync<T>(CancellationToken ct = default)
    {
        var res = await ApiClient.PostAsync(ApiConstants.Auth.GetIdByUserToken, content: null, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    // -------- Upload (multipart/form-data) --------
    public async Task<T?> UploadFileAsync<T>(Stream fileStream, string fileName, CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "file", fileName);

        var res = await FileClient.PostAsync(ApiConstants.File.Upload, content, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    // -------- Insert (metadata) --------
    public async Task<T?> InsertAsync<T>(object model, CancellationToken ct = default)
    {
        var res = await FileClient.PostAsJsonAsync(ApiConstants.File.Insert, model, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    // -------- GetFile (by gid) --------
    public async Task<T?> GetFileAsync<T>(string gid, CancellationToken ct = default)
    {
        var res = await FileClient.PostAsJsonAsync(ApiConstants.File.GetById, new { gid }, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    // -------- Download (binary) --------
    public async Task<byte[]> DownloadAsync(object model, CancellationToken ct = default)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, ApiConstants.File.Download)
        {
            Content = JsonContent.Create(model)
        };

        var res = await FileClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadAsByteArrayAsync(ct);
    }
}
