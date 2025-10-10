using Microsoft.Extensions.Options;

namespace Api.Services;

public class StorageOptions
{
    public string ComprobantesPath { get; set; } = "wwwroot/uploads/comprobantes";
}

public class LocalFileStorage : IFileStorage
{
    private readonly StorageOptions _opt;
    private readonly IWebHostEnvironment _env;

    public LocalFileStorage(IOptions<StorageOptions> opt, IWebHostEnvironment env)
    {
        _opt = opt.Value;
        _env = env;
    }

    public async Task<string> SaveAsync(Stream file, string fileName, string subFolder, CancellationToken ct)
    {
        var basePath = _opt.ComprobantesPath;
        var root = Path.IsPathRooted(basePath) ? basePath : Path.Combine(_env.ContentRootPath, basePath);
        var dir = Path.Combine(root, subFolder);
        Directory.CreateDirectory(dir);

        var safeName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Path.GetFileName(fileName)}";
        var full = Path.Combine(dir, safeName);

        using var fs = new FileStream(full, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await file.CopyToAsync(fs, ct);

        var relative = full.Replace(_env.ContentRootPath, "").Replace("\\", "/");
        return relative.StartsWith("/") ? relative : "/" + relative;
    }
}
