using System;

namespace HRManagementApp.models
{
    public class MyPayslipDTO
    {
        // Thông tin chung
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string HoTen { get; set; }
        public string TenPB { get; set; }
        public string TenChucVu { get; set; }
        public DateTime TuNgay { get; set; } // Ngày đầu tháng
        public DateTime DenNgay { get; set; } // Ngày cuối tháng

        // Phần Thu Nhập
        public decimal LuongCoBan { get; set; }
        public double TongGioLam { get; set; } // Quy đổi từ ngày công
        public decimal LuongTheoNgayCong { get; set; } // Lương thực tế theo ngày đi làm
        public decimal TongPhuCap { get; set; }
        public decimal TienThuong { get; set; } // Nếu có

        // Kiêm nhiệm
        public bool CoKiemNhiem { get; set; }
        public decimal HeSoKiemNhiem { get; set; }
        public decimal TienKiemNhiem { get; set; }

        // Phần Khấu Trừ
        public decimal TongThue { get; set; }
        public decimal TongBaoHiem { get; set; } // BHXH, BHYT...
        public decimal TienUng { get; set; }     // Tiền ứng
        public decimal TongKhauTruKhac { get; set; }

        // Tổng kết
        public decimal ThucLanh { get; set; }
        public string TrangThai { get; set; } // Đã thanh toán / Chưa thanh toán
    }
}