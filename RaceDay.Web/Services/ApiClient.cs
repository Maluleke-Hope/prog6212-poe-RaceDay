using System.Text.Json;

namespace RaceDay.Web.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private void AddAuthCookie()
    {
        var authCookie = _httpContextAccessor.HttpContext?.Session.GetString("AuthCookie");
        if (!string.IsNullOrEmpty(authCookie))
        {
            _httpClient.DefaultRequestHeaders.Remove("Cookie");
            _httpClient.DefaultRequestHeaders.Add("Cookie", authCookie);
        }
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        AddAuthCookie();
        return await _httpClient.GetAsync(url);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
    {
        AddAuthCookie();
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        return await _httpClient.PostAsync(url, content);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
    {
        AddAuthCookie();
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        return await _httpClient.PutAsync(url, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        AddAuthCookie();
        return await _httpClient.DeleteAsync(url);
    }

    public async Task<HttpResponseMessage> PostFileAsync(string url, IFormFile file)
    {
        AddAuthCookie();
        var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(file.OpenReadStream());
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "file", file.FileName);
        return await _httpClient.PostAsync(url, content);
    }
}