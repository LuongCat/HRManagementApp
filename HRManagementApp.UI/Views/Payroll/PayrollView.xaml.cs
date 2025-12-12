using System;
using System.Collections.Generic;
using System.Linq; // Cần thêm dòng này để dùng LINQ (Where, ToList)
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
        private LuongService _luongService;
        private PhongBanService _phongBanService;

        // Dùng biến này để lưu danh sách gốc
        private List<PayrollResult> _allPayrollResults; 
        
        private int _selectedMonth;
        private int _selectedYear;

        public PayrollView()
        {
            InitializeComponent();
            _nhanVienService = new NhanVienService();
            _payrollResultService = new PayrollResultService();
            _luongService = new LuongService();
            _phongBanService = new PhongBanService();
            _allPayrollResults = new List<PayrollResult>();

            // Đặt mặc định là tháng/năm hiện tại
            _selectedMonth = DateTime.Now.Month;
            _selectedYear = DateTime.Now.Year;

            // Hiển thị lên TextBox
            txtMonth.Text = _selectedMonth.ToString();
            txtYear.Text = _selectedYear.ToString();

            LoadPayrollData();
            LoadComboBoxPhongBan();
        }
        
        private void LoadComboBoxPhongBan()
        {
            try 
            {
                var listPB = _phongBanService.GetListPhongBan();
                
                // Thêm mục mặc định "Tất cả"
                var allItem = new PhongBan { MaPB = -1, TenPB = "--- Tất cả phòng ban ---" };
                listPB.Insert(0, allItem);

                cboPhongBan.ItemsSource = listPB;
                cboPhongBan.SelectedIndex = 0; // Chọn mặc định là Tất cả
            }
            catch(Exception ex)
            {
                // Xử lý nếu chưa có PhongBanService
                MessageBox.Show("Chưa tải được danh sách phòng ban: " + ex.Message);
            }
        }
        
        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtMonth.Text, out int m) && int.TryParse(txtYear.Text, out int y))
            {
                if (m < 1 || m > 12)
                {
                    MessageBox.Show("Tháng phải từ 1 đến 12!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (y < 2000 || y > 2100) 
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
            // Reset list gốc
            _allPayrollResults = new List<PayrollResult>();

            HeaderTitle.Text = $"Bảng lương tháng {_selectedMonth:D2}/{_selectedYear}";

            var listNhanVien = _nhanVienService.GetListNhanVien();

            foreach (var nv in listNhanVien)
            {
                var result = _payrollResultService.GetPayrollResultForEmployee(nv, _selectedMonth, _selectedYear);
                _allPayrollResults.Add(result);
            }

            ApplyFilters();
        }
        // --- MỚI: Xử lý sự kiện khi chọn ComboBox ---
        private void cboPhongBan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        // --- CẬP NHẬT: Xử lý tìm kiếm gọi về hàm chung ---
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        // --- MỚI: Hàm Lọc trung tâm (Logic cốt lõi) ---
        private void ApplyFilters()
        {
            // 1. Lấy dữ liệu gốc
            var filteredList = _allPayrollResults.AsEnumerable();

            // 2. Lọc theo Phòng ban (nếu không chọn "Tất cả" có MaPB = -1)
            if (cboPhongBan.SelectedValue != null)
            {
                if (int.TryParse(cboPhongBan.SelectedValue.ToString(), out int selectedMaPB))
                {
                    if (selectedMaPB != -1) // -1 là tất cả
                    {
                        // Lưu ý: PayrollResult cần có thuộc tính MaPB
                        filteredList = filteredList.Where(x => x.maPB == selectedMaPB);
                    }
                }
            }

            // 3. Lọc theo Tên (Search text)
            string keyword = txtSearch.Text.ToLower().Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                filteredList = filteredList.Where(x => x.TenNV.ToLower().Contains(keyword));
            }

            // 4. Hiển thị kết quả
            UpdateDataGridSource(filteredList.ToList());
        }

       

        // Hàm phụ trợ để gán Source cho DataGrid an toàn
        private void UpdateDataGridSource(List<PayrollResult> data)
        {
            PayrollDataGrid.ItemsSource = null;
            PayrollDataGrid.ItemsSource = data;
        }

        // --- Các hàm xử lý nút bấm giữ nguyên logic cũ ---

        private void ViewDetail_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedItem = button?.DataContext as PayrollResult;
            if (selectedItem == null) return;

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

                // Kiểm tra trạng thái
                if (selectedItem.maLuong > 0)
                {
                    var statusDB = _luongService.GetChotLuongStatusByMaLuong(selectedItem.maLuong);
                    if (statusDB == "Đã chốt")
                    {
                        MessageBox.Show("Bảng lương này ĐÃ ĐƯỢC CHỐT trước đó.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var confirm = MessageBox.Show(
                    $"Bạn có chắc chắn muốn CHỐT LƯƠNG cho {selectedItem.TenNV}?\nThực nhận: {selectedItem.LuongThucNhan:N0} VNĐ",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm != MessageBoxResult.Yes) return;

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
                    ChotLuong = "Đã chốt"
                };

                bool success = false;
                if (luong.MaLuong == 0)
                {
                    selectedItem.maLuong = _luongService.AddSalaryReturnID(luong);
                    success = selectedItem.maLuong > 0;
                    selectedItem.TrangThai = "Đã chốt"; // Cập nhật UI tạm thời
                }
                else
                {
                    success = _luongService.UpdateSalary(luong);
                }

                if (success)
                {
                    MessageBox.Show("Đã chốt lương thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Cập nhật lại trong danh sách gốc để chức năng tìm kiếm vẫn đúng dữ liệu mới
                    var itemInList = _allPayrollResults.FirstOrDefault(x => x.maNV == selectedItem.maNV);
                    if (itemInList != null)
                    {
                        itemInList.TrangThai = "Đã chốt";
                        itemInList.maLuong = selectedItem.maLuong;
                    }
                    
                    // Refresh lại view
                    PayrollDataGrid.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi lưu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

                if (selectedItem.maLuong == 0)
                {
                    MessageBox.Show("Vui lòng CHỐT LƯƠNG trước khi thanh toán!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var chotLuongStatus = _luongService.GetChotLuongStatusByMaLuong(selectedItem.maLuong);
                if (chotLuongStatus != "Đã chốt")
                {
                    MessageBox.Show("Bảng lương này CHƯA ĐƯỢC CHỐT.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedItem.TrangThai == "Đã trả" || selectedItem.TrangThai == "Đã thanh toán")
                {
                    MessageBox.Show("Đã thanh toán rồi.", "Thông tin", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var confirm = MessageBox.Show(
                    $"Xác nhận THANH TOÁN lương cho {selectedItem.TenNV}?\nSố tiền: {selectedItem.LuongThucNhan:N0} VNĐ",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm != MessageBoxResult.Yes) return;

                bool success = _luongService.UpdateTrangThai(selectedItem.maLuong, "Đã trả");

                if (success)
                {
                    MessageBox.Show("Thanh toán thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Cập nhật UI và danh sách gốc
                    selectedItem.TrangThai = "Đã trả";
                    var itemInList = _allPayrollResults.FirstOrDefault(x => x.maNV == selectedItem.maNV);
                    if (itemInList != null) itemInList.TrangThai = "Đã trả";

                    PayrollDataGrid.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("Thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            // Logic xuất Excel của bạn
        }
    }
}