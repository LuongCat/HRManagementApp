using System;
using System.Collections.Generic; // Thêm namespace này

namespace HRManagementApp.models
{
    // Class tĩnh để lưu thông tin người dùng đang đăng nhập
    public static class UserSession
    {
        // --- Thông tin Tài khoản ---
        public static int MaTK { get; set; }
        public static int? MaNV { get; set; }
        public static string TenDangNhap { get; set; } = string.Empty;
        public static string VaiTro { get; set; } = "Guest";

        // --- Danh sách Quyền hạn (Mới) ---
        // Lưu các mã quyền như: "DuyetDon", "XemBaoCao", "QuanLyNhanVien"...
        public static List<string> QuyenHan { get; set; } = new List<string>();

        // --- Thông tin Nhân viên mở rộng ---
        public static string HoTen { get; set; } = "Admin"; // Mặc định
        public static string? DienThoai { get; set; }
        public static string? SoCCCD { get; set; }
        public static DateTime? NgaySinh { get; set; }
        public static string? GioiTinh { get; set; }
        
        // --- Thông tin Phòng ban ---
        public static int? MaPB { get; set; }
        public static string? TenPB { get; set; }

        public static bool IsLoggedIn => MaTK > 0;

        // Hàm kiểm tra quyền nhanh
        public static bool HasPermission(string tenQuyen)
        {
            // Nếu là Admin (RoleID = 1 hoặc tên VaiTro = Admin) thì luôn có quyền (Optional)
            if (VaiTro == "Admin") return true; 
            
            return QuyenHan.Contains(tenQuyen);
        }

        // Xóa session khi đăng xuất
        public static void Clear()
        {
            MaTK = 0;
            MaNV = null;
            TenDangNhap = string.Empty;
            VaiTro = string.Empty;
            QuyenHan.Clear(); // Xóa danh sách quyền
            
            // Reset thông tin nhân viên
            HoTen = string.Empty;
            DienThoai = null;
            SoCCCD = null;
            NgaySinh = null;
            GioiTinh = null;
            
            // Reset thông tin phòng ban
            MaPB = null;
            TenPB = null;
        }
    }
}