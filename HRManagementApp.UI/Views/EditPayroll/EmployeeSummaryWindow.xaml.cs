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
        
        // Giả sử mức lương cơ sở chung
        private const decimal BASE_SALARY_UNIT = 1800000; 

        public EmployeeSummaryWindow(NhanVien employee)
        {
            InitializeComponent();
            _employee = employee;

            LoadData();
        }

        private void LoadData()
        {
            // 1. Header
            TxtHoTen.Text = _employee.HoTen.ToUpper();
            TxtMaNV.Text = $"Mã NV: {_employee.MaNV}";
            TxtThangHienTai.Text = DateTime.Now.ToString("MM / yyyy");

            // 2. Chức vụ & Hệ số
            if (_employee.DanhSachChucVu == null) _employee.DanhSachChucVu = new List<VaiTroNhanVien>();
            DgRoles.ItemsSource = _employee.DanhSachChucVu;

            // Tính tổng lương theo hệ số (Lương cơ bản + Phụ cấp chức vụ)
            decimal tongLuongHeSo = 0;
            foreach (var role in _employee.DanhSachChucVu)
            {
                decimal luongCV = (role.HeSoLuongCoBan ?? 0) * BASE_SALARY_UNIT;
                decimal phuCapCV = (role.HeSoPhuCapKiemNhiem ?? 0) * BASE_SALARY_UNIT;
                tongLuongHeSo += luongCV + phuCapCV;
            }
            TxtTongLuongHeSo.Text = $"{tongLuongHeSo:N0} VNĐ";

            // 3. Phụ cấp (Khác)
            if (_employee.PhuCaps == null) _employee.PhuCaps = new List<PhuCapNhanVien>();
            // Lọc phụ cấp còn hiệu lực
            var validAllowances = _employee.PhuCaps
                .Where(p => p.ApDungDenNgay == null || p.ApDungDenNgay >= DateTime.Now)
                .ToList();
            DgAllowances.ItemsSource = validAllowances;
            
            decimal tongPhuCap = validAllowances.Sum(p => p.SoTien);
            TxtTongPhuCap.Text = $"{tongPhuCap:N0} VNĐ";

            // 4. Thuế
            if (_employee.Thues == null) _employee.Thues = new List<Thue>();
            var validTaxes = _employee.Thues
                .Where(t => t.ApDungDenNgay == null || t.ApDungDenNgay >= DateTime.Now)
                .ToList();
            DgTaxes.ItemsSource = validTaxes;

            decimal tongThue = validTaxes.Sum(t => t.SoTien);
            TxtTongThue.Text = $"{tongThue:N0} VNĐ";

            // 5. Khấu trừ / Phạt (Chỉ tính trong tháng hiện tại)
            if (_employee.KhauTrus == null) _employee.KhauTrus = new List<KhauTru>();
            var currentMonth = DateTime.Now;
            var monthDeductions = _employee.KhauTrus
                .Where(k => k.Ngay.Month == currentMonth.Month && k.Ngay.Year == currentMonth.Year)
                .ToList();
            DgDeductions.ItemsSource = monthDeductions;

            decimal tongPhat = monthDeductions.Sum(k => k.SoTien);
            TxtTongPhat.Text = $"{tongPhat:N0} VNĐ";

            // 6. TỔNG KẾT
            decimal totalIncome = tongLuongHeSo + tongPhuCap;
            decimal totalDeduction = tongThue + tongPhat;
            decimal thucNhan = totalIncome - totalDeduction;

            // Hiển thị
            TxtThucNhan.Text = $"{thucNhan:N0} VNĐ";
        }
    }
}