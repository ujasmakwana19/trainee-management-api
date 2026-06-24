namespace TraineeManagement.Api.FileServices;
public interface IFileStorageService
{
    Task<SavedFileResult> SaveAsync(Stream content, string boundary, CancellationToken cancellationToken);
    Task<Stream> OpenReadAsync(string storageName, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string storageName, CancellationToken cancellationToken);
    Task DeleteAsync(string storageName, CancellationToken cancellationToken);
}