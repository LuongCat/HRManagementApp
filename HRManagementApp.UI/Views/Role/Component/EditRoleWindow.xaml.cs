using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class EditRoleWindow : Window
    {
        private RoleModel _role; // Lưu trữ vai trò đang sửa

        // Constructor bắt buộc phải truyền Role vào
        public EditRoleWindow(RoleModel roleToEdit)
        {
            InitializeComponent();
            _role = roleToEdit;
            LoadData();
        }

        private void LoadData()
        {
            // Đổ dữ liệu cũ lên Form
            if (_role != null)
            {
                txtName.Text = _role.TenVaiTro;
                txtDesc.Text = _role.MoTa;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên vai trò không được để trống.");
                return;
            }

            // Cập nhật dữ liệu vào object
            _role.TenVaiTro = txtName.Text.Trim();
            _role.MoTa = txtDesc.Text.Trim();

            // Gọi BLL Update
            if (new RoleBLL().UpdateRole(_role))
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true; // Báo cho cửa sổ cha biết là đã xong
                this.Close();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}