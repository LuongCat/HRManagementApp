namespace HRManagementApp.models;

public class KhauTru
{
    //bảng khấu trừ
    public int MaKT { get; set; }
    public int MaNV { get; set; }
    public string TenKhoanTru { get; set; }
    public decimal SoTien { get; set; }
    public DateTime Ngay { get; set; }
    public string GhiChu { get; set; }

    // Quan hệ
    public NhanVien NhanVien { get; set; } = new();
}
