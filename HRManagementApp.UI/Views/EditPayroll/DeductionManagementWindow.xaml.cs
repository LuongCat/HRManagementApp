using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views
{
    public partial class DeductionManagementWindow : Window
    {
        private NhanVien _targetEmployee;
        private LuongService _luongService;
        private KhauTruService _khauTruService;
        private SystemLogService _logService; // 1. Khai báo Log Service

        public DeductionManagementWindow(NhanVien employee)
        {
            InitializeComponent();
            _targetEmployee = employee;
            
            _khauTruService = new KhauTruService();
            _luongService = new LuongService();
            _logService = new SystemLogService(); // 2. Khởi tạo

            // Header Info
            TxtEmployeeName.Text = $"{_targetEmployee.HoTen} (Mã: {_targetEmployee.MaNV})";

            // Default date
            DpNgay.SelectedDate = DateTime.Now;

            LoadData();
        }

        private void LoadData()
        {
            // Load data from Service
            var list = _khauTruService.GetDeductionByMaNV(_targetEmployee.MaNV);

            // Giả lập (nếu service chưa trả về list gán vào object)
            if (_targetEmployee.KhauTrus == null) 
                _targetEmployee.KhauTrus = new List<KhauTru>();

            // Sắp xếp theo ngày giảm dần để dễ nhìn cái mới nhất
            var sortedList = _targetEmployee.KhauTrus.OrderByDescending(k => k.Ngay).ToList();

            DgDeductions.ItemsSource = null;
            DgDeductions.ItemsSource = sortedList;
        }

        // ==========================
        // UI EVENTS
        // ==========================
        private void DgDeductions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgDeductions.SelectedItem is KhauTru selected)
            {
                TxtTenKhoanTru.Text = selected.TenKhoanTru;
                TxtSoTien.Text = selected.SoTien.ToString("N0");
                DpNgay.SelectedDate = selected.Ngay;
                TxtGhiChu.Text = selected.GhiChu;

                // Action Buttons State
                BtnAdd.IsEnabled = false;
                BtnEdit.IsEnabled = true;
                BtnDelete.IsEnabled = true;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            DgDeductions.SelectedItem = null;
            TxtTenKhoanTru.Text = string.Empty;
            TxtSoTien.Text = "0";
            DpNgay.SelectedDate = DateTime.Now;
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

            var newItem = new KhauTru
            {
                // MaKT = ... DB Auto
                MaNV = _targetEmployee.MaNV,
                TenKhoanTru = TxtTenKhoanTru.Text.Trim(),
                SoTien = decimal.Parse(TxtSoTien.Text),
                Ngay = DpNgay.SelectedDate.Value,
                GhiChu = TxtGhiChu.Text
            };

            // Service Call
            _khauTruService.AddDeduction(newItem);
            _targetEmployee.KhauTrus.Add(newItem); // Update UI

            // --- GHI LOG INSERT ---
            // Mẫu: Thêm khấu trừ 'Đi muộn' cho NV Nguyễn Văn A (ID: 10) - Số tiền: 50,000
            string logDesc = $"Thêm khấu trừ '{newItem.TenKhoanTru}' cho NV {_targetEmployee.HoTen} (ID: {_targetEmployee.MaNV}) - Số tiền: {newItem.SoTien:N0}";
            
            _logService.WriteLog(
                UserSession.HoTen,
                "INSERT",
                "KhauTru",
                newItem.MaNV.ToString(),
                logDesc
            );
            // ----------------------
            
            // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
            bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, newItem.Ngay);

            if (isUnlocked)
            {
                MessageBox.Show($"Thêm khấu trừ thành công! Đã mở chốt lương tháng {newItem.Ngay:MM/yyyy}.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Thêm khấu trừ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            LoadData();
            BtnClear_Click(null, null);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgDeductions.SelectedItem is not KhauTru selected) return;
            if (!ValidateInput()) return;

            // 1. SNAPSHOT: Lưu giá trị cũ TRƯỚC khi update
            string oldName = selected.TenKhoanTru;
            decimal oldMoney = selected.SoTien;
            DateTime oldDate = selected.Ngay;

            // Update Object (Gán giá trị mới)
            selected.TenKhoanTru = TxtTenKhoanTru.Text.Trim();
            selected.SoTien = decimal.Parse(TxtSoTien.Text);
            selected.Ngay = DpNgay.SelectedDate.Value;
            selected.GhiChu = TxtGhiChu.Text;

            // Service Call
            _khauTruService.UpdateDeduction(selected);

            // --- GHI LOG UPDATE ---
            string logDetail = $"Sửa khấu trừ NV {_targetEmployee.HoTen} (ID: {_targetEmployee.MaNV}): ";
            bool hasChange = false;

            if (oldName != selected.TenKhoanTru)
            {
                logDetail += $"[Lý do: {oldName} -> {selected.TenKhoanTru}] ";
                hasChange = true;
            }
            if (oldMoney != selected.SoTien)
            {
                logDetail += $"[Tiền: {oldMoney:N0} -> {selected.SoTien:N0}] ";
                hasChange = true;
            }
            if (oldDate != selected.Ngay)
            {
                logDetail += $"[Ngày: {oldDate:dd/MM} -> {selected.Ngay:dd/MM}] ";
                hasChange = true;
            }
            if (!hasChange) logDetail += "Cập nhật ghi chú.";

            _logService.WriteLog(
                UserSession.HoTen,
                "UPDATE",
                "KhauTru",
                selected.MaKT.ToString(), // Hoặc ID của khoản trừ
                logDetail
            );
            // ----------------------

            // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
            // Logic: Nếu sửa ngày sang tháng khác, cần mở chốt cả tháng cũ lẫn tháng mới
            bool isUnlockedNew = _luongService.UnLockSalary(_targetEmployee.MaNV, selected.Ngay);
            bool isUnlockedOld = false;
            
            if (oldDate.Month != selected.Ngay.Month || oldDate.Year != selected.Ngay.Year)
            {
                isUnlockedOld = _luongService.UnLockSalary(_targetEmployee.MaNV, oldDate);
            }

            if (isUnlockedNew || isUnlockedOld)
            {
                MessageBox.Show($"Sửa thành công! Đã mở chốt lương tháng {selected.Ngay:MM/yyyy} để tính lại.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Sửa khấu trừ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            LoadData(); 
            BtnClear_Click(null, null);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgDeductions.SelectedItem is not KhauTru selected) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa khoản trừ '{selected.TenKhoanTru}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Snapshot thông tin trước khi xóa
                string backupInfo = $"{selected.TenKhoanTru} - {selected.SoTien:N0} (Ngày: {selected.Ngay:dd/MM/yyyy})";

                // Service Call
                _khauTruService.Delete(selected.MaKT);
                _targetEmployee.KhauTrus.Remove(selected); // UI Remove

                // --- GHI LOG DELETE ---
                _logService.WriteLog(
                    UserSession.HoTen,
                    "DELETE",
                    "KhauTru",
                    selected.MaKT.ToString(),
                    $"Đã xóa khấu trừ của NV {_targetEmployee.HoTen}. Chi tiết: {backupInfo}"
                );
                // ----------------------

                // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
                bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, selected.Ngay);

                if (isUnlocked)
                {
                    MessageBox.Show($"Xóa thành công! Đã mở chốt lương tháng {selected.Ngay:MM/yyyy}.", 
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Xóa khấu trừ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (string.IsNullOrWhiteSpace(TxtTenKhoanTru.Text))
            {
                MessageBox.Show("Vui lòng nhập tên/lý do khoản trừ.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtSoTien.Text, out decimal soTien) || soTien <= 0)
            {
                MessageBox.Show("Số tiền phải là số lớn hơn 0.", "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DpNgay.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày ghi nhận.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}