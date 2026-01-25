using Gym.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Gym.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _basePath;

    public LocalFileStorageService(IWebHostEnvironment env, IConfiguration config)
    {
        _env = env;
        _basePath = config["Storage:ComprobantesPath"] ?? "wwwroot/uploads";
    }

    public async Task<string> SaveAsync(Stream file, string fileName, string subFolder, CancellationToken ct = default)
    {
        var root = Path.IsPathRooted(_basePath)
            ? _basePath
            : Path.Combine(_env.ContentRootPath, _basePath);

        var dir = Path.Combine(root, subFolder);
        Directory.CreateDirectory(dir);

        var safeName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Path.GetFileName(fileName)}";
        var fullPath = Path.Combine(dir, safeName);

        await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await file.CopyToAsync(fs, ct);

        var relative = fullPath.Replace(_env.WebRootPath ?? _env.ContentRootPath, "").Replace("\\", "/");
        if (!relative.StartsWith("/"))
            relative = "/" + relative;

        return relative;
    }

    public Task<bool> DeleteAsync(string filePath, CancellationToken ct = default)
    {
        try
        {
            var fullPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
