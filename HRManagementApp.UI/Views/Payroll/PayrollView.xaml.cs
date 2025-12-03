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
        private LuongService _luongService;

        private int _selectedMonth;
        private int _selectedYear;

        public PayrollView()
        {
            InitializeComponent();
            _nhanVienService = new NhanVienService();
            _payrollResultService = new PayrollResultService();
            _luongService = new LuongService();

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
                MessageBox.Show("Vui lòng nhập tháng và năm là số!", "Lỗi nhập liệu", MessageBoxButton.OK,
                    MessageBoxImage.Error);
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

            NhanVien nhanVien = _nhanVienService.GetEmployeeById(selectedItem.maNV);
            var detailWindow = new EmployeeSummaryWindow(nhanVien, _selectedMonth, _selectedYear);
            detailWindow.ShowDialog();
        }

        private void ConfirmSalary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var selectedItem = button?.DataContext as PayrollResult;
                
                if (selectedItem == null) return;

                // 1. Kiểm tra: Nếu đã chốt rồi thì không cho chốt lại (tùy nghiệp vụ)
                if (selectedItem.maLuong > 0)
                {
                    // Kiểm tra kỹ trong DB xem trạng thái thực tế là gì
                    var statusDB = _luongService.GetChotLuongStatusByMaLuong(selectedItem.maLuong);
                    
                    if (statusDB == "đã chốt")
                    {
                        MessageBox.Show("Bảng lương này ĐÃ ĐƯỢC CHỐT trước đó, không cần chốt lại.",
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // 2. Hỏi xác nhận
                var confirm = MessageBox.Show(
                    $"Bạn có chắc chắn muốn CHỐT LƯƠNG cho nhân viên {selectedItem.TenNV}?\n" +
                    $"Thực nhận: {selectedItem.LuongThucNhan:N0} VNĐ",
                    "Xác nhận chốt lương",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm != MessageBoxResult.Yes) return;

                // 3. Chuẩn bị dữ liệu
                Luong luong = new Luong
                {
                    MaLuong = selectedItem.maLuong,
                    MaNV = selectedItem.maNV,
                    Thang = selectedItem.Thang,
                    Nam = selectedItem.Nam,
                    TongNgayCong = selectedItem.TongNgayCong ?? 0,

                    TienLuong = selectedItem.LuongCoBan,
                    LuongThucNhan = selectedItem.LuongThucNhan,

                    TrangThai = "Chưa trả",
                    ChotLuong = "đã chốt"
                };

                // 4. Thực hiện Lưu xuống DB
                bool success = false;
                if (luong.MaLuong == 0)
                {
                    // Thêm mới
                    selectedItem.maLuong= _luongService.AddSalaryReturnID(luong);
                    success = selectedItem.maLuong > 0;
                    
                    selectedItem.TrangThai = "đã chốt";
                }
                else
                {
                    // Cập nhật (trường hợp đã lưu nháp trước đó)
                    success = _luongService.UpdateSalary(luong);
                }

                // 5. Thông báo kết quả
                if (success)
                {
                    MessageBox.Show("Đã chốt lương thành công!", "Thành công", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    LoadPayrollData(); // <--- QUAN TRỌNG: Load lại DataGrid để cập nhật MaLuong và Trạng thái
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi lưu vào cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeStatusSalary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var selectedItem = button?.DataContext as PayrollResult;

                if (selectedItem == null) return;

                // 1. Kiểm tra: Nếu chưa có MaLuong (tức là chưa chốt/lưu vào DB)
                if (selectedItem.maLuong == 0)
                {
                    MessageBox.Show("Vui lòng CHỐT LƯƠNG trước khi thực hiện thanh toán!",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 2. Kiểm tra trạng thái chốt trong DB
                var chotLuongStatus = _luongService.GetChotLuongStatusByMaLuong(selectedItem.maLuong);

                if (chotLuongStatus != "đã chốt")
                {
                    MessageBox.Show("Bảng lương này CHƯA ĐƯỢC CHỐT. Không thể thanh toán.",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 3. Kiểm tra xem đã trả chưa (tránh bấm nhầm nhiều lần)
                // (Giả sử bạn có load TrangThai vào PayrollResult)
                if (selectedItem.TrangThai == "Đã trả")
                {
                    MessageBox.Show("Nhân viên này đã được thanh toán rồi.", "Thông tin", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // 4. Hỏi xác nhận
                var confirm = MessageBox.Show(
                    $"Xác nhận THANH TOÁN lương cho {selectedItem.TenNV}?\n" +
                    $"Số tiền: {selectedItem.LuongThucNhan:N0} VNĐ",
                    "Xác nhận thanh toán",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm != MessageBoxResult.Yes) return;

                // 5. Thực hiện Update
                bool success = _luongService.UpdateTrangThai(selectedItem.maLuong, "Đã trả");

                if (success)
                {
                    MessageBox.Show("Cập nhật trạng thái ĐÃ THANH TOÁN thành công!", "Thành công", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    LoadPayrollData(); // <--- Load lại để đổi màu nút hoặc hiển thị trạng thái mới
                }
                else
                {
                    MessageBox.Show("Cập nhật trạng thái thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}