using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class EditPayroll : UserControl
    {
        private NhanVienService _nhanVienService;
        private List<NhanVien> _allEmployees;
        private List<NhanVien> _filteredEmployees;

        public EditPayroll()
        {
            InitializeComponent();
            _nhanVienService = new NhanVienService();
            LoadData();
        }

        private void LoadData()
        {
            _allEmployees = _nhanVienService.GetListNhanVien();
            _filteredEmployees = _allEmployees;
            EmployeeDataGrid.ItemsSource = _filteredEmployees;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                _filteredEmployees = _allEmployees;
            }
            else
            {
                _filteredEmployees = _allEmployees.Where(nv =>
                    nv.MaNV.ToString().ToLower().Contains(searchText) ||
                    nv.HoTen.ToLower().Contains(searchText)
                ).ToList();
            }

            EmployeeDataGrid.ItemsSource = null;
            EmployeeDataGrid.ItemsSource = _filteredEmployees;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            LoadData();
            MessageBox.Show("Đã làm mới danh sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ===========================
        // QUẢN LÝ VAI TRÒ / CHỨC VỤ
        // ===========================
        private void EditRole_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.DataContext as NhanVien;

            if (employee != null)
            {
                // Khởi tạo Window mới, truyền nhân viên đang chọn vào
                var roleWindow = new RoleManagementWindow(employee);
        
                // Hiển thị dưới dạng Dialog (người dùng phải tắt window con mới thao tác được window cha)
                roleWindow.ShowDialog();

                // Sau khi tắt window con, có thể cần load lại data grid chính để cập nhật nếu có thay đổi hiển thị
                // LoadData(); 
            }
        }
        
        
        

        // ===========================
        // QUẢN LÝ PHỤ CẤP
        // ===========================
        private void EditAllowance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.DataContext as NhanVien;

            if (employee != null)
            {
                // Mở cửa sổ quản lý phụ cấp
                var allowanceWindow = new AllowanceManagementWindow(employee);
                allowanceWindow.ShowDialog();
        
                // Load lại data grid chính nếu cần cập nhật tổng tiền
                // LoadData(); 
            }
        }

        // ===========================
        // QUẢN LÝ KHẤU TRỪ
        // ===========================
        private void EditDeduction_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.DataContext as NhanVien;

            if (employee != null)
            {
                // Mở cửa sổ quản lý khấu trừ
                var deductionWindow = new DeductionManagementWindow(employee);
                deductionWindow.ShowDialog();
        
                // LoadData(); // Load lại Grid chính nếu cần
            }
        }

        // ===========================
        // QUẢN LÝ THUẾ
        // ===========================
        private void EditTax_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.DataContext as NhanVien;

            if (employee != null)
            {
                // Mở cửa sổ quản lý thuế
                var taxWindow = new TaxManagementWindow(employee);
                taxWindow.ShowDialog();
        
                // LoadData(); // Cập nhật lại màn hình chính nếu cần
            }
        }

        // ===========================
        // XEM TỔNG HỢP
        // ===========================
        private void ViewSummary_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.DataContext as NhanVien;

            if (employee != null)
            {
                // Tính toán tổng hợp
                var roles = string.Join(", ", employee.DanhSachChucVu.Select(cv => cv.ChucVu.TenCV));
                
                var totalAllowance = employee.PhuCaps
                    .Where(p => p.ApDungDenNgay == null || p.ApDungDenNgay >= System.DateTime.Now)
                    .Sum(p => p.SoTien);

                var totalTax = employee.Thues
                    .Where(t => t.ApDungDenNgay == null || t.ApDungDenNgay >= System.DateTime.Now)
                    .Sum(t => t.SoTien);

                var currentMonth = System.DateTime.Now;
                var monthlyDeduction = employee.KhauTrus
                    .Where(k => k.Ngay.Month == currentMonth.Month && k.Ngay.Year == currentMonth.Year)
                    .Sum(k => k.SoTien);

                MessageBox.Show(
                    $"TỔNG HỢP THÔNG TIN LƯƠNG\n" +
                    $"══════════════════════════════\n\n" +
                    $"Nhân viên: {employee.HoTen}\n" +
                    $"Mã NV: {employee.MaNV}\n\n" +
                    $"📋 Vai trò: {(string.IsNullOrEmpty(roles) ? "Chưa có" : roles)}\n" +
                    $"💰 Phụ cấp: {totalAllowance:N0} VNĐ\n" +
                    $"📊 Thuế: {totalTax:N0} VNĐ\n" +
                    $"📉 Khấu trừ (tháng này): {monthlyDeduction:N0} VNĐ\n",
                    "Tổng hợp thông tin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Để mở form chi tiết:
                // var summaryDialog = new EmployeeSummaryDialog(employee);
                // summaryDialog.ShowDialog();
            }
        }
    }
}