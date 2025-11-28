using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class AddRoleWindow : Window
    {
        public AddRoleWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên vai trò");
                return;
            }

            // Chỉ thực hiện logic THÊM MỚI
            var role = new RoleModel 
            { 
                TenVaiTro = txtName.Text.Trim(), 
                MoTa = txtDesc.Text.Trim() 
            };

            if (new RoleBLL().AddRole(role))
            {
                MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Lỗi thêm vai trò.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}