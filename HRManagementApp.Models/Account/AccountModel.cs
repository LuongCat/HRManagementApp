namespace HRManagementApp.models
{
    public class AccountModel
    {
        public int MaTK { get; set; }
        public int? MaNV { get; set; } // Nullable int
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Hoạt động";

        // Thuộc tính mở rộng để hiển thị lên GridView
        public string? TenNhanVien { get; set; } 
    }
}