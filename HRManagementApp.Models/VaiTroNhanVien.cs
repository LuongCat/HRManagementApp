namespace HRManagementApp.models;

public class VaiTroNhanVien
{
    public int MaNV { get; set; }
    public int MaCV { get; set; }
    public string LoaiChucVu { get; set; }   // Chính thức / Kiêm nhiệm
    public decimal? HeSoPhuCapKiemNhiem { get; set; }
    
    public decimal? HeSoLuongCoBan {get; set;}
    public string? GhiChu { get; set; }
    
    public ChucVu ChucVu { get; set; } = new();
    
}
