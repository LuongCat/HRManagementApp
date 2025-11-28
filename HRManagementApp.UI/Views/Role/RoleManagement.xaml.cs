using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class RoleManagementView : UserControl
    {
        private RoleBLL bll = new RoleBLL();
        private List<RoleModel> allRoles = new List<RoleModel>();

        public RoleManagementView()
        {
            InitializeComponent();
            try
            {
                LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi tải trang: " + ex.Message);
            }
        }

        private void LoadData()
        {
            allRoles = bll.GetRoles();
            dgRoles.ItemsSource = allRoles;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.ToLower().Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                dgRoles.ItemsSource = allRoles;
            }
            else
            {
                dgRoles.ItemsSource = allRoles.Where(x => x.TenVaiTro.ToLower().Contains(keyword)).ToList();
            }
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnAddRole_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddRoleWindow();
            // Đặt Owner để cửa sổ con hiện giữa cửa sổ cha
            win.Owner = Window.GetWindow(this);
            if (win.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnEditPermission_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var role = btn.CommandParameter as RoleModel;

            if (role != null)
            {
                var win = new EditPermissionWindow(role);
                win.Owner = Window.GetWindow(this);
                win.ShowDialog();
            }
        }
        // Thêm các hàm này vào RoleManagementView.xaml.cs

        // 1. Xử lý nút SỬA
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var role = btn.CommandParameter as RoleModel;

            if (role != null)
            {
                // Sử dụng EditRoleWindow riêng biệt
                var win = new EditRoleWindow(role);
                win.Owner = Window.GetWindow(this);

                if (win.ShowDialog() == true)
                {
                    LoadData(); // Load lại danh sách sau khi sửa xong
                }
            }
        }

        // 2. Xử lý nút XÓA
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var role = btn.CommandParameter as RoleModel;

            if (role != null)
            {
                // Hỏi xác nhận trước khi xóa
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa vai trò '{role.TenVaiTro}' không?",
                                             "Xác nhận xóa",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Gọi BLL để xóa (BLL sẽ tự kiểm tra ràng buộc)
                    string message = bll.DeleteRole(role.MaVaiTro);

                    if (message == "Success")
                    {
                        MessageBox.Show("Đã xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData(); // Load lại lưới
                    }
                    else
                    {
                        // Hiện thông báo lỗi (ví dụ: đang có người dùng)
                        MessageBox.Show(message, "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}