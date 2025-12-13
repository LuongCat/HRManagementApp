using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views
{
    public partial class AllowanceManagementWindow : Window
    {
        private NhanVien _targetEmployee;
        
        // Service
        private PhuCapService _phuCapService;
        private LuongService _luongService;
        private SystemLogService _logService; // 1. Khai báo Service Log

        public AllowanceManagementWindow(NhanVien employee)
        {
            InitializeComponent();
            _targetEmployee = employee;
            
            _phuCapService = new PhuCapService();
            _luongService = new LuongService();
            _logService = new SystemLogService(); // 2. Khởi tạo Service Log

            // Hiển thị info
            TxtEmployeeName.Text = $"{_targetEmployee.HoTen} (Mã: {_targetEmployee.MaNV})";

            // Set mặc định ngày bắt đầu là hôm nay
            DpTuNgay.SelectedDate = DateTime.Now;

            LoadData();
        }

        private void LoadData()
        {
            // TODO: Gọi Service lấy danh sách từ DB
            // var list = _phuCapService.GetByEmployee(_targetEmployee.MaNV);

            // Giả lập load từ object truyền vào
            if (_targetEmployee.PhuCaps == null) 
                _targetEmployee.PhuCaps = new List<PhuCapNhanVien>();

            DgAllowances.ItemsSource = null;
            DgAllowances.ItemsSource = _targetEmployee.PhuCaps;
        }

        // ==========================
        // UI EVENTS
        // ==========================
        private void DgAllowances_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgAllowances.SelectedItem is PhuCapNhanVien selected)
            {
                TxtTenPhuCap.Text = selected.TenPhuCap;
                TxtSoTien.Text = selected.SoTien.ToString("N0");
                DpTuNgay.SelectedDate = selected.ApDungTuNgay;
                DpDenNgay.SelectedDate = selected.ApDungDenNgay;

                // Chế độ Edit
                BtnAdd.IsEnabled = false;
                BtnEdit.IsEnabled = true;
                BtnDelete.IsEnabled = true;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            DgAllowances.SelectedItem = null;
            TxtTenPhuCap.Text = string.Empty;
            TxtSoTien.Text = "0";
            DpTuNgay.SelectedDate = DateTime.Now;
            DpDenNgay.SelectedDate = null;

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

            var newItem = new PhuCapNhanVien
            {
                MaNV = _targetEmployee.MaNV,
                TenPhuCap = TxtTenPhuCap.Text.Trim(),
                SoTien = decimal.Parse(TxtSoTien.Text),
                ApDungTuNgay = DpTuNgay.SelectedDate.Value,
                ApDungDenNgay = DpDenNgay.SelectedDate
            };

            // Service Call
            _phuCapService.AddPhuCap(newItem);
            _targetEmployee.PhuCaps.Add(newItem); // Update UI

            // --- GHI LOG INSERT (CẬP NHẬT MÔ TẢ) ---
            // Mẫu: "Thêm phụ cấp 'Xăng xe' cho NV Nguyễn Văn A (ID: 10) - Số tiền: 500,000"
            string moTaLog = $"Thêm phụ cấp '{newItem.TenPhuCap}' cho NV {_targetEmployee.HoTen} (ID: {_targetEmployee.MaNV}) - Số tiền: {newItem.SoTien:N0}";

            _logService.WriteLog(
                UserSession.HoTen,       // Người thực hiện (HR)
                "INSERT", 
                "PhuCapNhanVien", 
                newItem.MaNV.ToString(), 
                moTaLog                  // <--- Đã thêm tên nhân viên vào đây
            );
            // ----------------------------------------
    
            // Mở chốt lương...
            bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, newItem.ApDungTuNgay);
            if (isUnlocked)
                MessageBox.Show($"Thêm thành công! Đã mở chốt lương tháng {newItem.ApDungTuNgay:MM/yyyy}.", "Thông báo");
            else
                MessageBox.Show("Thêm phụ cấp thành công!", "Thông báo");
    
            LoadData();
            BtnClear_Click(null, null);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgAllowances.SelectedItem is not PhuCapNhanVien selected) return;
            if (!ValidateInput()) return;
    
            // Snapshot giá trị cũ
            string oldName = selected.TenPhuCap;
            decimal oldMoney = selected.SoTien;
    
            // Update Value
            selected.TenPhuCap = TxtTenPhuCap.Text.Trim();
            selected.SoTien = decimal.Parse(TxtSoTien.Text);
            selected.ApDungTuNgay = DpTuNgay.SelectedDate.Value;
            selected.ApDungDenNgay = DpDenNgay.SelectedDate;

            // Service Call
            _phuCapService.UpdatePhuCap(selected);

            // --- GHI LOG UPDATE (CẬP NHẬT MÔ TẢ) ---
            // Mẫu: "Sửa phụ cấp của NV Nguyễn Văn A (ID: 10): [Tiền: 500,000 -> 600,000]"
            string logDetail = $"Sửa phụ cấp của NV {_targetEmployee.HoTen} (ID: {_targetEmployee.MaNV}): ";
            bool hasChange = false;

            if (oldName != selected.TenPhuCap) 
            {
                logDetail += $"[Tên: {oldName} -> {selected.TenPhuCap}] ";
                hasChange = true;
            }
            if (oldMoney != selected.SoTien) 
            {
                logDetail += $"[Tiền: {oldMoney:N0} -> {selected.SoTien:N0}] ";
                hasChange = true;
            }
            if (!hasChange) logDetail += "Cập nhật ngày áp dụng.";

            _logService.WriteLog(
                UserSession.HoTen,
                "UPDATE",
                "PhuCapNhanVien",
                selected.ID.ToString(),
                logDetail // <--- Đã có tên nhân viên
            );
            // ----------------------------------------
    
            // Mở chốt lương...
            _luongService.UnLockSalary(_targetEmployee.MaNV, selected.ApDungTuNgay);
            MessageBox.Show("Cập nhật thành công!", "Thông báo");
    
            LoadData();
            BtnClear_Click(null, null);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgAllowances.SelectedItem is not PhuCapNhanVien selected) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa phụ cấp '{selected.TenPhuCap}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Snapshot
                string backupInfo = $"{selected.TenPhuCap} - {selected.SoTien:N0}";

                // Service Call
                _phuCapService.DeletePhuCap(selected.ID);
                _targetEmployee.PhuCaps.Remove(selected);

                // --- GHI LOG DELETE (CẬP NHẬT MÔ TẢ) ---
                string logDesc = $"Đã xóa phụ cấp của NV {_targetEmployee.HoTen} (ID: {_targetEmployee.MaNV}). Chi tiết: {backupInfo}";

                _logService.WriteLog(
                    UserSession.HoTen,
                    "DELETE",
                    "PhuCapNhanVien",
                    selected.ID.ToString(),
                    logDesc // <--- Đã có tên nhân viên
                );
                // ----------------------------------------

                // Mở chốt lương...
                _luongService.UnLockSalary(_targetEmployee.MaNV, selected.ApDungTuNgay);
                MessageBox.Show("Xóa thành công!", "Thông báo");

                LoadData();
                BtnClear_Click(null, null);
            }
        }

        // ==========================
        // VALIDATION
        // ==========================
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TxtTenPhuCap.Text))
            {
                MessageBox.Show("Vui lòng nhập tên phụ cấp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtSoTien.Text, out decimal soTien) || soTien <= 0)
            {
                MessageBox.Show("Số tiền phải lớn hơn 0.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DpTuNgay.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DpDenNgay.SelectedDate != null && DpDenNgay.SelectedDate < DpTuNgay.SelectedDate)
            {
                MessageBox.Show("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}