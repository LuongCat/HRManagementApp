namespace HRManagementApp.models;

public class VaiTroNhanVien
{
    public int MaNV { get; set; }
    public int MaCV { get; set; }
    public int MaPB { get; set; }

    public string LoaiChucVu { get; set; }   // Chính thức / Kiêm nhiệm
    public decimal? HeSoPhuCapKiemNhiem { get; set; }
    public string? GhiChu { get; set; }

    public ChucVu ChucVu { get; set; } = new();
    public PhongBan PhongBan { get; set; } = new();
}
