using System.Collections.Generic;

namespace HRManagementApp.Constants
{
    public static class AppPermissions
    {

        public const string NHANVIEN_XEM = "NhanVien.Xem";
        public const string NHANVIEN_THEM = "NhanVien.Them";
        public const string NHANVIEN_SUA = "NhanVien.Sua";
        public const string NHANVIEN_XOA = "NhanVien.Xoa"; // Bao gồm cả khóa tài khoản

        public const string PHONGBAN_XEM = "PhongBan.Xem";
        public const string PHONGBAN_SUA = "PhongBan.Sua";
        public const string PHONGBAN_EXCEL = "PhongBan.XuatExcel";
        

        public const string LUONG_XEM = "Luong.Xem";
        public const string LUONG_TINHTOAN = "Luong.TinhToan"; // Quyền chạy tính lương
        public const string LUONG_CHOT = "Luong.Chot";         // Quyền chốt lương (không cho sửa nữa)
        public const string LUONG_EXCEL = "Luong.XuatExcel";   // Xuất phiếu lương/bảng lương
        public const string LUONG_THANHTOAN = "Luong.XuatThanhToan";
        public const string LUONG_SUAVAITRO = "Luong.XuatSuaVaiTro";
        public const string LUONG_SUAPHUCAP = "Luong.SuaPhuCap";
        public const string LUONG_SUAKHAUTRU = "Luong.SuaKhauTru";
        public const string LUONG_SUATHUE = "Luong.SuaThue";
        
        
        
        public const string CHAMCONG_XEM = "ChamCong.Xem";
        public const string CHAMCONG_SUA = "ChamCong.Sua";     // Sửa giờ vào/ra thủ công
        public const string CHAMCONG_DUYET = "ChamCong.Duyet"; // Duyệt đơn nghỉ phép (Thay cho PERM_DUYET_DON cũ)
        public const string CHAMCONG_EXCEL = "ChamCong.XuatExcel";
        
        public const string HETHONG_XEM = "HeThong.Xem";
        public const string HETHONG_PHANQUYEN = "HeThong.PhanQuyen"; // Quản lý tài khoản, vai trò
        public const string HETHONG_CAUHINH = "HeThong.CauHinh";     // Cấu hình tham số chung
        public const string HETHONG_TAOTAIKHOAN = "HeThong.TaoTaiKhoan"; 
        public const string HETHONG_SUATAIKHOAN = "HeThong.SuaTaiKhoan"; 
        public const string HETHONG_TAOVAITRO = "HeThong.TaoVaiTro"; 
        public const string HETHONG_SUAVAITRO = "HeThong.SuaVaiTro"; 
        public const string HETHONG_XOAVAITRO = "HeThong.XaoVaiTro"; 

        
        public const string DONTU_XEM = "DonTu.Xem";
        public const string DONTU_TAO = "DonTu.Tao";
        public const string DONTU_THEMLOAIDON = "DonTu.ThemLoaiDon";
        public const string DONTU_DUYETDON = "DonTu.DuyetDon";
        public const string DONTU_SUALOAIDON = "DonTu.SuaLoaiDon";
        public const string DONTU_XOALOAIDON = "DonTu.XoaLoaiDon";
        
        public const string BAOCAO_XEM = "BaoCao.Xem";
        public const string BAOCAO_EXCEL = "BaoCao.XuatExcel";        
        public static Dictionary<string, string> ListAll = new Dictionary<string, string>
        {
            // --- Nhân sự ---
            { NHANVIEN_XEM, "Xem danh sách nhân viên" },
            { NHANVIEN_THEM, "Thêm mới nhân viên" },
            { NHANVIEN_SUA, "Cập nhật thông tin nhân viên" },
            { NHANVIEN_XOA, "Xóa hoặc khóa nhân viên" },

            // --- Phòng ban ---
            { PHONGBAN_XEM, "Xem danh sách phòng ban" },
            { PHONGBAN_SUA, "Quản lý thông tin phòng ban" },
            { PHONGBAN_EXCEL, "Xuất báo cáo phòng ban ra Excel"},

            // --- Lương ---
            { LUONG_XEM, "Xem bảng lương" },
            { LUONG_TINHTOAN, "Thực hiện tính toán lương" },
            { LUONG_CHOT, "Khóa/Chốt sổ lương tháng" },
            { LUONG_EXCEL, "Xuất báo cáo lương ra Excel" },
            { LUONG_THANHTOAN, "Xuất danh sách thanh toán lương" },
            { LUONG_SUAVAITRO, "Điều chỉnh hệ số lương/chức vụ" },
            { LUONG_SUAPHUCAP, "Điều chỉnh phụ cấp nhân viên" },
            { LUONG_SUAKHAUTRU, "Điều chỉnh các khoản khấu trừ" },
            { LUONG_SUATHUE, "Cấu hình thuế TNCN" },

            // --- Chấm công ---
            { CHAMCONG_XEM, "Xem dữ liệu chấm công" },
            { CHAMCONG_SUA, "Hiệu chỉnh giờ công/tăng ca" },
            { CHAMCONG_DUYET, "Duyệt đơn nghỉ phép/công tác" },
            { CHAMCONG_EXCEL, "Xuất bảng công ra Excel" },

            // --- Hệ thống ---
            { HETHONG_XEM, "Xem nhật ký hoạt động hệ thống" },
            { HETHONG_PHANQUYEN, "Truy cập quản lý phân quyền" },
            { HETHONG_CAUHINH, "Thiết lập cấu hình hệ thống" },
            
            // Chi tiết hệ thống
            { HETHONG_TAOTAIKHOAN, "Tạo tài khoản mới" },
            { HETHONG_SUATAIKHOAN, "Sửa/Khóa tài khoản" },
            { HETHONG_TAOVAITRO, "Tạo vai trò mới" },
            { HETHONG_SUAVAITRO, "Sửa quyền hạn của vai trò" },
            { HETHONG_XOAVAITRO, "Xóa vai trò khỏi hệ thống" },

            // --- Đơn từ & Báo cáo ---
            { DONTU_XEM, "Xem danh sách đơn từ nghỉ phép" },
            { DONTU_TAO, "Tạo đơn xin nghỉ/công tác mới" },
            { DONTU_DUYETDON, "Quyền duyệt hoặc từ chối đơn" },

            // --- Cấu hình Loại đơn ---
            { DONTU_THEMLOAIDON, "Thêm mới loại đơn (VD: Nghỉ ốm, Thai sản)" },
            { DONTU_SUALOAIDON, "Chỉnh sửa tên/mô tả loại đơn" },
            { DONTU_XOALOAIDON, "Xóa loại đơn" },
            
            { BAOCAO_XEM, "Truy cập xem các báo cáo thống kê" },
           { BAOCAO_EXCEL, "Xuất dữ liệu báo cáo ra file Excel" }
        };
    }
}