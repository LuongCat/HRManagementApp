using System;

namespace HRManagementApp.models
{
    public class DonTu
    {
        public int MaDon { get; set; }
        public int MaNV { get; set; }
        public int MaLoaiDon { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string LyDo { get; set; }
        public string TrangThai { get; set; } // 'Chưa duyệt', 'Đã duyệt', 'Từ chối'
        public DateTime NgayGui { get; set; }
        public string NguoiDuyet { get; set; }

        // Join Data (Dữ liệu hiển thị)
        public string HoTenNhanVien { get; set; }
        public string TenLoaiDon { get; set; }

        // Helpers cho giao diện (Display Properties)
        public string MaDonDisplay => $"DT{MaDon:000}";

        public string SoNgayDisplay
        {
            get
            {
                TimeSpan span = NgayKetThuc - NgayBatDau;
                return $"{(int)span.TotalDays + 1} ngày";
            }
        }
    }
}