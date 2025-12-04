namespace HRManagementApp.Models
{
    public class TaiKhoan
    {
        public int MaTK { get; set; }
        public int? MaNV { get; set; }
        public string TenDangNhap { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public string TrangThai { get; set; } = "Hoạt động";
    }
}
