using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class DepartmentListView : UserControl
    {
        private readonly PhongBanService _phongBanService;
        private readonly VaiTroNhanVienService _vaiTroNhanVienService;
        private readonly NhanVienService _nhanVienService;
        private ObservableCollection<PhongBanViewModel> _departments;

        public DepartmentListView()
        {
            InitializeComponent();
            _phongBanService = new PhongBanService();
            _vaiTroNhanVienService = new VaiTroNhanVienService();
            _nhanVienService = new NhanVienService();
            LoadDepartments();
        }

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

                _departments = new ObservableCollection<PhongBanViewModel>(
                    phongBans.Select(pb => new PhongBanViewModel
                    {
                        MaPhongBan = pb.MaPB.ToString(), // Làm rõ kiểu string
                        TenPhongBan = pb.TenPB,
                        MoTa = pb.MoTa ?? "Chưa có mô tả",
                        TruongPhong = GetTruongPhong(pb.MaPB),
                        SoNhanVien = GetSoNhanVien(pb.MaPB)
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

        private string GetTruongPhong(int maPB)
        {
            return "Chưa có trưởng phòng";
        }

        private int GetSoNhanVien(int maPB)
        {
            return 0;
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (!int.TryParse(button?.Tag?.ToString(), out int maPB))
                return;

            var department = _departments.FirstOrDefault(d => int.Parse(d.MaPhongBan) == maPB
            );

            if (department == null)
            {
                MessageBox.Show("Không tìm thấy phòng ban!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy danh sách nhân viên
            var employees = _vaiTroNhanVienService.GetNhanVienOfPhongBan(maPB);

            var window = new DepartmentEmployeesWindow();

            window.SetDepartmentInfo(
                department.MaPhongBan,
                department.TenPhongBan,
                department.TruongPhong,
                department.MoTa
            );

            window.SetStatistics(
                total: 45, active: 42, inactive: 3
            );

            window.SetEmployees(employees);

            window.ShowDialog();
        }

        
        //chỉnh sửa phòng ban
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (!int.TryParse(button?.Tag?.ToString(), out int maPB))
                return;

            var department = _departments.FirstOrDefault(d => int.Parse(d.MaPhongBan) == maPB);
    
            if (department != null)
            {
                // Lấy thông tin phòng ban từ service
                var phongBan = _phongBanService.GetPhongBanByID(maPB);
        
                // Lấy tất cả nhân viên (để chọn trưởng phòng)
                var allEmployees = _nhanVienService.GetListNhanVien();
        
                // Lấy nhân viên thuộc phòng ban
                var departmentEmployees = _vaiTroNhanVienService.GetNhanVienOfPhongBan(maPB);
        
                // Mở window (window này trong file Department)
                var window = new EditDepartmentWindow();
                window.LoadDepartmentInfo(phongBan, departmentEmployees);
        
                if (window.ShowDialog() == true)
                {
                    LoadDepartments(); // Refresh
                }
            }
        }


        private void BtnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (!int.TryParse(button?.Tag?.ToString(), out int maPB))
                return;


            var department = _departments.FirstOrDefault(d => int.Parse(d.MaPhongBan) == maPB);
            
            if (department != null)
            {
                // Lấy danh sách nhân viên chưa có phòng ban hoặc ở phòng ban khác
                var availableEmployees = _vaiTroNhanVienService.GetEmployeesNotInDepartment(maPB);

                var window = new AddEmployeeToDepartmentWindow();
                window.SetDepartmentInfo(department.MaPhongBan, department.TenPhongBan);
                 window.LoadEmployees(availableEmployees);

                if (window.ShowDialog() == true)
                {
                    MessageBox.Show("Thêm nhân viên thành công!", "Thông báo");
                    LoadDepartments(); // Refresh
                }
            }
        }
    }

    public class PhongBanViewModel
    {
        public string MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public string TruongPhong { get; set; }
        public int SoNhanVien { get; set; }
        public string MoTa { get; set; }
    }
}