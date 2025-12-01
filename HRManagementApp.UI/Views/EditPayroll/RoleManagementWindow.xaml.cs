using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views
{
    public partial class RoleManagementWindow : Window
    {
        private NhanVien _targetEmployee;
        
        // Giả lập Service - Hãy thay thế bằng Service thật của bạn
        private VaiTroNhanVienService _vaiTroNhanVienService; 
        private ChucVuService _chucVuService;

        public RoleManagementWindow(NhanVien employee)
        {
            InitializeComponent();
            _targetEmployee = employee;
            _vaiTroNhanVienService = new VaiTroNhanVienService();
            _chucVuService = new ChucVuService();
            
            // Hiển thị tên nhân viên lên header
            TxtEmployeeName.Text = $"{_targetEmployee.HoTen} (Mã: {_targetEmployee.MaNV})";

            LoadComboBoxData();
            LoadEmployeeRoles();
        }

        private void LoadComboBoxData()
        {
            // TODO: Lấy danh sách chức vụ từ Database
             var listChucVu = _chucVuService.GetAllChucVu();
            

            CboChucVu.ItemsSource = listChucVu;
        }

        private void LoadEmployeeRoles()
        {
            // TODO: Lấy danh sách vai trò của nhân viên này từ Database
             var roles = _vaiTroNhanVienService.GetVaiTroNhanVien(_targetEmployee.MaNV);

            // Sử dụng dữ liệu hiện có từ đối tượng NhanVien truyền vào (nếu đã load kèm)
            // Hoặc reload lại từ DB để đảm bảo mới nhất
            if (_targetEmployee.DanhSachChucVu == null) 
                _targetEmployee.DanhSachChucVu = new List<VaiTroNhanVien>();

            DgRoles.ItemsSource = null;
            DgRoles.ItemsSource = _targetEmployee.DanhSachChucVu;
        }

        // ==========================
        // XỬ LÝ SỰ KIỆN UI
        // ==========================

        private void DgRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgRoles.SelectedItem is VaiTroNhanVien selectedRole)
            {
                CboChucVu.SelectedValue = selectedRole.MaCV;
                CboLoaiVaiTro.Text = selectedRole.LoaiChucVu; // Combobox item content match
                TxtHeSoLuong.Text = selectedRole.HeSoLuongCoBan?.ToString("N2");
                TxtHeSoPhuCap.Text = selectedRole.HeSoPhuCapKiemNhiem?.ToString("N2");
                TxtGhiChu.Text = selectedRole.GhiChu;

                // Cập nhật trạng thái nút
                CboChucVu.IsEnabled = false; // Không cho sửa mã chức vụ (khoá chính)
                BtnAdd.IsEnabled = false;
                BtnEdit.IsEnabled = true;
                BtnDelete.IsEnabled = true;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            DgRoles.SelectedItem = null;
            CboChucVu.SelectedIndex = -1;
            CboChucVu.IsEnabled = true;
            CboLoaiVaiTro.SelectedIndex = 0;
            TxtHeSoLuong.Text = "0.00";
            TxtHeSoPhuCap.Text = "0.00";
            TxtGhiChu.Text = string.Empty;

            BtnAdd.IsEnabled = true;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        // ==========================
        // CRUD ACTIONS
        // ==========================

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var selectedChucVu = CboChucVu.SelectedItem as ChucVu;

            // Kiểm tra xem nhân viên đã có chức vụ này chưa
            if (_targetEmployee.DanhSachChucVu.Any(x => x.MaCV == selectedChucVu.MaCV))
            {
                MessageBox.Show("Nhân viên này đã nắm giữ chức vụ này rồi!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newRole = new VaiTroNhanVien
            {
                MaNV = _targetEmployee.MaNV,
                MaCV = selectedChucVu.MaCV,
                ChucVu = selectedChucVu, // Gán object để hiển thị DataGrid đẹp
                LoaiChucVu = CboLoaiVaiTro.Text,
                HeSoLuongCoBan = decimal.Parse(TxtHeSoLuong.Text),
                HeSoPhuCapKiemNhiem = decimal.Parse(TxtHeSoPhuCap.Text),
                GhiChu = TxtGhiChu.Text
            };

            // TODO: Gọi Service để lưu vào DB
             _vaiTroNhanVienService.InsertVaiTroNhanVien(newRole);
            
            // Giả lập thêm vào list
            _targetEmployee.DanhSachChucVu.Add(newRole);
            
            MessageBox.Show("Thêm vai trò thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadEmployeeRoles();
            BtnClear_Click(null, null);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgRoles.SelectedItem is not VaiTroNhanVien selectedRole) return;
            if (!ValidateInput()) return;

            // Cập nhật dữ liệu object
            selectedRole.LoaiChucVu = CboLoaiVaiTro.Text;
            selectedRole.HeSoLuongCoBan = decimal.Parse(TxtHeSoLuong.Text);
            selectedRole.HeSoPhuCapKiemNhiem = decimal.Parse(TxtHeSoPhuCap.Text);
            selectedRole.GhiChu = TxtGhiChu.Text;

            // TODO: Gọi Service để Update DB
            _vaiTroNhanVienService.UpdateVaiTroNhanVien(selectedRole);

            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadEmployeeRoles(); // Refresh Grid
            BtnClear_Click(null, null);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgRoles.SelectedItem is not VaiTroNhanVien selectedRole) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa vai trò '{selectedRole.ChucVu.TenCV}' của nhân viên này?", 
                                         "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                // TODO: Gọi Service để Xóa khỏi DB
                 _vaiTroNhanVienService.DeleteVaiTroNhanVien(selectedRole.MaNV, selectedRole.MaCV);
                 

                MessageBox.Show("Đã xóa vai trò!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadEmployeeRoles();
                BtnClear_Click(null, null);
            }
        }

        private bool ValidateInput()
        {
            if (CboChucVu.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chức vụ.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtHeSoLuong.Text, out _) || !decimal.TryParse(TxtHeSoPhuCap.Text, out _))
            {
                MessageBox.Show("Hệ số lương và phụ cấp phải là số.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}