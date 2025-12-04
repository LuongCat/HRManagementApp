namespace HRManagementApp.models;

public class PhuCapNhanVien
{
    public int ID { get; set; }
    public int MaNV { get; set; }
    public string TenPhuCap { get; set; }
    public decimal SoTien { get; set; }
    public DateTime ApDungTuNgay { get; set; }
    public DateTime? ApDungDenNgay { get; set; }

    // Quan hệ
    public NhanVien NhanVien { get; set; }
}
