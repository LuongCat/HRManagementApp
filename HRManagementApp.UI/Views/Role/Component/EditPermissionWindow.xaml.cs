using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class EditPermissionWindow : Window
    {
        private RoleModel _currentRole;
        private RoleBLL _bll = new RoleBLL();
        private List<PermissionSelectionViewModel> _permissionsList;

        public EditPermissionWindow(RoleModel role)
        {
            InitializeComponent();
            _currentRole = role;
            txtTitle.Text = $"Phân quyền: {role.TenVaiTro}";
            LoadPermissions();
        }

        private void LoadPermissions()
        {
            // Lấy danh sách quyền và trạng thái tick từ BLL
            _permissionsList = _bll.GetPermissionsForEdit(_currentRole.MaVaiTro);
            icPermissions.ItemsSource = _permissionsList;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Lọc ra các ID quyền được chọn (IsSelected = true)
            var selectedIds = _permissionsList
                                .Where(p => p.IsSelected)
                                .Select(p => p.MaQuyen)
                                .ToList();

            if (_bll.UpdatePermissions(_currentRole.MaVaiTro, selectedIds))
            {
                MessageBox.Show("Cập nhật quyền thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi lưu quyền.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}