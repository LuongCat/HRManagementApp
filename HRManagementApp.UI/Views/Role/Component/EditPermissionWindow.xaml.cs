using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class EditPermissionWindow : Window
    {
        private RoleModel _currentRole;
        private RoleBLL _bll = new RoleBLL();
        
        // Đổi List thành List các ModuleGroupViewModel
        private List<ModuleGroupViewModel> _moduleGroups;

        public EditPermissionWindow(RoleModel role)
        {
            InitializeComponent();
            _currentRole = role;
            txtTitle.Text = $"Phân quyền: {role.TenVaiTro}";
            LoadPermissions();
        }

        private void LoadPermissions()
        {
            // Sử dụng hàm GetGroupedPermissions mới
            _moduleGroups = _bll.GetGroupedPermissions(_currentRole.MaVaiTro);
            icPermissions.ItemsSource = _moduleGroups;
        }

        // Sự kiện mở Popup Chi tiết
        private void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var group = btn.Tag as ModuleGroupViewModel;

            if (group != null)
            {
                // Nếu người dùng chưa tick vào Module chính mà bấm chi tiết
                // Ta có thể tự động tick luôn Module chính (tùy chọn)
                if(!group.MainPermission.IsSelected)
                {
                    group.MainPermission.IsSelected = true;
                }

                // Mở cửa sổ popup, truyền danh sách quyền phụ
                var detailWin = new DetailedPermissionWindow(group.TenModule, group.DetailedPermissions);
                detailWin.Owner = this;
                detailWin.ShowDialog();
                
                // Sau khi đóng Popup, UI tự cập nhật nhờ Binding IsSelected
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectedIds = new List<int>();

            foreach (var group in _moduleGroups)
            {
                // 1. Nếu quyền chính (Xem) được chọn -> Thêm ID
                if (group.MainPermission != null && group.MainPermission.IsSelected)
                {
                    selectedIds.Add(group.MainPermission.MaQuyen);
                }

                // 2. Duyệt qua các quyền phụ (Thêm, Xóa...), cái nào chọn thì thêm ID
                foreach (var detail in group.DetailedPermissions)
                {
                    if (detail.IsSelected)
                    {
                        selectedIds.Add(detail.MaQuyen);
                    }
                }
            }

            // Gọi BLL cập nhật
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