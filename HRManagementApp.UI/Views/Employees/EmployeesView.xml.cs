using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HRManagementApp.Services;
using HRManagementApp.models;

namespace HRManagementApp.Views
{
    public partial class EmployeesView : UserControl
    {
        private readonly NhanVienService _service;
        private ObservableCollection<NhanVien> _allEmployees;
        private ObservableCollection<NhanVien> _filteredEmployees;

        public EmployeesView()
        {
            InitializeComponent();
            _service = new NhanVienService();
            LoadNhanVien();
        }   

        private void LoadNhanVien()
        {
            // Lấy danh sách nhân viên từ Service
            var nhanViens = _service.GetNhanVienDayDu();
            
            // Nếu danh sách rỗng hoặc null, tránh crash
            if (nhanViens == null || nhanViens.Count == 0)
            {
                EmployeesDataGrid.ItemsSource = null;
                _allEmployees = new ObservableCollection<NhanVien>();
                _filteredEmployees = new ObservableCollection<NhanVien>();
                return;
            }

            _allEmployees = new ObservableCollection<NhanVien>(nhanViens);
            _filteredEmployees = new ObservableCollection<NhanVien>(nhanViens);
            EmployeesDataGrid.ItemsSource = _filteredEmployees;
        }

        // Xử lý tìm kiếm
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TxtSearch.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                // Hiển thị tất cả nếu không có từ khóa
                _filteredEmployees.Clear();
                foreach (var emp in _allEmployees)
                {
                    _filteredEmployees.Add(emp);
                }
            }
            else
            {
                // Lọc theo tên, mã NV, phòng ban, chức vụ, điện thoại
                var filtered = _allEmployees.Where(e =>
                    (!string.IsNullOrEmpty(e.HoTen) && e.HoTen.ToLower().Contains(searchText)) ||
                    (e.MaNV.ToString().Contains(searchText)) ||
                    (e.PhongBan != null && e.PhongBan.TenPB.ToLower().Contains(searchText)) ||
                    (e.ChucVu != null && e.ChucVu.TenCV.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(e.DienThoai) && e.DienThoai.Contains(searchText))
                ).ToList();


                _filteredEmployees.Clear();
                foreach (var emp in filtered)
                {
                    _filteredEmployees.Add(emp);
                }
            }
        }
        

        // Thêm nhân viên mới
        private void BtnThemNV_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Mở form thêm nhân viên mới", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // TODO: Mở Window/Dialog thêm nhân viên
            // Sau khi thêm thành công, gọi LoadNhanVien() để refresh
        }

        // Xem chi tiết nhân viên
        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string maNV = button?.Tag?.ToString();
            
            if (!string.IsNullOrEmpty(maNV) && int.TryParse(maNV, out int id))
            {
                var employee = _allEmployees.FirstOrDefault(emp => emp.MaNV == id);
                if (employee != null)
                {
                    // Tạo danh sách thông tin
                    var infoItems = new List<InfoItem>
                    {
                        new InfoItem("Mã nhân viên:", employee.MaNV.ToString(), "#1F2937", true),
                        new InfoItem("Họ và tên:", employee.HoTen, "#1F2937", true),
                        new InfoItem("Ngày sinh:", employee.NgaySinh.ToString()),
                        
                        new InfoItem("Điện thoại:", employee.DienThoai),
                        
                        new InfoItem("Phòng ban:", employee.PhongBan?.TenPB ?? "Chưa có", "#2563EB", true),
                        new InfoItem("Chức vụ:", employee.ChucVu?.TenCV ?? "Chưa có", "#2563EB", true),
                        new InfoItem("Ngày vào làm:", employee.NgayVaoLam?.ToString("dd/MM/yyyy") ?? "Chưa có"),
                        new InfoItem("Trạng thái:", employee.TrangThai, "#10B981", true),
                        new InfoItem("Lương cơ bản:", $"{employee.ChucVu?.LuongCB:N0} VNĐ", "#10B981", true),
                        new InfoItem("Phụ cấp:", $"{employee.ChucVu?.PhuCap:N0} VNĐ", "#10B981", true)
                    };

                    // Mở window hiển thị
                    var window = new InfoDisplayWindow();
                    window.SetInfo(
                        "THÔNG TIN NHÂN VIÊN",
                        employee.HoTen,
                        infoItems
                    );
                    window.ShowDialog();
                }
            }
        }

        // Chỉnh sửa nhân viên
        // Chỉnh sửa nhân viên
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string maNV = button?.Tag?.ToString();
            
            if (!string.IsNullOrEmpty(maNV) && int.TryParse(maNV, out int id))
            {
                var employee = _allEmployees.FirstOrDefault(emp => emp.MaNV == id);
                if (employee == null)
                {
                    MessageBox.Show("Không tìm thấy nhân viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Tạo danh sách các trường có thể chỉnh sửa
                var editItems = new List<InfoEditItem>
                {
                    new InfoEditItem 
                    { 
                        Label = "Họ và tên:", 
                        Value = employee.HoTen, 
                        PropertyName = "HoTen" 
                    },
                    new InfoEditItem 
                    { 
                        Label = "Ngày sinh:", 
                        Value = employee.NgaySinh?.ToString("dd/MM/yyyy") ?? "", 
                        PropertyName = "NgaySinh" 
                    },
                    new InfoEditItem 
                    { 
                        Label = "Số CCCD:", 
                        Value = employee.SoCCCD ?? "", 
                        PropertyName = "SoCCCD" 
                    },
                    new InfoEditItem 
                    { 
                        Label = "Điện thoại:", 
                        Value = employee.DienThoai ?? "", 
                        PropertyName = "DienThoai" 
                    },
                    new InfoEditItem 
                    { 
                        Label = "Mã phòng ban:", 
                        Value = employee.MaPB?.ToString() ?? "", 
                        PropertyName = "MaPB" 
                    },
                    new InfoEditItem 
                    { 
                        Label = "Tên phòng ban:", 
                        Value = employee.PhongBan?.TenPB ?? "Chưa có", 
                        PropertyName = "TenPB" 
                    },
                    new InfoEditItem 
                    { 
                        Label = "Mã chức vụ:", 
                        Value = employee.MaCV?.ToString() ?? "", 
                        PropertyName = "MaCV" 
                    },
                    new InfoEditItem 
                    { 
                        Label = "Tên chức vụ:", 
                        Value = employee.ChucVu?.TenCV ?? "Chưa có", 
                        PropertyName = "TenCV" 
                    },
                    new InfoEditItem 
                    { 
                        Label = "Ngày vào làm:", 
                        Value = employee.NgayVaoLam?.ToString("dd/MM/yyyy") ?? "", 
                        PropertyName = "NgayVaoLam" 
                    },
                    new InfoEditItem 
                    { 
                        Label = "Trạng thái:", 
                        Value = employee.TrangThai ?? "Đang làm việc", 
                        PropertyName = "TrangThai" 
                    }
                };

                // Mở window chỉnh sửa
                var editWindow = new InfoEditWindow();
                editWindow.LoadData(
                    "CHỈNH SỬA THÔNG TIN NHÂN VIÊN",
                    $"Mã NV: {employee.MaNV}",
                    editItems
                );
                editWindow.ShowDialog();

                // Nếu người dùng bấm Lưu
                if (editWindow.IsSaved)
                {
                    try
                    {
                        // Cập nhật thông tin từ form vào object employee
                        foreach (var item in editWindow.EditedData)
                        {
                            switch (item.PropertyName)
                            {
                                case "HoTen":
                                    employee.HoTen = item.Value;
                                    break;

                                case "NgaySinh":
                                    if (DateTime.TryParseExact(item.Value, "dd/MM/yyyy", 
                                        System.Globalization.CultureInfo.InvariantCulture, 
                                        System.Globalization.DateTimeStyles.None, out DateTime ngaySinh))
                                    {
                                        employee.NgaySinh = ngaySinh;
                                    }
                                    break;

                                case "SoCCCD":
                                    employee.SoCCCD = item.Value;
                                    break;

                                case "DienThoai":
                                    employee.DienThoai = item.Value;
                                    break;

                                case "MaPB":
                                    if (int.TryParse(item.Value, out int maPB))
                                    {
                                        employee.MaPB = maPB;
                                    }
                                    break;

                                case "MaCV":
                                    if (int.TryParse(item.Value, out int maCV))
                                    {
                                        employee.MaCV = maCV;
                                    }
                                    break;

                                case "NgayVaoLam":
                                    if (DateTime.TryParseExact(item.Value, "dd/MM/yyyy", 
                                        System.Globalization.CultureInfo.InvariantCulture, 
                                        System.Globalization.DateTimeStyles.None, out DateTime ngayVaoLam))
                                    {
                                        employee.NgayVaoLam = ngayVaoLam;
                                    }
                                    break;

                                case "TrangThai":
                                    employee.TrangThai = item.Value;
                                    break;
                            }
                        }

                        // Gọi service để cập nhật vào database
                        bool success = _service.UpdateNhanVien(employee);

                        if (success)
                        {
                            // Refresh lại danh sách để hiển thị thông tin mới
                            LoadNhanVien();
                            
                            MessageBox.Show(
                                "Cập nhật thông tin nhân viên thành công!",
                                "Thành công",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show(
                                "Cập nhật thông tin thất bại!",
                                "Lỗi",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Lỗi khi cập nhật nhân viên: {ex.Message}",
                            "Lỗi",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        // Xóa nhân viên
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {/*
            var button = sender as Button;
            string maNV = button?.Tag?.ToString();
            
            if (!string.IsNullOrEmpty(maNV))
            {
                var employee = _allEmployees.FirstOrDefault(emp => emp.MaNV == maNV);
                if (employee == null)
                {
                    MessageBox.Show("Không tìm thấy nhân viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa nhân viên?\n\n" +
                    $"Mã NV: {employee.MaNV}\n" +
                    $"Họ tên: {employee.HoTen}\n\n" +
                    $"Hành động này không thể hoàn tác!",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Gọi service để xóa
                        bool success = _service.DeleteNhanVien(maNV);
                        
                        if (success)
                        {
                            _allEmployees.Remove(employee);
                            _filteredEmployees.Remove(employee);
                            MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Xóa nhân viên thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }*/
        }
    }
}