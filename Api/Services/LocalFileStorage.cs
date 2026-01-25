using Microsoft.Extensions.Options;

namespace Api.Services
{
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
            // 📁 Base física de almacenamiento (wwwroot/uploads/...)
            var basePath = _opt.ComprobantesPath;
            var root = Path.IsPathRooted(basePath)
                ? basePath
                : Path.Combine(_env.ContentRootPath, basePath);

            // 📂 Subcarpeta dinámica (por ejemplo: avatars, comprobantes, etc.)
            var dir = Path.Combine(root, subFolder);
            Directory.CreateDirectory(dir);

            // 🧩 Nombre de archivo seguro y único
            var safeName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Path.GetFileName(fileName)}";
            var full = Path.Combine(dir, safeName);

            // 💾 Guardar archivo de forma asíncrona
            using var fs = new FileStream(full, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await file.CopyToAsync(fs, ct);

            // 🌐 Generar ruta relativa pública (quita wwwroot)
            var relative = full.Replace(_env.WebRootPath, "").Replace("\\", "/");

            if (!relative.StartsWith("/"))
                relative = "/" + relative;

            return relative; // ejemplo: /uploads/avatars/12345_avatar.jpg
        }
    }
}
