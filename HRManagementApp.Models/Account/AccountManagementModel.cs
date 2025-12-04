using System;

namespace HRManagementApp.models
{
    public class AccountManagementModel
    {
        // Các thuộc tính Binding trực tiếp với XAML
        public int MaTK { get; set; } // Dùng để định danh khi Sửa/Xóa
        
        public string TenDangNhap { get; set; } // Binding: TenDangNhap
        
        public string Name { get; set; } // Binding: Name (Lấy từ nhanvien.HoTen)
        
        public string SDT { get; set; } // Binding: SDT (Lấy từ nhanvien.DienThoai)
        
        public string VaiTro { get; set; } // Binding: VaiTro (Lấy từ vaitro.TenVaiTro)
        
        public string PhongBan { get; set; } // Binding: PhongBan (Lấy từ phongban.TenPB)
        
        public string TrangThai { get; set; } // Binding: TrangThai
        
        // Các thuộc tính ẩn cần thiết cho logic xử lý
        public int? MaNV { get; set; } // Để biết tài khoản thuộc nhân viên nào
        public string MatKhau { get; set; } // Dùng khi tạo mới/đổi pass
    }
}