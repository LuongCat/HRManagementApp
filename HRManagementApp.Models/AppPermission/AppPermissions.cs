using System.Collections.Generic;

namespace HRManagementApp.Constants
{
    public static class AppPermissions
    {
        // Định nghĩa các Key quyền hạn (Key này sẽ lưu vào DB cột TenQuyen)
        public const string PERM_DUYET_DON = "DuyetDon";
        public const string PERM_XEM_BAO_CAO = "XemBaoCao";
        public const string PERM_QL_NHAN_VIEN = "QuanLyNhanVien";
        public const string PERM_QT_HE_THONG = "QuanTriHeThong";
        public const string PERM_QL_CHAM_CONG = "QuanLyChamCong";
        public const string PERM_QL_LUONG = "QuanLyLuong";
        public const string PERM_XUAT_LUONG = "XuatBangLuong"; 

        // Danh sách ánh xạ giữa Key và Mô tả (để hiển thị lên UI tiếng Việt)
        public static Dictionary<string, string> ListAll = new Dictionary<string, string>
        {
            { PERM_DUYET_DON, "Quyền duyệt đơn từ nghỉ phép" },
            { PERM_XEM_BAO_CAO, "Quyền xem các báo cáo thống kê" },
            { PERM_QL_NHAN_VIEN, "Thêm, sửa, xóa hồ sơ nhân viên" },
            { PERM_QT_HE_THONG, "Cấu hình hệ thống, tạo tài khoản" },
            { PERM_QL_CHAM_CONG, "Xem và chỉnh sửa dữ liệu chấm công" },
            {PERM_QL_LUONG,"Xem và quản lý lương"},
            { PERM_XUAT_LUONG, "Quyền xuất file Excel bảng lương" } // Mới
        };
    }
}