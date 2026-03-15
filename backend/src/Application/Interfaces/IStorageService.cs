namespace ResumeBuilder.Application.Interfaces;

/// <summary>
/// Service for file storage abstraction.
/// Allows swapping between Azure Blob Storage, local file system, or other providers.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Uploads a file to storage and returns a reference/URL.
    /// </summary>
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from storage.
    /// </summary>
    Task<Stream> DownloadAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    Task DeleteAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a blob exists.
    /// </summary>
    Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default);
}
