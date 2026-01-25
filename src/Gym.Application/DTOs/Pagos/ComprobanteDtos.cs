namespace Gym.Application.DTOs.Pagos;

public class ComprobanteDto
{
    public int Id { get; set; }
    public string? FileUrl { get; set; }
    public string? MimeType { get; set; }
    public DateTime SubidoEn { get; set; }
}

public class ComprobanteUploadResult
{
    public int Id { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
}
