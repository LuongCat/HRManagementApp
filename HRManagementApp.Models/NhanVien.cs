namespace HRManagementApp.models;

public class NhanVien
{
    public int MaNV { get; set; }
    public string HoTen { get; set; }
    public DateTime? NgaySinh { get; set; }
    public string? SoCCCD { get; set; }
    public string? DienThoai { get; set; }
    
    public string GioiTinh { get; set; }
    public DateTime? NgayVaoLam { get; set; }

    public List<PhongBan> PhongBan { get; set; } = new();
    public List<ChucVu> ChucVu { get; set; } = new();
    
    public String TrangThai { get; set; }
    
    public string PhongBanDisplay
    {
        get
        {
            if (PhongBan == null || PhongBan.Count == 0)
                return "";

            return string.Join(", ", PhongBan.Select(p => p.TenPB));
        }
    }

    public string ChucVuDisplay
    {
        get
        {
            if (ChucVu == null || ChucVu.Count == 0)
                return "";

            return string.Join(", ", ChucVu.Select(c => c.TenCV));
        }
    }

}