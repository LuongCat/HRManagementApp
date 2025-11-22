using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class PayrollView : UserControl
    {
        private NhanVienService _nhanVienService;
        private PayrollResultService _payrollResultService;
        private List<PayrollResult> _payrollResults;
        
        private int _selectedMonth = 9;
        private int _selectedYear = 2024;

        public PayrollView()
        {
            InitializeComponent();
            _nhanVienService = new NhanVienService();
            _payrollResultService = new PayrollResultService();
            
            InitializeMonthYearComboBox();
            LoadPayrollData();
        }

        private void InitializeMonthYearComboBox()
        {
            // Tạo danh sách 12 tháng gần nhất
            var months = new List<MonthYearItem>();
            var currentDate = System.DateTime.Now;
            
            for (int i = 0; i < 12; i++)
            {
                var date = currentDate.AddMonths(-i);
                months.Add(new MonthYearItem
                {
                    Month = date.Month,
                    Year = date.Year,
                    Display = $"Tháng {date.Month:D2}/{date.Year}"
                });
            }
            
            MonthYearComboBox.ItemsSource = months;
            MonthYearComboBox.SelectedIndex = 0;
        }

        private void LoadPayrollData()
        {
            _payrollResults = new List<PayrollResult>();
            var listNhanVien = _nhanVienService.GetListNhanVien();

            foreach (var nv in listNhanVien)
            {
                var result = _payrollResultService.GetPayrollResultForEmployee(nv, _selectedMonth, _selectedYear);
                _payrollResults.Add(result);
            }

            PayrollDataGrid.ItemsSource = _payrollResults;
            
            // Cập nhật tiêu đề
            HeaderTitle.Text = $"Bảng lương tháng {_selectedMonth:D2}/{_selectedYear}";
        }

        private void MonthYearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthYearComboBox.SelectedItem is MonthYearItem selected)
            {
                _selectedMonth = selected.Month;
                _selectedYear = selected.Year;
                LoadPayrollData();
            }
        }

        private void ViewDetail_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedItem = button?.DataContext as PayrollResult;

            if (selectedItem != null)
            {
                // Chuyển sang PayrollDetailView với thông tin nhân viên
                var parentWindow = Window.GetWindow(this) as MainWindow;
                // parentWindow?.NavigateToPayrollDetail(selectedItem);
                
                MessageBox.Show(
                    $"Chi tiết lương nhân viên: {selectedItem.TenNV}\n" +
                    $"Mã NV: {selectedItem.maNV}\n" +
                    $"Lương thực nhận: {selectedItem.LuongThucNhan:N0} VNĐ",
                    "Chi tiết lương",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement Excel export
            MessageBox.Show(
                $"Xuất Excel bảng lương tháng {_selectedMonth:D2}/{_selectedYear}",
                "Xuất Excel",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

    // Helper class cho ComboBox tháng/năm
    public class MonthYearItem
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string Display { get; set; }
    }
}