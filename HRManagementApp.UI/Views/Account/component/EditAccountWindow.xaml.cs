using System;
using System.Collections.Generic;
using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class EditAccountWindow : Window
    {
        public AccountManagementModel EditedAccount { get; private set; }
        private string _oldPasswordHash;
        private AccountBLL bll = new AccountBLL();

        public EditAccountWindow(AccountManagementModel accountData)
        {
            InitializeComponent();
            LoadComboBoxData(); // 1. Load danh sách Vai trò & Phòng ban trước
            LoadData(accountData); // 2. Sau đó mới đổ dữ liệu User vào
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Load Vai trò
                List<string> roles = bll.GetRoleList();
                cboVaiTro.ItemsSource = roles;

                // Load Phòng ban
                List<string> depts = bll.GetDepartmentList();
                cboPhongBan.ItemsSource = depts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách Vai trò/Phòng ban: " + ex.Message);
            }
        }

        private void LoadData(AccountManagementModel acc)
        {
            // Clone object
            EditedAccount = new AccountManagementModel
            {
                MaTK = acc.MaTK,
                MaNV = acc.MaNV,
                TenDangNhap = acc.TenDangNhap,
                MatKhau = acc.MatKhau,
                Name = acc.Name,
                SDT = acc.SDT,
                VaiTro = acc.VaiTro,
                PhongBan = acc.PhongBan,
                TrangThai = acc.TrangThai
            };
            
            _oldPasswordHash = acc.MatKhau;
            // Đổ dữ liệu lên giao diện
            txtTenDangNhap.Text = acc.TenDangNhap;
            txtName.Text = acc.Name;
            txtSDT.Text = acc.SDT;
            
            // Set ComboBox Vai Trò (Chọn item khớp với dữ liệu User)
            if (acc.VaiTro != null)
                cboVaiTro.SelectedItem = acc.VaiTro;

            // Set ComboBox Phòng Ban
            if (acc.PhongBan != null)
                cboPhongBan.SelectedItem = acc.PhongBan;

            // Set ComboBox Trạng Thái
            foreach (System.Windows.Controls.ComboBoxItem item in cboTrangThai.Items)
            {
                if (item.Content.ToString() == acc.TrangThai)
                {
                    cboTrangThai.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
            {
                MessageBox.Show("Tên đăng nhập không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Họ tên không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy dữ liệu từ form
            EditedAccount.TenDangNhap = txtTenDangNhap.Text.Trim();
            EditedAccount.Name = txtName.Text.Trim();
            EditedAccount.SDT = txtSDT.Text.Trim();
            
            // Lấy giá trị String từ ComboBox
            EditedAccount.VaiTro = cboVaiTro.SelectedItem?.ToString() ?? "Nhân viên";
            EditedAccount.PhongBan = cboPhongBan.SelectedItem?.ToString() ?? "";

            var selectedStatus = cboTrangThai.SelectedItem as System.Windows.Controls.ComboBoxItem;
            EditedAccount.TrangThai = selectedStatus?.Content?.ToString() ?? "Hoạt động";

            string newPass = pwdPassword.Password;

            // Logic Mật khẩu
            if (!string.IsNullOrEmpty(newPass))
            {
                if (newPass.Length < 6)
                {
                    MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                EditedAccount.MatKhau = newPass; 
            }
            else
            {
                EditedAccount.MatKhau = _oldPasswordHash;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}