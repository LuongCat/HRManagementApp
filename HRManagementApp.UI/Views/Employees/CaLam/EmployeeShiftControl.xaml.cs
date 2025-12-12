using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class EmployeeShiftControl : UserControl
    {
        private NhanVien _currentNhanVien;
        private readonly CaLamService _caLamService;
        private List<CaLam> _allShifts; // Danh sách tất cả ca để đổ vào ComboBox

        public EmployeeShiftControl()
        {
            InitializeComponent();
            _caLamService = new CaLamService();
        }

        /// <summary>
        /// Hàm này được gọi từ Form Cha để truyền Nhân Viên vào
        /// </summary>
        public void LoadDataForEmployee(NhanVien nv)
        {
            if (nv == null) return;
            
            _currentNhanVien = nv;
            TxtTenNhanVien.Text = $"{nv.HoTen} (MNV: {nv.MaNV})";

            // 1. Tải danh sách tất cả các ca có sẵn để user chọn thêm
            LoadAvailableShifts();

            // 2. Tải danh sách ca mà nhân viên này đang làm
            LoadAssignedShifts();
        }

        private void LoadAvailableShifts()
        {
            try
            {
                // Lấy tất cả ca hoạt động
                _allShifts = _caLamService.GetAllCaLam();
                CboAvailableShifts.ItemsSource = _allShifts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách ca: " + ex.Message);
            }
        }

        private void LoadAssignedShifts()
        {
            try
            {
                if (_currentNhanVien == null) return;

                // Gọi Service lấy list ca của nhân viên từ DB
                var assignedShifts = _caLamService.GetCaLamByNhanVien(_currentNhanVien.MaNV);
                
                // Cập nhật lại List trong Object NhanVien (để đồng bộ nếu cần dùng ở nơi khác)
                _currentNhanVien.CaLams = assignedShifts;

                // Đổ dữ liệu lên lưới
                DgvAssignedShifts.ItemsSource = assignedShifts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải ca làm nhân viên: " + ex.Message);
            }
        }

        // --- XỬ LÝ THÊM CA ---
        private void BtnAddShift_Click(object sender, RoutedEventArgs e)
        {
            if (_currentNhanVien == null) return;

            var selectedCa = CboAvailableShifts.SelectedItem as CaLam;
            if (selectedCa == null)
            {
                MessageBox.Show("Vui lòng chọn một ca làm việc!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra xem nhân viên đã có ca này chưa (Check trên giao diện cho nhanh)
            var currentList = DgvAssignedShifts.ItemsSource as List<CaLam>;
            if (currentList != null && currentList.Any(c => c.MaCa == selectedCa.MaCa))
            {
                MessageBox.Show($"Nhân viên {_currentNhanVien.HoTen} đã có ca '{selectedCa.TenCa}' rồi!", "Trùng lặp", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Gọi Service gán xuống DB
                string result = _caLamService.PhanCongCaLam(_currentNhanVien.MaNV, selectedCa.MaCa);
                
                if (result.Contains("thành công"))
                {
                    MessageBox.Show("Đã thêm ca làm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAssignedShifts(); // Refresh lại lưới
                }
                else
                {
                    MessageBox.Show(result, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message);
            }
        }

        // --- XỬ LÝ GỠ BỎ CA (Nút xóa trên từng dòng) ---
        private void BtnRemoveShift_Click(object sender, RoutedEventArgs e)
        {
            if (_currentNhanVien == null) return;

            // Lấy nút vừa bấm
            Button btn = sender as Button;
            if (btn?.Tag == null) return;

            int maCaCanXoa = (int)btn.Tag;
            
            // Tìm thông tin ca để hiển thị confirm cho rõ ràng
            var caToDelete = _currentNhanVien.CaLams.FirstOrDefault(c => c.MaCa == maCaCanXoa);
            string tenCa = caToDelete?.TenCa ?? "này";

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn gỡ bỏ ca '{tenCa}' khỏi nhân viên này?", 
                "Xác nhận", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _caLamService.HuyPhanCongCaLam(_currentNhanVien.MaNV, maCaCanXoa);
                    if (success)
                    {
                        MessageBox.Show("Đã gỡ bỏ ca làm việc.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAssignedShifts(); // Refresh lại lưới
                    }
                    else
                    {
                        MessageBox.Show("Gỡ bỏ thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message);
                }
            }
        }
    }
}