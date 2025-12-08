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
            
            // Khởi tạo các Service
            _payrollResultService = new PayrollResultService();
            _nhanVienService = new NhanVienService();

            LoadAvailableMonths();
            _isLoaded = true;
        }

        /// <summary>
        /// Tạo danh sách các tháng (ví dụ 12 tháng gần nhất) để đưa vào ComboBox
        /// </summary>
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
                    Display = $"Tháng {d.Month}/{d.Year}", 
                    Month = d.Month, 
                    Year = d.Year 
                });
            }

            cboMonth.ItemsSource = months;
            cboMonth.DisplayMemberPath = "Display";
            cboMonth.SelectedIndex = 0; // Mặc định chọn tháng hiện tại
        }

        private void CboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded)
            {
                LoadPayslipData();
            }
        }

        /// <summary>
        /// Hàm xử lý chính: Lấy dữ liệu và Binding lên giao diện
        /// </summary>
        private void LoadPayslipData()
        {
            // 1. Kiểm tra đăng nhập (Giả sử UserSession có MaNV)
            // Lưu ý: Bạn cần đảm bảo UserSession.MaNV có giá trị hợp lệ
            if (!UserSession.IsLoggedIn || UserSession.MaNV == null)

            {

                MessageBox.Show("Vui lòng đăng nhập bằng tài khoản nhân viên.");

                return;

            }


            int maNV = Convert.ToInt32(UserSession.MaNV);

            

            // 2. Lấy thông tin tháng/năm đang chọn
            if (cboMonth.SelectedItem is not MonthOption selectedMonth) return;

            try
            {
                // 3. Lấy thông tin nhân viên (để lấy Chức vụ, Phòng ban, Hệ số kiêm nhiệm)
                // Cần đảm bảo hàm GetEmployeeById đã Include các bảng liên quan (ChucVu, PhongBan)
                NhanVien nhanVien = _nhanVienService.GetEmployeeById(maNV);

                if (nhanVien == null)
                {
                    ShowError("Không tìm thấy thông tin nhân viên.");
                    return;
                }

                // 4. Gọi Service tính lương
                PayrollResult result = _payrollResultService.GetPayrollResultForEmployee(nhanVien, selectedMonth.Month, selectedMonth.Year);

                // 5. Kiểm tra kết quả
                // Nếu chưa có dữ liệu chấm công hoặc lương = 0, có thể coi là chưa có dữ liệu
                if (result.LuongThucNhan == 0 && result.TongNgayCong == 0)
                {
                    ShowError(); // Hiện thông báo không có dữ liệu
                    return;
                }

                // 6. Ánh xạ sang ViewModel để Binding lên XAML
                // Logic hiển thị Phòng ban và Chức vụ
                string tenPB = nhanVien.PhongBan != null && nhanVien.PhongBan.Count > 0 
                               ? nhanVien.PhongBan[0].TenPB 
                               : "Chưa phân bổ";
                
                string tenCV = nhanVien.ChucVu != null && nhanVien.ChucVu.Count > 0 
                               ? nhanVien.ChucVu[0].TenCV 
                               : "N/A";

                var viewModel = new MyPayslipDTO()
                {
                    HoTen = nhanVien.HoTen,
                    TenChucVu = tenCV,
                    TenPB = tenPB,
                    
                    // Thời gian kỳ lương (Đầu tháng -> Cuối tháng)
                    TuNgay = new DateTime(selectedMonth.Year, selectedMonth.Month, 1),
                    DenNgay = new DateTime(selectedMonth.Year, selectedMonth.Month, DateTime.DaysInMonth(selectedMonth.Year, selectedMonth.Month)),

                    // Các khoản thu nhập
                    LuongCoBan = result.LuongCoBan ?? 0,
                    TongGioLam = result.TongNgayCong ?? 0, // XAML ghi là "giờ" nhưng logic service đang tính theo "ngày công"
                    LuongTheoNgayCong = result.luongchinh ?? 0,
                    TongPhuCap = result.TongPhuCap,
                    
                    // Kiêm nhiệm
                    HeSoKiemNhiem = nhanVien.HeSoKiemNhiem, // Lấy từ Model NV
                    TienKiemNhiem = result.TongTienKiemNhiem ?? 0,

                    // Các khoản khấu trừ
                    // Trong Service bạn: TongKhauTru (gồm phạt,...) và TongThue.
                    // XAML có: Bảo hiểm, Thuế, Tạm ứng.
                    // Ta map tạm: TongKhauTru -> Tạm ứng/Phạt. TongThue -> Thuế. Bảo hiểm -> 0 (vì service chưa tính riêng).
                    TongBaoHiem = 0, 
                    TongThue = result.TongThue,
                    TienUng = result.TongKhauTru,

                    // Tổng kết
                    ThucLanh = result.LuongThucNhan ?? 0,
                    TrangThai = string.IsNullOrEmpty(result.TrangThai) ? "Chưa chốt" : result.TrangThai
                };

                // 7. Binding
                pnlPayslipContent.DataContext = viewModel;
                
                // Hiển thị nội dung, ẩn thông báo lỗi
                pnlPayslipContent.Visibility = Visibility.Visible;
                txtNoData.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị bảng lương: " + ex.Message);
            }
        }

        private void ShowError(string msg = null)
        {
            pnlPayslipContent.Visibility = Visibility.Collapsed;
            txtNoData.Visibility = Visibility.Visible;
            if (msg != null) txtNoData.Text = msg;
            else txtNoData.Text = "Không có dữ liệu lương cho tháng này.";
        }
    }

    // ===============================================
    // CÁC CLASS HỖ TRỢ (VIEW MODELS)
    // ===============================================

    public class MonthOption
    {
        public string Display { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    // Class này khớp với Binding trong XAML của bạn
   
}