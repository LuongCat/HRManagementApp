namespace HRManagementApp.UI.Views;

public class DepartmentPayrollItem
{
    
        // Thông tin hiển thị
        public int MaNV { get; set; }
        public string TenNV { get; set; }
        public string TenPhongBan { get; set; } // Dùng để Group
        
        // Các chỉ số lương
        public decimal LuongCoBan { get; set; }
        public decimal LuongThucNhan { get; set; }
        public int TongNgayCong { get; set; }
        public decimal TongPhuCap { get; set; }
        public decimal TongKhauTru { get; set; }
        public decimal TongThue { get; set; }
        public string TrangThai { get; set; }
    
}