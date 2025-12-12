namespace HRManagementApp.models;
using System.ComponentModel;
public class NhanVien 
{
    public int MaNV { get; set; }
    public string HoTen { get; set; }
    public DateTime? NgaySinh { get; set; }
    public string? SoCCCD { get; set; }
    public string? DienThoai { get; set; }
    
    public string GioiTinh { get; set; }
    public DateTime? NgayVaoLam { get; set; }
    
    public int? MaPB { get; set; }
    public List< PhongBan> PhongBan { get; set; } = new();
    public List<ChucVu> ChucVu { get; set; } = new();

    public List<CaLam> CaLams { get; set; } = new();
    public string CaLamDisplay
    {
        get
        {
            if (CaLams == null || CaLams.Count == 0)
                return "Chưa phân công";
            
            // Kết quả sẽ là: "Ca Sáng, Ca Chiều, Ca Đêm"
            return string.Join(", ", CaLams.Select(c => c.TenCa));
        }
    }
    
    public List<VaiTroNhanVien> DanhSachChucVu { get; set; } = new();

    public List<PhuCapNhanVien> PhuCaps  { get; set; } = new();

    public List<KhauTru> KhauTrus  { get; set; } = new() ;
    
    public List<Thue> Thues  { get; set; } = new();

    public List<Luong> Luongs { get; set; } = new();
    
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
    // hai cái này đang dùng cho thêm nhân viên vào 1 phòng ban
    public bool IsSelected { get; set; } = false;
    public int MaChucVuMoi {get; set;}
    
    public decimal HeSoKiemNhiem { get; set; } = 1;


}