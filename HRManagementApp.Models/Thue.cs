namespace HRManagementApp.models;

public class Thue
{
    public int MaThue { get; set; }
    public int MaNV { get; set; }
    public string TenThue { get; set; }
    public decimal SoTien { get; set; }
    public DateTime ApDungTuNgay { get; set; }
    public DateTime? ApDungDenNgay { get; set; }

    // Quan hệ
    public NhanVien NhanVien { get; set; }
}