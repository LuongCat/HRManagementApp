using HRManagementApp.BLL; // Import namespace chứa Service
using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HRManagementApp.UI.Views
{
    public partial class EmployeeSummaryWindow : Window
    {
        private NhanVien _employee;
        private PayrollResultService _payrollService; // Khai báo Service

        public EmployeeSummaryWindow(NhanVien employee, int thang, int nam)
        {
            InitializeComponent();
            _employee = employee;
            _payrollService = new PayrollResultService(); // Khởi tạo Service

            LoadData(thang, nam);
        }

        private void LoadData(int thang, int nam)
        {
            // 1. Header Info
            TxtHoTen.Text = _employee.HoTen?.ToUpper();
            TxtMaNV.Text = $"Mã NV: {_employee.MaNV}";
            TxtThangHienTai.Text = $"{thang:00} / {nam}";

            // =============================================================
            // GỌI SERVICE ĐỂ TÍNH TOÁN SỐ LIỆU CHÍNH XÁC
            // =============================================================
            // Lưu ý: Đảm bảo _employee đã được Include đầy đủ (PhuCaps, Thues,...) từ lớp gọi window này
            PayrollResult result = _payrollService.GetPayrollResultForEmployee(_employee, thang, nam);

            // =============================================================
            // HIỂN THỊ CHI TIẾT (Binding DataGrid)
            // =============================================================
            var targetDate = new DateTime(nam, thang, 1);

            // A. Chức vụ (Hiển thị danh sách)
            if (_employee.DanhSachChucVu == null) _employee.DanhSachChucVu = new List<VaiTroNhanVien>();
            DgRoles.ItemsSource = _employee.DanhSachChucVu;

            // Hiển thị Lương chính (Đã tính theo ngày công và trừ phạt đi trễ từ Service)
            // Service trả về result.luongchinh = (LươngCB * HS + PC Kiêm nhiệm) / 26 * Ngày công ...
            TxtTongLuongHeSo.Text = $"{result.luongchinh:N0} VNĐ"; 
            
            // Có thể hiển thị thêm ngày công để rõ ràng hơn (nếu UI có chỗ để)
             TxtNgayCong.Text = $"Công: {result.TongNgayCong}";


            // B. Phụ cấp (Hiển thị danh sách chi tiết để user biết gồm những khoản nào)
            if (_employee.PhuCaps == null) _employee.PhuCaps = new List<PhuCapNhanVien>();
            var validAllowances = _employee.PhuCaps
                .Where(p =>
                    (p.ApDungTuNgay <= targetDate.AddMonths(1).AddDays(-1)) &&
                    (p.ApDungDenNgay == null || p.ApDungDenNgay >= targetDate)
                ).ToList();
            DgAllowances.ItemsSource = validAllowances;

            // Tổng tiền lấy từ Service
            TxtTongPhuCap.Text = $"{result.TongPhuCap:N0} VNĐ";


            // C. Thuế (Hiển thị danh sách)
            if (_employee.Thues == null) _employee.Thues = new List<Thue>();
            var validTaxes = _employee.Thues
                .Where(t =>
                    (t.ApDungTuNgay <= targetDate.AddMonths(1).AddDays(-1)) &&
                    (t.ApDungDenNgay == null || t.ApDungDenNgay >= targetDate)
                ).ToList();
            DgTaxes.ItemsSource = validTaxes;

            // Tổng tiền lấy từ Service
            TxtTongThue.Text = $"{result.TongThue:N0} VNĐ";


            // D. Khấu trừ (Hiển thị danh sách)
            if (_employee.KhauTrus == null) _employee.KhauTrus = new List<KhauTru>();
            var monthDeductions = _employee.KhauTrus
                .Where(k => k.Ngay.Month == thang && k.Ngay.Year == nam)
                .ToList();
            DgDeductions.ItemsSource = monthDeductions;

            // Tổng tiền lấy từ Service
            TxtTongPhat.Text = $"{result.TongKhauTru:N0} VNĐ";


            // =============================================================
            // TỔNG KẾT
            // =============================================================
            
            // Lương thực nhận lấy từ Service (Đã bao gồm công thức cộng trừ phức tạp)
            TxtThucNhan.Text = $"{result.LuongThucNhan:N0} VNĐ";
            
            // Cập nhật trạng thái hiển thị (Đã trả / Chưa trả)
            if (!string.IsNullOrEmpty(result.TrangThai))
            {
                this.Title = $"Chi tiết lương - {result.TrangThai}"; // Hoặc gán vào Label nào đó
            }
        }
    }
}