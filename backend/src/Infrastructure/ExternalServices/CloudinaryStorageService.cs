using ResumeBuilder.Application.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace ResumeBuilder.Infrastructure.ExternalServices;

/// <summary>
/// Cloudinary-based file storage service for free deployment.
/// Supports uploading and deleting files from Cloudinary CDN.
/// Free tier: 25 GB storage + 25 GB bandwidth per month.
/// </summary>
public class CloudinaryStorageService : IStorageService
{
    private readonly string _cloudName;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly HttpClient _httpClient;

    public CloudinaryStorageService(IConfiguration configuration, HttpClient httpClient)
    {
        _cloudName = configuration["Cloudinary:CloudName"] 
            ?? throw new InvalidOperationException("Cloudinary:CloudName not configured");
        _apiKey = configuration["Cloudinary:ApiKey"] 
            ?? throw new InvalidOperationException("Cloudinary:ApiKey not configured");
        _apiSecret = configuration["Cloudinary:ApiSecret"] 
            ?? throw new InvalidOperationException("Cloudinary:ApiSecret not configured");
        _httpClient = httpClient;
    }

    /// <summary>
    /// Uploads a file to Cloudinary and returns the secure URL.
    /// </summary>
    public async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
        using var content = new MultipartFormDataContent();
        
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var signature = ComputeSignature(timestamp);

        content.Add(new StreamContent(fileStream), "file", fileName);
        content.Add(new StringContent(_cloudName), "cloud_name");
        content.Add(new StringContent(_apiKey), "api_key");
        content.Add(new StringContent(timestamp), "timestamp");
        content.Add(new StringContent(signature), "signature");
        content.Add(new StringContent("auto"), "eager_async");

        var uploadUrl = $"https://api.cloudinary.com/v1_1/{_cloudName}/raw/upload";
        
        var response = await _httpClient.PostAsync(uploadUrl, content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        // Extract secure_url from JSON response
        var urlStart = jsonResponse.IndexOf("\"secure_url\":\"") + 14;
        var urlEnd = jsonResponse.IndexOf("\"", urlStart);
        
        return jsonResponse[urlStart..urlEnd];
    }

    /// <summary>
    /// Deletes a file from Cloudinary by public ID.
    /// </summary>
    public async Task DeleteAsync(string fileUrl)
    {
        var publicId = Path.GetFileNameWithoutExtension(fileUrl.Split('/').Last());
        
        using var content = new MultipartFormDataContent();
        
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var signature = ComputeSignature(publicId + timestamp);

        content.Add(new StringContent(publicId), "public_id");
        content.Add(new StringContent(_cloudName), "cloud_name");
        content.Add(new StringContent(_apiKey), "api_key");
        content.Add(new StringContent(timestamp), "timestamp");
        content.Add(new StringContent(signature), "signature");

        var deleteUrl = $"https://api.cloudinary.com/v1_1/{_cloudName}/raw/destroy";
        
        var response = await _httpClient.PostAsync(deleteUrl, content);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Computes SHA-256 signature for Cloudinary API authentication.
    /// </summary>
    private string ComputeSignature(string data)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data + _apiSecret));
        return Convert.ToHexString(hash).ToLower();
    }
}
