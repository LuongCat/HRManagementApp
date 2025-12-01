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
        
        // TODO: Inject TaxService
         private ThueService _thueService;

        public TaxManagementWindow(NhanVien employee)
        {
            InitializeComponent();
            _targetEmployee = employee;
            _thueService = new ThueService();
            // Header Info
            TxtEmployeeName.Text = $"{_targetEmployee.HoTen} (Mã: {_targetEmployee.MaNV})";

            // Default date
            DpTuNgay.SelectedDate = DateTime.Now;

            LoadData();
        }

        private void LoadData()
        {
            // TODO: Load data from Service
             var list = _thueService.GetTaxByMaNV(_targetEmployee.MaNV);

            // Giả lập
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
        // CRUD ACTIONS
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

            // TODO: Service Call ->
            _thueService.AddTax(newItem);
            
            // Giả lập
            _targetEmployee.Thues.Add(newItem);

            MessageBox.Show("Thêm thông tin thuế thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData();
            BtnClear_Click(null, null);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgTaxes.SelectedItem is not Thue selected) return;
            if (!ValidateInput()) return;

            // Update Object
            selected.TenThue = TxtTenThue.Text.Trim();
            selected.SoTien = decimal.Parse(TxtSoTien.Text);
            selected.ApDungTuNgay = DpTuNgay.SelectedDate.Value;
            selected.ApDungDenNgay = DpDenNgay.SelectedDate;

            // TODO: Service Call ->
            _thueService.UpdateTax(selected);

            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData(); // Reload Grid
            BtnClear_Click(null, null);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgTaxes.SelectedItem is not Thue selected) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa khoản thuế '{selected.TenThue}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // TODO: Service Call ->
                _thueService.Delete(selected.MaThue);

                // Giả lập
                _targetEmployee.Thues.Remove(selected);

                MessageBox.Show("Đã xóa khoản thuế!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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