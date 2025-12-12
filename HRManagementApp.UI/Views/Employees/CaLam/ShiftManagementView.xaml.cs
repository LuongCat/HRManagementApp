using System;
using System.Collections.Generic;
using System.ComponentModel; // Cần thiết cho ICollectionView
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data; // Cần thiết cho CollectionViewSource
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class ShiftManagementView : UserControl
    {
        private readonly NhanVienService _nhanVienService;
        private readonly CaLamService _caLamService;
        
        // Biến lưu trữ dữ liệu gốc và View để lọc
        private List<NhanVien> _originalList;
        private ICollectionView _filteredView;

        public ShiftManagementView()
        {
            InitializeComponent();
            _nhanVienService = new NhanVienService();
            _caLamService = new CaLamService();

            Loaded += ShiftManagementView_Loaded;
        }

        private void ShiftManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // 1. Lấy dữ liệu từ DB
                _originalList = _nhanVienService.GetListNhanVien();

                if (_originalList == null) return;

                // 2. Load thông tin ca làm
                foreach (var nv in _originalList)
                {
                    nv.CaLams = _caLamService.GetCaLamByNhanVien(nv.MaNV);
                }

                // 3. Tạo CollectionView từ List gốc để hỗ trợ Filter
                _filteredView = CollectionViewSource.GetDefaultView(_originalList);
                
                // Gán logic lọc cho View
                _filteredView.Filter = FilterEmployees;

                // Gán View vào DataGrid thay vì List gốc
                DgvShiftAssignment.ItemsSource = _filteredView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- LOGIC TÌM KIẾM ---
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Mỗi khi gõ chữ, hàm này chạy và gọi Refresh của View
            // Refresh sẽ kích hoạt hàm FilterEmployees bên dưới
            if (_filteredView != null)
            {
                _filteredView.Refresh();
            }
        }

        private bool FilterEmployees(object item)
        {
            if (item is NhanVien nv)
            {
                string searchText = TxtSearch.Text;

                // Nếu ô tìm kiếm rỗng thì hiện tất cả
                if (string.IsNullOrWhiteSpace(searchText))
                    return true;

                // Kiểm tra tên nhân viên có chứa từ khóa không (Không phân biệt hoa thường)
                return nv.HoTen.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                       nv.MaNV.ToString().Contains(searchText); // Tìm theo cả Mã NV
            }
            return false;
        }

        // --- XỬ LÝ PHÂN CA ---
        private void BtnManageShift_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var selectedNV = btn?.Tag as NhanVien;

            if (selectedNV != null)
            {
                ShiftManagementDialog dialog = new ShiftManagementDialog(selectedNV);
                
                // Nếu người dùng đóng popup, ta cần refresh lại dòng đó để hiện ca mới
                dialog.ShowDialog();

                // Load lại dữ liệu để cập nhật cột "Ca Làm" mới nhất từ DB
                LoadData();
                
                // Giữ lại từ khóa tìm kiếm sau khi load lại (UX tốt hơn)
                TxtSearch_TextChanged(null, null);
            }
        }
    }
}