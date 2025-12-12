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

        public EmployeeSummaryWindow(NhanVien employee , int thang , int nam)
        {
            InitializeComponent();
            _employee = employee;

            LoadData(thang, nam);
        }

        private void LoadData(int thang, int nam)
        {
            // 1. Header
            TxtHoTen.Text = _employee.HoTen.ToUpper();
            TxtMaNV.Text = $"Mã NV: {_employee.MaNV}";
            TxtThangHienTai.Text = $"{thang:00} / {nam}";

            var targetDate = new DateTime(nam, thang, 1);

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

            // 3. Phụ cấp (lọc theo tháng/năm)
            if (_employee.PhuCaps == null) _employee.PhuCaps = new List<PhuCapNhanVien>();

            var validAllowances = _employee.PhuCaps
                .Where(p =>
                    // Áp dụng trong tháng/năm này
                    (p.ApDungTuNgay == null || p.ApDungTuNgay <= targetDate.AddMonths(1).AddDays(-1)) &&
                    (p.ApDungDenNgay == null || p.ApDungDenNgay >= targetDate)
                )
                .ToList();

            DgAllowances.ItemsSource = validAllowances;

            decimal tongPhuCap = validAllowances.Sum(p => p.SoTien);
            TxtTongPhuCap.Text = $"{tongPhuCap:N0} VNĐ";

            // 4. Thuế (lọc theo tháng/năm)
            if (_employee.Thues == null) _employee.Thues = new List<Thue>();

            var validTaxes = _employee.Thues
                .Where(t =>
                    (t.ApDungTuNgay == null || t.ApDungTuNgay <= targetDate.AddMonths(1).AddDays(-1)) &&
                    (t.ApDungDenNgay == null || t.ApDungDenNgay >= targetDate)
                )
                .ToList();

            DgTaxes.ItemsSource = validTaxes;

            decimal tongThue = validTaxes.Sum(t => t.SoTien);
            TxtTongThue.Text = $"{tongThue:N0} VNĐ";

            // 5. Khấu trừ / Phạt (chỉ tính trong tháng/năm được truyền vào)
            if (_employee.KhauTrus == null) _employee.KhauTrus = new List<KhauTru>();

            var monthDeductions = _employee.KhauTrus
                .Where(k => k.Ngay.Month == thang && k.Ngay.Year == nam)
                .ToList();

            DgDeductions.ItemsSource = monthDeductions;

            decimal tongPhat = monthDeductions.Sum(k => k.SoTien);
            TxtTongPhat.Text = $"{tongPhat:N0} VNĐ";

            // 6. Tổng kết
            decimal totalIncome = tongLuongHeSo + tongPhuCap;
            decimal totalDeduction = tongThue + tongPhat;
            decimal thucNhan = totalIncome - totalDeduction;

            TxtThucNhan.Text = $"{thucNhan:N0} VNĐ";
        }
    }
}