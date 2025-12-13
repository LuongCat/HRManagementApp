using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views
{
    public partial class TaxManagementWindow : Window
    {
        private NhanVien _targetEmployee;
        
        // Services
        private ThueService _thueService;
        private LuongService _luongService;     // Service xử lý lương
        private SystemLogService _logService;   // Service ghi log

        public TaxManagementWindow(NhanVien employee)
        {
            InitializeComponent();
            _targetEmployee = employee;
            
            _thueService = new ThueService();
            _luongService = new LuongService();     // Khởi tạo
            _logService = new SystemLogService();   // Khởi tạo
            
            // Header Info
            TxtEmployeeName.Text = $"{_targetEmployee.HoTen} (Mã: {_targetEmployee.MaNV})";

            // Default date
            DpTuNgay.SelectedDate = DateTime.Now;

            LoadData();
        }

        private void LoadData()
        {
            // Load data from Service
            var list = _thueService.GetTaxByMaNV(_targetEmployee.MaNV);

            // Giả lập loading (nếu service chưa trả về list gán vào object)
            if (_targetEmployee.Thues == null) 
                _targetEmployee.Thues = new List<Thue>();

            DgTaxes.ItemsSource = null;
            DgTaxes.ItemsSource = _targetEmployee.Thues;
        }

        // ==========================
        // UI EVENTS
        // ==========================
        private void DgTaxes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgTaxes.SelectedItem is Thue selected)
            {
                TxtTenThue.Text = selected.TenThue;
                TxtSoTien.Text = selected.SoTien.ToString("N0");
                DpTuNgay.SelectedDate = selected.ApDungTuNgay;
                DpDenNgay.SelectedDate = selected.ApDungDenNgay;

                // Action Buttons State
                BtnAdd.IsEnabled = false;
                BtnEdit.IsEnabled = true;
                BtnDelete.IsEnabled = true;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            DgTaxes.SelectedItem = null;
            TxtTenThue.Text = string.Empty;
            TxtSoTien.Text = "0";
            DpTuNgay.SelectedDate = DateTime.Now;
            DpDenNgay.SelectedDate = null;

            BtnAdd.IsEnabled = true;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        // ==========================
        // CRUD ACTIONS (CÓ GHI LOG & MỞ CHỐT LƯƠNG)
        // ==========================
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var newItem = new Thue
            {
                // MaThue = ... DB Auto,
                MaNV = _targetEmployee.MaNV,
                TenThue = TxtTenThue.Text.Trim(),
                SoTien = decimal.Parse(TxtSoTien.Text),
                ApDungTuNgay = DpTuNgay.SelectedDate.Value,
                ApDungDenNgay = DpDenNgay.SelectedDate
            };

            // Service Call
            _thueService.AddTax(newItem);
            _targetEmployee.Thues.Add(newItem); // Update UI

            // --- GHI LOG INSERT ---
            // Mẫu: Thêm thuế 'TNCN' cho NV Nguyễn Văn A (ID: 10) - Số tiền: 500,000
            string logDesc = $"Thêm thuế '{newItem.TenThue}' cho NV {_targetEmployee.HoTen} (ID: {_targetEmployee.MaNV}) - Số tiền: {newItem.SoTien:N0}";
            
            _logService.WriteLog(
                UserSession.HoTen,
                "INSERT",
                "Thue",
                _targetEmployee.MaNV.ToString(),
                logDesc
            );
            // ----------------------

            // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
            bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, newItem.ApDungTuNgay);

            if (isUnlocked)
            {
                MessageBox.Show($"Thêm thuế thành công! Đã mở chốt lương tháng {newItem.ApDungTuNgay:MM/yyyy} để tính lại.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Thêm thông tin thuế thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            LoadData();
            BtnClear_Click(null, null);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgTaxes.SelectedItem is not Thue selected) return;
            if (!ValidateInput()) return;

            // 1. SNAPSHOT: Lưu giá trị cũ TRƯỚC khi update
            string oldName = selected.TenThue;
            decimal oldMoney = selected.SoTien;
            DateTime oldDate = selected.ApDungTuNgay;

            // Update Object
            selected.TenThue = TxtTenThue.Text.Trim();
            selected.SoTien = decimal.Parse(TxtSoTien.Text);
            selected.ApDungTuNgay = DpTuNgay.SelectedDate.Value;
            selected.ApDungDenNgay = DpDenNgay.SelectedDate;

            // Service Call
            _thueService.UpdateTax(selected);

            // --- GHI LOG UPDATE ---
            // Mẫu: Sửa thuế NV Nguyễn Văn A: [Tên: TNCN -> Giảm trừ] [Tiền: 500k -> 1tr]
            string logDetail = $"Sửa thuế NV {_targetEmployee.HoTen} (ID: {_targetEmployee.MaNV}): ";
            bool hasChange = false;

            if (oldName != selected.TenThue)
            {
                logDetail += $"[Tên: {oldName} -> {selected.TenThue}] ";
                hasChange = true;
            }
            if (oldMoney != selected.SoTien)
            {
                logDetail += $"[Tiền: {oldMoney:N0} -> {selected.SoTien:N0}] ";
                hasChange = true;
            }
            if (oldDate != selected.ApDungTuNgay)
            {
                logDetail += $"[Ngày: {oldDate:dd/MM} -> {selected.ApDungTuNgay:dd/MM}] ";
                hasChange = true;
            }
            if (!hasChange) logDetail += "Cập nhật ngày kết thúc.";

            _logService.WriteLog(
                UserSession.HoTen,
                "UPDATE",
                "Thue",
                selected.MaThue.ToString(),
                logDetail
            );
            // ----------------------

            // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
            bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, selected.ApDungTuNgay);
            // Nếu đổi ngày áp dụng sang tháng khác, mở luôn cả tháng cũ
            if (oldDate.Month != selected.ApDungTuNgay.Month)
            {
                 _luongService.UnLockSalary(_targetEmployee.MaNV, oldDate);
            }

            if (isUnlocked)
            {
                MessageBox.Show($"Cập nhật thành công! Đã mở chốt lương tháng {selected.ApDungTuNgay:MM/yyyy}.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            LoadData(); 
            BtnClear_Click(null, null);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgTaxes.SelectedItem is not Thue selected) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa khoản thuế '{selected.TenThue}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Snapshot thông tin backup
                string backupInfo = $"{selected.TenThue} - {selected.SoTien:N0} (Từ: {selected.ApDungTuNgay:dd/MM/yyyy})";

                // Service Call
                _thueService.Delete(selected.MaThue);
                _targetEmployee.Thues.Remove(selected);

                // --- GHI LOG DELETE ---
                _logService.WriteLog(
                    UserSession.HoTen,
                    "DELETE",
                    "Thue",
                    selected.MaThue.ToString(),
                    $"Đã xóa khoản thuế của NV {_targetEmployee.HoTen}. Chi tiết: {backupInfo}"
                );
                // ----------------------

                // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
                bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, selected.ApDungTuNgay);

                if (isUnlocked)
                {
                    MessageBox.Show($"Đã xóa khoản thuế! Đã mở chốt lương tháng {selected.ApDungTuNgay:MM/yyyy}.", 
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Đã xóa khoản thuế!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                LoadData();
                BtnClear_Click(null, null);
            }
        }

        // ==========================
        // VALIDATION
        // ==========================
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TxtTenThue.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại thuế.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtSoTien.Text, out decimal soTien) || soTien <= 0)
            {
                MessageBox.Show("Số tiền phải là số lớn hơn 0.", "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DpTuNgay.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày áp dụng.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (DpDenNgay.SelectedDate != null && DpDenNgay.SelectedDate < DpTuNgay.SelectedDate)
            {
                MessageBox.Show("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.", "Lỗi logic", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}