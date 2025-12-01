using System;
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
        
        private int _selectedMonth;
        private int _selectedYear;

        public PayrollView()
        {
            InitializeComponent();
            _nhanVienService = new NhanVienService();
            _payrollResultService = new PayrollResultService();
            
            // Đặt mặc định là tháng/năm hiện tại
            _selectedMonth = DateTime.Now.Month;
            _selectedYear = DateTime.Now.Year;

            // Hiển thị lên TextBox
            txtMonth.Text = _selectedMonth.ToString();
            txtYear.Text = _selectedYear.ToString();

            LoadPayrollData();
        }

        // Hàm xử lý sự kiện khi nhấn nút "Xem"
        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (int.TryParse(txtMonth.Text, out int m) && int.TryParse(txtYear.Text, out int y))
            {
                if (m < 1 || m > 12)
                {
                    MessageBox.Show("Tháng phải từ 1 đến 12!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (y < 2000 || y > 2100) // Giới hạn năm hợp lý
                {
                    MessageBox.Show("Năm không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _selectedMonth = m;
                _selectedYear = y;
                LoadPayrollData();
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tháng và năm là số!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPayrollData()
        {
            _payrollResults = new List<PayrollResult>();
            
            // Cập nhật tiêu đề trước khi load
            HeaderTitle.Text = $"Bảng lương tháng {_selectedMonth:D2}/{_selectedYear}";

            var listNhanVien = _nhanVienService.GetListNhanVien();

            foreach (var nv in listNhanVien)
            {
                var result = _payrollResultService.GetPayrollResultForEmployee(nv, _selectedMonth, _selectedYear);
                
                
                _payrollResults.Add(result);
            }

            PayrollDataGrid.ItemsSource = null; // Reset source để đảm bảo grid cập nhật lại
            PayrollDataGrid.ItemsSource = _payrollResults;
        }

        private void ViewDetail_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedItem = button?.DataContext as PayrollResult;

            if (selectedItem != null)
            {
                MessageBox.Show(
                    $"Chi tiết lương nhân viên: {selectedItem.TenNV}\n" +
                    $"Trạng thái: {selectedItem.TrangThai}\n" +
                    $"Lương thực nhận: {selectedItem.LuongThucNhan:N0} VNĐ",
                    "Chi tiết lương",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                $"Xuất Excel bảng lương tháng {_selectedMonth:D2}/{_selectedYear}",
                "Xuất Excel",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}