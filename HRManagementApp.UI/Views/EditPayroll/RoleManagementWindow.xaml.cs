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
        
        // Services
        private VaiTroNhanVienService _vaiTroNhanVienService; 
        private ChucVuService _chucVuService;
        private LuongService _luongService;
        private SystemLogService _logService; // 1. Khai báo Log Service

        public RoleManagementWindow(NhanVien employee)
        {
            InitializeComponent();
            _targetEmployee = employee;
            
            _vaiTroNhanVienService = new VaiTroNhanVienService();
            _chucVuService = new ChucVuService();
            _luongService = new LuongService();
            _logService = new SystemLogService(); // 2. Khởi tạo Log Service
            
            // Hiển thị tên nhân viên lên header
            TxtEmployeeName.Text = $"{_targetEmployee.HoTen} (Mã: {_targetEmployee.MaNV})";

            LoadComboBoxData();
            LoadEmployeeRoles();
        }

        private void LoadComboBoxData()
        {
             var listChucVu = _chucVuService.GetAllChucVu();
             CboChucVu.ItemsSource = listChucVu;
        }

        private void LoadEmployeeRoles()
        {
            // 1. Lấy dữ liệu mới nhất từ Database
            var rolesFromDB = _vaiTroNhanVienService.GetVaiTroNhanVien(_targetEmployee.MaNV);

            // 2. Cập nhật lại list của nhân viên bằng list mới lấy về
            _targetEmployee.DanhSachChucVu = rolesFromDB ?? new List<VaiTroNhanVien>();

            // 3. Gán lên Grid
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
                CboLoaiVaiTro.Text = selectedRole.LoaiChucVu;
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
        // CRUD ACTIONS (CÓ GHI LOG)
        // ==========================

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var selectedChucVu = CboChucVu.SelectedItem as ChucVu;

            // Kiểm tra trùng chức vụ
            if (_targetEmployee.DanhSachChucVu.Any(x => x.MaCV == selectedChucVu.MaCV))
            {
                MessageBox.Show("Nhân viên này đã nắm giữ chức vụ này rồi!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newRole = new VaiTroNhanVien
            {
                MaNV = _targetEmployee.MaNV,
                MaCV = selectedChucVu.MaCV,
                ChucVu = selectedChucVu, 
                LoaiChucVu = CboLoaiVaiTro.Text,
                HeSoLuongCoBan = decimal.Parse(TxtHeSoLuong.Text),
                HeSoPhuCapKiemNhiem = decimal.Parse(TxtHeSoPhuCap.Text),
                GhiChu = TxtGhiChu.Text
            };

            // Service Call
            _vaiTroNhanVienService.InsertVaiTroNhanVien(newRole);
            _targetEmployee.DanhSachChucVu.Add(newRole); // Update UI

            // --- GHI LOG INSERT ---
            // Mẫu: Phân công chức vụ 'Trưởng phòng' (Chính thức) cho NV Nguyễn Văn A
            string logDesc = $"Phân công chức vụ '{selectedChucVu.TenCV}' ({newRole.LoaiChucVu}) cho NV {_targetEmployee.HoTen} (ID: {_targetEmployee.MaNV})";
            
            _logService.WriteLog(
                UserSession.HoTen,
                "INSERT",
                "VaiTroNhanVien",
                _targetEmployee.MaNV.ToString(), // RefID là Mã NV
                logDesc
            );
            // ----------------------
            
            // Mở chốt lương
            bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, DateTime.Today);

            if (isUnlocked)
            {
                MessageBox.Show($"Thêm vai trò thành công! Đã mở chốt lương tháng {DateTime.Today:MM/yyyy} để tính lại.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Thêm vai trò thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            LoadEmployeeRoles();
            BtnClear_Click(null, null);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgRoles.SelectedItem is not VaiTroNhanVien selectedRole) return;
            if (!ValidateInput()) return;

            // 1. SNAPSHOT: Lưu giá trị cũ TRƯỚC khi update
            string oldType = selectedRole.LoaiChucVu;
            decimal oldHSL = selectedRole.HeSoLuongCoBan ?? 0;
            decimal oldHSPC = selectedRole.HeSoPhuCapKiemNhiem ?? 0;

            // Cập nhật dữ liệu object
            selectedRole.LoaiChucVu = CboLoaiVaiTro.Text;
            selectedRole.HeSoLuongCoBan = decimal.Parse(TxtHeSoLuong.Text);
            selectedRole.HeSoPhuCapKiemNhiem = decimal.Parse(TxtHeSoPhuCap.Text);
            selectedRole.GhiChu = TxtGhiChu.Text;

            // Service Call
            _vaiTroNhanVienService.UpdateVaiTroNhanVien(selectedRole);

            // --- GHI LOG UPDATE ---
            // Mẫu: Sửa vai trò 'Trưởng phòng' của NV A: [Loại: Thử việc -> Chính thức] [HS Lương: 2.34 -> 3.00]
            string roleName = selectedRole.ChucVu != null ? selectedRole.ChucVu.TenCV : selectedRole.MaCV.ToString();
            string logDetail = $"Sửa vai trò '{roleName}' của NV {_targetEmployee.HoTen}: ";
            bool hasChange = false;

            if (oldType != selectedRole.LoaiChucVu)
            {
                logDetail += $"[Loại: {oldType} -> {selectedRole.LoaiChucVu}] ";
                hasChange = true;
            }
            if (oldHSL != selectedRole.HeSoLuongCoBan)
            {
                logDetail += $"[HS Lương: {oldHSL:N2} -> {selectedRole.HeSoLuongCoBan:N2}] ";
                hasChange = true;
            }
            if (oldHSPC != selectedRole.HeSoPhuCapKiemNhiem)
            {
                logDetail += $"[HS Phụ Cấp: {oldHSPC:N2} -> {selectedRole.HeSoPhuCapKiemNhiem:N2}] ";
                hasChange = true;
            }
            if (!hasChange) logDetail += "Cập nhật ghi chú.";

            _logService.WriteLog(
                UserSession.HoTen,
                "UPDATE",
                "VaiTroNhanVien",
                _targetEmployee.MaNV.ToString(),
                logDetail
            );
            // ----------------------

            // Mở chốt lương
            bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, DateTime.Today);

            if (isUnlocked)
            {
                MessageBox.Show($"Sửa thành công! Đã mở chốt lương tháng {DateTime.Today:MM/yyyy}.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Sửa vai trò thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            LoadEmployeeRoles();
            BtnClear_Click(null, null);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgRoles.SelectedItem is not VaiTroNhanVien selectedRole) return;
            string roleName = selectedRole.ChucVu != null ? selectedRole.ChucVu.TenCV : selectedRole.MaCV.ToString();

            var result = MessageBox.Show($"Bạn có chắc muốn xóa vai trò '{roleName}' của nhân viên này?", 
                                         "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                // Service Call
                _vaiTroNhanVienService.DeleteVaiTroNhanVien(selectedRole.MaNV, selectedRole.MaCV);
                
                // --- GHI LOG DELETE ---
                string logDesc = $"Đã xóa vai trò '{roleName}' của NV {_targetEmployee.HoTen} (ID: {_targetEmployee.MaNV})";
                
                _logService.WriteLog(
                    UserSession.HoTen,
                    "DELETE",
                    "VaiTroNhanVien",
                    _targetEmployee.MaNV.ToString(),
                    logDesc
                );
                // ----------------------

                 // Mở chốt lương
                 bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, DateTime.Today);

                 if (isUnlocked)
                 {
                     MessageBox.Show($"Xóa thành công! Đã mở chốt lương tháng {DateTime.Today:MM/yyyy}.", 
                         "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                 }
                 else
                 {
                     MessageBox.Show("Xóa vai trò thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                 }
                 
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