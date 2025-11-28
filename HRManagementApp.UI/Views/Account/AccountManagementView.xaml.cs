using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class AccountManagementView : UserControl
    {
        private List<AccountManagementModel> allAccounts = new List<AccountManagementModel>();
        private AccountBLL accBLL = new AccountBLL();

        public AccountManagementView()
        {
            InitializeComponent();
            LoadAccounts();
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadAccounts()
        {
            try
            {
                allAccounts = accBLL.getAllAccountModelBLL();
                dgAccounts.ItemsSource = allAccounts;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatistics()
        {
            if (allAccounts == null) return;
            txtTotalAccounts.Text = allAccounts.Count.ToString();
            txtActiveAccounts.Text = allAccounts.Count(a => a.TrangThai == "Hoạt động").ToString();
            txtLockedAccounts.Text = allAccounts.Count(a => a.TrangThai == "Đã khóa").ToString();
            txtManagerAccounts.Text = allAccounts.Count(a => a.VaiTro != null && a.VaiTro.Contains("Quản lý")).ToString();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower().Trim();
            var selectedColumnItem = cboSearchColumn.SelectedItem as ComboBoxItem;
            string selectedColumn = selectedColumnItem?.Content?.ToString() ?? "Tất cả";

            if (allAccounts == null || allAccounts.Count == 0) return;

            if (string.IsNullOrEmpty(searchText))
            {
                dgAccounts.ItemsSource = allAccounts;
                return;
            }

            var filteredAccounts = allAccounts.Where(acc =>
            {
                string tenDangNhap = acc.TenDangNhap?.ToLower() ?? "";
                string name = acc.Name?.ToLower() ?? "";
                string sdt = acc.SDT?.ToLower() ?? "";
                string vaiTro = acc.VaiTro?.ToLower() ?? "";
                string phongBan = acc.PhongBan?.ToLower() ?? "";

                switch (selectedColumn)
                {
                    case "Tên đăng nhập": return tenDangNhap.Contains(searchText);
                    case "Họ tên": return name.Contains(searchText);
                    case "SDT": return sdt.Contains(searchText);
                    case "Vai trò": return vaiTro.Contains(searchText);
                    case "Phòng ban": return phongBan.Contains(searchText);
                    default: 
                        return tenDangNhap.Contains(searchText) || name.Contains(searchText) ||
                               sdt.Contains(searchText) || vaiTro.Contains(searchText) || phongBan.Contains(searchText);
                }
            }).ToList();

            dgAccounts.ItemsSource = filteredAccounts;
        }

        // --- ĐÃ SỬA LOGIC TẠI ĐÂY ---
        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddAccountWindow();
            win.Owner = Window.GetWindow(this);
            
            // Mở cửa sổ thêm mới
            var result = win.ShowDialog();

            // Nếu cửa sổ trả về True (nghĩa là đã thêm thành công bên trong AddAccountWindow)
            if (result == true)
            {
                // KHÔNG gọi lại hàm addAccountBLL ở đây nữa
                // Chỉ cần tải lại danh sách để hiển thị dữ liệu mới
                LoadAccounts();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var account = button?.CommandParameter as AccountManagementModel;

            if (account != null)
            {
                var editWin = new EditAccountWindow(account);
                editWin.Owner = Window.GetWindow(this);

                if (editWin.ShowDialog() == true)
                {
                    var updatedAcc = editWin.EditedAccount;
                    
                    // Logic update vẫn giữ nguyên vì EditAccountWindow chỉ trả về object chứ chưa lưu xuống DB
                    // (Tùy thuộc vào cách bạn viết EditAccountWindow, nếu EditWindow cũng tự lưu rồi thì ở đây cũng chỉ cần LoadAccounts)
                    if (accBLL.UpdateFullAccountBLL(updatedAcc))
                    {
                        MessageBox.Show($"Cập nhật tài khoản '{updatedAcc.TenDangNhap}' thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAccounts();
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra khi cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnLock_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var account = button?.CommandParameter as AccountManagementModel;

            if (account == null) return;

            string actionName = (account.TrangThai == "Hoạt động") ? "khóa" : "mở khóa";
            string confirmMsg = $"Bạn có chắc chắn muốn {actionName} tài khoản '{account.TenDangNhap}' không?";

            var result = MessageBox.Show(confirmMsg, "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool success = accBLL.LockUnlockAccountBLL(account.MaTK, account.TrangThai);

                if (success)
                {
                    LoadAccounts();
                }
                else
                {
                    MessageBox.Show("Thao tác thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}