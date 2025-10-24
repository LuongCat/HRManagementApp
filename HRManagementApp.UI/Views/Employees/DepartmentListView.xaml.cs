using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    /// <summary>
    /// View hiển thị danh sách phòng ban dạng card
    /// </summary>
    public partial class DepartmentListView : UserControl
    {
        private readonly PhongBanService _phongBanService;
        private ObservableCollection<PhongBanViewModel> _departments;

        public DepartmentListView()
        {
            InitializeComponent();
            _phongBanService = new PhongBanService();
            LoadDepartments();
        }

        /// <summary>
        /// Load danh sách phòng ban từ database
        /// </summary>
        private void LoadDepartments()
        {
            try
            {
                var phongBans = _phongBanService.GetListPhongBan();

                if (phongBans == null || phongBans.Count == 0)
                {
                    DepartmentItemsControl.ItemsSource = null;
                    MessageBox.Show("Không có dữ liệu phòng ban!", "Thông báo", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Chuyển đổi sang ViewModel để hiển thị
                _departments = new ObservableCollection<PhongBanViewModel>(
                    phongBans.Select(pb => new PhongBanViewModel
                    {
                        MaPhongBan = pb.MaPB.ToString(),
                        TenPhongBan = pb.TenPB,
                        MoTa = pb.MoTa ?? "Chưa có mô tả",
                        TruongPhong = GetTruongPhong(pb.MaPB), // TODO: Lấy từ database
                        SoNhanVien = GetSoNhanVien(pb.MaPB)    // TODO: Lấy từ database
                    })
                );

                DepartmentItemsControl.ItemsSource = _departments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách phòng ban: {ex.Message}", 
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lấy tên trưởng phòng (TODO: Query từ database)
        /// </summary>
        private string GetTruongPhong(int maPB)
        {
            // TODO: Implement query để lấy trưởng phòng từ bảng NhanVien
            // WHERE MaPB = maPB AND ChucVu = 'Trưởng phòng'
            // Tạm thời return giá trị mặc định
            return "Chưa có trưởng phòng";
        }

        /// <summary>
        /// Đếm số nhân viên trong phòng ban (TODO: Query từ database)
        /// </summary>
        private int GetSoNhanVien(int maPB)
        {
            // TODO: Implement query COUNT nhân viên
            // SELECT COUNT(*) FROM NhanVien WHERE MaPB = maPB
            // Tạm thời return 0
            return 0;
        }

        /// <summary>
        /// Xem danh sách nhân viên trong phòng ban
        /// </summary>
        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string maPhongBan = button?.Tag?.ToString();
            
            if (!string.IsNullOrEmpty(maPhongBan))
            {
                var department = _departments.FirstOrDefault(d => d.MaPhongBan == maPhongBan);
                if (department != null)
                {
                    // TODO: Mở window/dialog hiển thị danh sách nhân viên trong phòng ban
                    MessageBox.Show(
                        $"Xem danh sách nhân viên phòng ban: {department.TenPhongBan}\n\n" +
                        $"Mã phòng ban: {department.MaPhongBan}\n" +
                        $"Trưởng phòng: {department.TruongPhong}\n" +
                        $"Số nhân viên: {department.SoNhanVien} người\n\n" +
                        $"Mô tả: {department.MoTa}",
                        "Thông tin phòng ban", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Chỉnh sửa thông tin phòng ban
        /// </summary>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string maPhongBan = button?.Tag?.ToString();
            
            if (!string.IsNullOrEmpty(maPhongBan))
            {
                var department = _departments.FirstOrDefault(d => d.MaPhongBan == maPhongBan);
                if (department != null)
                {
                    // TODO: Mở window/dialog chỉnh sửa thông tin phòng ban
                    MessageBox.Show(
                        $"Chỉnh sửa phòng ban: {department.TenPhongBan}\n\n" +
                        $"Mã phòng ban: {department.MaPhongBan}",
                        "Chỉnh sửa", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                    
                    // Sau khi sửa xong, gọi LoadDepartments() để refresh
                    // LoadDepartments();
                }
            }
        }
    }

    /// <summary>
    /// ViewModel cho hiển thị phòng ban với thông tin mở rộng
    /// </summary>
    public class PhongBanViewModel
    {
        public string MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public string TruongPhong { get; set; }
        public int SoNhanVien { get; set; }
        public string MoTa { get; set; }
    }
}