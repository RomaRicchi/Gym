namespace Gym.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream file, string fileName, string subFolder, CancellationToken ct = default);
    Task<bool> DeleteAsync(string filePath, CancellationToken ct = default);
}
