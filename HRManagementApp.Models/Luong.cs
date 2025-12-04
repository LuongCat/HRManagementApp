namespace HRManagementApp.models;

public class Luong
{
    public int MaLuong { get; set; }
    public int MaNV { get; set; }
    public int Thang { get; set; }
    public int Nam { get; set; }
    public int? TongNgayCong { get; set; }
    
    public decimal? TienLuong { get; set; }
    public decimal? LuongThucNhan { get; set; }
    
    public string TrangThai { get; set; } // Enum: "Chưa trả", "Đã trả"
}