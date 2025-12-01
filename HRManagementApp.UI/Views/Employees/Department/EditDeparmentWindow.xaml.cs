using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class EditDepartmentWindow : Window
    {
        private int _maPB;
        private string _tenPB;
        private int maTruongPhong;
        private List<NhanVien> _employees;
        private readonly PhongBanService _phongBanService;
        private readonly NhanVienService _nhanVienService;
        private readonly VaiTroNhanVienService _vaiTroNhanVienService;
        private NhanVien TruongPhong;

        public EditDepartmentWindow()
        {
            InitializeComponent();
            _phongBanService = new PhongBanService();
            _nhanVienService = new NhanVienService();
            _vaiTroNhanVienService = new VaiTroNhanVienService();
        }

        /// <summary>
        /// Load thông tin phòng ban
        /// </summary>
        public void LoadDepartmentInfo(PhongBan phongBan, List<NhanVien> departmentEmployees)
        {
            _maPB = phongBan.MaPB;
            TruongPhong = _phongBanService.GetDeparmentHead(phongBan.MaPB);
            _employees = departmentEmployees;

            // Set thông tin cơ bản
            TxtDepartmentName.Text = phongBan.TenPB;
            TxtTenPB.Text = phongBan.TenPB;
            TxtMoTa.Text = phongBan.MoTa ?? "";
            if (TruongPhong != null)
                TxtTruongPhong.Text = TruongPhong.HoTen;
            else
                TxtTruongPhong.Text = "Chưa có trưởng phòng";


            // Load danh sách nhân viên
            EmployeesDataGrid.ItemsSource = _employees;
        }

        /// <summary>
        /// Xóa nhân viên khỏi phòng ban
        /// </summary>
        private void BtnRemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (!int.TryParse(button?.Tag?.ToString(), out int maNV))
                return;

            var employee = _employees.FirstOrDefault(e => e.MaNV == maNV);
            if (employee == null) return;

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa nhân viên {employee.HoTen} khỏi phòng ban?\n\n" +
                $"Nhân viên sẽ không còn thuộc phòng ban này nữa.",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // TODO: Gọi service để cập nhật
                 _vaiTroNhanVienService.RemoveEmployeeFromDepartment(employee.MaNV);

                _employees.Remove(employee);
                EmployeesDataGrid.Items.Refresh();

                MessageBox.Show("Đã xóa nhân viên khỏi phòng ban!", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Lưu thay đổi
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate tên phòng ban
            if (string.IsNullOrWhiteSpace(TxtTenPB.Text))
            {
                MessageBox.Show("Vui lòng nhập tên phòng ban!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTenPB.Focus();
                return;
            }

            //  Validate trưởng phòng
            if (string.IsNullOrWhiteSpace(TxtTruongPhong.Text))
            {
                MessageBox.Show("Vui lòng chọn trưởng phòng!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTruongPhong.Focus();
                return;
            }

            //  Lấy thông tin nhân viên trưởng phòng từ service
            var truongPhong = _nhanVienService.GetEmployeeByName(TxtTruongPhong.Text.Trim());
            if (truongPhong == null)
            {
                MessageBox.Show("Nhân viên trưởng phòng không tồn tại!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo object cập nhật
            var updatedPhongBan = new PhongBan
            {
                MaPB = _maPB, // ID phòng ban hiện tại
                TenPB = TxtTenPB.Text.Trim(),
                MoTa = string.IsNullOrWhiteSpace(TxtMoTa.Text) ? null : TxtMoTa.Text.Trim(),
                MaTruongPhong = truongPhong.MaNV
            };

            // Gọi repository để cập nhật
            bool success = _phongBanService.UpdateDeparment(updatedPhongBan);
            
            if (success)
            {
                MessageBox.Show("Cập nhật phòng ban thành công!", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Cập nhật phòng ban thất bại!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}