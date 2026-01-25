using Api.Data.Models; 
using System;

public class Comprobante
{
    public int Id { get; set; }

    public string? FileUrl { get; set; }
    public string? MimeType { get; set; }
    public DateTime SubidoEn { get; set; }

    public OrdenPago? OrdenPago { get; set; }
}
