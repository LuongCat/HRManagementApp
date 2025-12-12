using HRManagementApp.BLL;
using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HRManagementApp.UI.Views
{
    public partial class MyPayslipView : UserControl
    {
        private readonly PayrollResultService _payrollResultService;
        private readonly NhanVienService _nhanVienService;
        private bool _isLoaded = false;

        public MyPayslipView()
        {
            InitializeComponent();
            
            _payrollResultService = new PayrollResultService();
            _nhanVienService = new NhanVienService();

            LoadAvailableMonths();
            _isLoaded = true;
            
            // Tải dữ liệu mặc định ngay khi mở
            if (cboMonth.Items.Count > 0)
            {
                LoadPayslipData();
            }
        }

        private void LoadAvailableMonths()
        {
            var months = new List<MonthOption>();
            DateTime currentDate = DateTime.Now;

            // Tạo danh sách 12 tháng gần nhất
            for (int i = 0; i < 12; i++)
            {
                DateTime d = currentDate.AddMonths(-i);
                months.Add(new MonthOption 
                { 
                    Display = $"Tháng {d.Month:00}/{d.Year}", 
                    Month = d.Month, 
                    Year = d.Year 
                });
            }

            cboMonth.ItemsSource = months;
            cboMonth.DisplayMemberPath = "Display";
            cboMonth.SelectedIndex = 0; 
        }

        private void CboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded)
            {
                LoadPayslipData();
            }
        }

        private void LoadPayslipData()
        {
            // 1. Kiểm tra User Session
            if (!UserSession.IsLoggedIn || UserSession.MaNV == null)
            {
                ShowError("Vui lòng đăng nhập để xem phiếu lương.");
                return;
            }

            int maNV = Convert.ToInt32(UserSession.MaNV);
            if (cboMonth.SelectedItem is not MonthOption selectedMonth) return;

            try
            {
                // 2. Lấy thông tin nhân viên
                // LƯU Ý: Hàm GetEmployeeById cần Include: ChucVu, PhuCaps, Thues, KhauTrus
                NhanVien nhanVien = _nhanVienService.GetEmployeeById(maNV);

                if (nhanVien == null)
                {
                    ShowError("Không tìm thấy thông tin nhân viên.");
                    return;
                }

                // 3. Gọi Service tính toán (Logic lõi)
                PayrollResult result = _payrollResultService.GetPayrollResultForEmployee(nhanVien, selectedMonth.Month, selectedMonth.Year);

                // Nếu không có công nào và lương = 0 thì coi như chưa có dữ liệu
                if ((result.LuongThucNhan == 0 || result.LuongThucNhan == null) && (result.TongNgayCong == 0 || result.TongNgayCong == null))
                {
                    ShowError("Chưa có dữ liệu chấm công hoặc lương cho tháng này.");
                    return;
                }

                // ==========================================================
                // XỬ LÝ DỮ LIỆU DANH SÁCH (Giống EmployeeSummaryWindow)
                // ==========================================================
                var targetDate = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);

                // A. Chức vụ (Xử lý null safe)
                if (nhanVien.DanhSachChucVu == null) nhanVien.DanhSachChucVu = new List<VaiTroNhanVien>();
                // DgRoles sẽ bind vào list này

                // B. Phụ cấp (Lọc theo ngày áp dụng)
                if (nhanVien.PhuCaps == null) nhanVien.PhuCaps = new List<PhuCapNhanVien>();
                var validAllowances = nhanVien.PhuCaps
                    .Where(p =>
                        (p.ApDungTuNgay <= targetDate.AddMonths(1).AddDays(-1)) &&
                        (p.ApDungDenNgay == null || p.ApDungDenNgay >= targetDate)
                    ).ToList();

                // C. Thuế (Lọc theo ngày áp dụng)
                if (nhanVien.Thues == null) nhanVien.Thues = new List<Thue>();
                var validTaxes = nhanVien.Thues
                    .Where(t =>
                        (t.ApDungTuNgay <= targetDate.AddMonths(1).AddDays(-1)) &&
                        (t.ApDungDenNgay == null || t.ApDungDenNgay >= targetDate)
                    ).ToList();

                // D. Khấu trừ (Lọc đúng tháng/năm đang xem)
                if (nhanVien.KhauTrus == null) nhanVien.KhauTrus = new List<KhauTru>();
                var monthDeductions = nhanVien.KhauTrus
                    .Where(k => k.Ngay.Month == selectedMonth.Month && k.Ngay.Year == selectedMonth.Year)
                    .ToList();

                // ==========================================================
                // TẠO VIEWMODEL ĐỂ BINDING
                // ==========================================================
                var viewModel = new PayslipViewModel
                {
                    // Thông tin chung
                    HoTen = nhanVien.HoTen?.ToUpper(),
                    MaNV = nhanVien.MaNV,
                    ThangNam = $"{selectedMonth.Month:00} / {selectedMonth.Year}",
                    NgayCong = result.TongNgayCong ?? 0,

                    // Danh sách (Cho DataGrid)
                    ChucVu = nhanVien.DanhSachChucVu,
                    DgAllowancesSource = validAllowances,  // Bind vào DgAllowances
                    DgTaxesSource = validTaxes,            // Bind vào DgTaxes
                    DgDeductionsSource = monthDeductions,  // Bind vào DgDeductions

                    // Các số tổng (Lấy từ PayrollResult đã tính toán chính xác)
                    TongLuongHeSo = result.luongchinh ?? 0,
                    TongPhuCap = result.TongPhuCap ,
                    TongThue = result.TongThue ,
                    TongPhat = result.TongKhauTru ,
                    
                    // Kết quả cuối
                    ThucLanh = result.LuongThucNhan ?? 0,
                    TrangThai = string.IsNullOrEmpty(result.TrangThai) ? "Chưa chốt" : result.TrangThai
                };

                // Đẩy dữ liệu lên giao diện
                pnlPayslipContent.DataContext = viewModel;
                
                // Binding riêng lẻ cho các DataGrid nếu DataContext inheritance không tự nhận (đôi khi cần thiết)
                // Tuy nhiên với cấu trúc XAML chuẩn thì Binding="{Binding DgAllowancesSource}" là đủ.
                // Ở XAML bạn đang để Binding="{Binding TenPhuCap}" nghĩa là DataGrid.ItemsSource phải được gán.
                // Để đơn giản cho XAML binding list, ta gán trực tiếp ItemsSource trong code behind hoặc dùng Binding trong XAML.
                // Cách tốt nhất: Gán ItemsSource trực tiếp ở đây để đảm bảo ăn ngay lập tức:
                DgRoles.ItemsSource = viewModel.ChucVu;
                DgAllowances.ItemsSource = viewModel.DgAllowancesSource;
                DgTaxes.ItemsSource = viewModel.DgTaxesSource;
                DgDeductions.ItemsSource = viewModel.DgDeductionsSource;

                pnlPayslipContent.Visibility = Visibility.Visible;
                txtNoData.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void ShowError(string msg = null)
        {
            pnlPayslipContent.Visibility = Visibility.Collapsed;
            txtNoData.Visibility = Visibility.Visible;
            txtNoData.Text = msg ?? "Không có dữ liệu.";
        }
    }

    // =====================================================
    // VIEW MODEL CHO BINDING
    // =====================================================
    public class MonthOption
    {
        public string Display { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class PayslipViewModel
    {
        // Thông tin Header
        public string HoTen { get; set; }
        public int MaNV { get; set; }
        public string ThangNam { get; set; }
        public double NgayCong { get; set; } // Bind vào TxtNgayCong

        // Nguồn dữ liệu cho các DataGrid
        public List<VaiTroNhanVien> ChucVu { get; set; }
        public List<PhuCapNhanVien> DgAllowancesSource { get; set; }
        public List<Thue> DgTaxesSource { get; set; }
        public List<KhauTru> DgDeductionsSource { get; set; }

        // Các số tổng tiền (Decimal để format N0)
        public decimal TongLuongHeSo { get; set; }
        public decimal TongPhuCap { get; set; }
        public decimal TongThue { get; set; }
        public decimal TongPhat { get; set; }
        public decimal ThucLanh { get; set; }

        public string TrangThai { get; set; }
    }
}