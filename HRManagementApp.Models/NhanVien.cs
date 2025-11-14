namespace HRManagementApp.models;

public class NhanVien
{
    public int MaNV { get; set; }
    public string HoTen { get; set; }
    public DateTime? NgaySinh { get; set; }
    public string? SoCCCD { get; set; }
    public string? DienThoai { get; set; }
    
    public bool GioiTinh { get; set; }
    public DateTime? NgayVaoLam { get; set; }
    
    public PhongBan PhongBan { get; set; }
    public ChucVu ChucVu { get; set; }
    
    public String TrangThai { get; set; }
}