using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
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
            var nhanViens = _service.GetListNhanVien();

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
                    
                    // tạm thời chưa lọc theo phòng ban và chức vụ 
                    
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
            // Mở form thêm nhân viên
            var window = new AddEmployeeWindow();
            window.ShowDialog();
            LoadNhanVien();
        }

        
        
        
        // Xem chi tiết nhân viên
        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string maNV = button?.Tag?.ToString();

            if (!string.IsNullOrEmpty(maNV) && int.TryParse(maNV, out int id))
            {
                // Lấy thông tin nhân viên
                var employee = _allEmployees.FirstOrDefault(emp => emp.MaNV == id);

                // Lấy danh sách phòng ban của nhân viên
                var listPhongBan = "";

                if (employee.PhongBan != null && employee.PhongBan.Count > 0)
                {
                    foreach (var pb in employee.PhongBan)
                    {
                        listPhongBan += pb.TenPB + ", ";
                    }

                    // Xóa dấu phẩy cuối nếu có
                    listPhongBan = listPhongBan.TrimEnd(',', ' ');
                }
                else
                {
                    listPhongBan = "Không thuộc phòng ban nào ";
                }
                
                // lấy danh sách chức vụ của nhân viên
                var listChucVu = "";

                if (employee.ChucVu != null && employee.ChucVu.Count > 0)
                {
                    foreach (var chv in employee.ChucVu)
                    {
                        listChucVu += chv.TenCV + ", ";
                    }
                    listChucVu = listChucVu.TrimEnd(',', ' ');
                }
                else
                {
                    listChucVu = "khong thuoc chuc vu nao";
                }
                

                if (employee != null)
                {
                    // Tạo danh sách thông tin hiển thị
                    var infoItems = new List<InfoItem>
                    {
                        new InfoItem("Mã nhân viên:", employee.MaNV.ToString(), "#1F2937", true),
                        new InfoItem("Họ và tên:", employee.HoTen, "#1F2937", true),
                        new InfoItem("Ngày sinh:", employee.NgaySinh.ToString()),
                        new InfoItem("Điện thoại:", employee.DienThoai),
                        new InfoItem("Phòng ban:", listPhongBan, "#2563EB", true),
                        new InfoItem("Chức vụ:", listChucVu, "#2563EB", true),
                        new InfoItem("Ngày vào làm:", employee.NgayVaoLam?.ToString("dd/MM/yyyy") ?? "Chưa có"),
                        new InfoItem("Trạng thái:", employee.TrangThai, "#10B981", true),
                       
                        
                        // new InfoItem("Lương cơ bản:", $"{employee.ChucVu?.LuongCB:N0} VNĐ", "#10B981", true),
                       //new InfoItem("Phụ cấp:", $"{employee.ChucVu?.PhuCap:N0} VNĐ", "#10B981", true)
                    };

                    // Mở cửa sổ hiển thị
                    var window = new InfoDisplayWindow();
                    window.SetInfo("THÔNG TIN NHÂN VIÊN", employee.HoTen, infoItems);
                    window.ShowDialog();
                }
            }
        }


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
        {
            var button = sender as Button;
            if (button == null) return;

            // Lấy MaNV từ Tag
            if (!int.TryParse(button.Tag?.ToString(), out int maNV))
            {
                MessageBox.Show("Mã nhân viên không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Tạo đối tượng NhanVien (nếu cần, tùy service)
            var nhanVien = new NhanVien { MaNV = maNV };

            // Xác nhận xóa
            var result = MessageBox.Show($"Bạn có chắc muốn xóa nhân viên {maNV} không?", 
                "Xác nhận xóa", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            // Gọi service/repository xóa
            bool deleted = false;
            try
            {
                deleted = _service.DeleteNhanVien(nhanVien); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa nhân viên: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Thông báo kết quả
            if (deleted)
            {
                MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadNhanVien(); 
            }
            else
            {
                MessageBox.Show("Không tìm thấy nhân viên hoặc xóa thất bại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        
        
        
        private void BtnEditStatus_Click(object sender, RoutedEventArgs e)
        {
            // Lấy Button và MaNV
            if (sender is not Button button) return;
            if (!int.TryParse(button.Tag?.ToString(), out int maNV)) return;

            // Hiển thị MessageBox hỏi xác nhận
            var result = MessageBox.Show(
                "Bạn có chắc muốn thay đổi trạng thái nhân viên này không?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return; // nếu chọn No thì dừng

            // Cập nhật trạng thái
            bool ok = _service.UpdateSate(maNV); // hàm toggle trạng thái

            if (ok)
            {
                MessageBox.Show("Cập nhật trạng thái thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Load lại DataGrid
                LoadNhanVien();
            }
            else
            {
                MessageBox.Show("Cập nhật trạng thái thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        

    }
}