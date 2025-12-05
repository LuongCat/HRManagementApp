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
        // TODO: Inject Service Khấu Trừ
        private KhauTruService _khauTruService;

        public DeductionManagementWindow(NhanVien employee)
        {
            InitializeComponent();
            _targetEmployee = employee;
            _khauTruService = new KhauTruService();
            _luongService = new LuongService();
            // Header Info
            TxtEmployeeName.Text = $"{_targetEmployee.HoTen} (Mã: {_targetEmployee.MaNV})";

            // Default date
            DpNgay.SelectedDate = DateTime.Now;

            LoadData();
        }

        private void LoadData()
        {
            // TODO: Load data from Service
             var list = _khauTruService.GetDeductionByMaNV(_targetEmployee.MaNV);

            // Giả lập
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
        // CRUD ACTIONS
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

            // TODO: Service Call ->
            _khauTruService.AddDeduction(newItem);
            
            // Giả lập
            _targetEmployee.KhauTrus.Add(newItem);
            
            // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
            bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, newItem.Ngay);

            if (isUnlocked)
            {
                MessageBox.Show($"thêm khấu trừ thành công! Bảng lương tháng {newItem.Ngay:MM/yyyy} đã được mở chốt để tính lại.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Chỉ thông báo thêm thành công, không nhắc gì tới lương vì chưa có bảng lương
                MessageBox.Show("thêm khấu trừ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            
            LoadData();
            BtnClear_Click(null, null);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgDeductions.SelectedItem is not KhauTru selected) return;
            if (!ValidateInput()) return;

            // Update Object
            selected.TenKhoanTru = TxtTenKhoanTru.Text.Trim();
            selected.SoTien = decimal.Parse(TxtSoTien.Text);
            selected.Ngay = DpNgay.SelectedDate.Value;
            selected.GhiChu = TxtGhiChu.Text;

            // TODO: Service Call -> _khauTruService.Update(selected);

            // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
            bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, selected.Ngay);

            if (isUnlocked)
            {
                MessageBox.Show($"sửa khấu trừ thành công! Bảng lương tháng {selected.Ngay:MM/yyyy} đã được mở chốt để tính lại.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Chỉ thông báo thêm thành công, không nhắc gì tới lương vì chưa có bảng lương
                MessageBox.Show("sửa khấu trừ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadData(); // Reload Grid
            BtnClear_Click(null, null);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgDeductions.SelectedItem is not KhauTru selected) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa khoản trừ '{selected.TenKhoanTru}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //hơi kì xíu nhưng kệ đi chạy ngon là được lười chỉnh quá
                // TODO: Service Call ->
                _khauTruService.Delete(selected.MaKT);

                // Giả lập
                _targetEmployee.KhauTrus.Remove(selected);

                // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
                bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, selected.Ngay);

                if (isUnlocked)
                {
                    MessageBox.Show($"xóa khấu trừ thành công! Bảng lương tháng {selected.Ngay:MM/yyyy} đã được mở chốt để tính lại.", 
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Chỉ thông báo thêm thành công, không nhắc gì tới lương vì chưa có bảng lương
                    MessageBox.Show("xóa khấu trừ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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