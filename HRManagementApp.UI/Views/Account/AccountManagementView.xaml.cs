using System;
using System.Collections.Generic;
using System.Windows.Controls;
using HRManagementApp.BLL.AccountDetailDTO;
namespace HRManagementApp.Views.Account
{
    public partial class AccountManagementView : UserControl
    {
        private List<AccountDetailDTO> allAccounts;


        public AccountManagementView()
        {
                InitializeComponent();
                LoadAccounts();
                txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadAccounts()
        {
            allAccounts = new List<AccountDetailDTO>
                {
                    new AccountDetailDTO
                    {
                        TenDangNhap = "nguyenvana",
                        Name = "Nguyễn Văn A",
                        Email = "nguyenvana@company.com",
                        VaiTro = "Quản lý",
                        PhongBan = "IT",
                        TrangThai = "Hoạt động",
                        // LastLogin = "22/10/2023 15:30"
                    },
                    new AccountDetailDTO
                    {
                        TenDangNhap = "tranthib",
                        Name = "Trần Thị B",
                        Email = "tranthib@company.com",
                        VaiTro = "Nhân viên",
                        PhongBan = "Sales",
                        TrangThai = "Hoạt động",
                        // LastLogin = "22/10/2023 15:30"
                    },
                    new AccountDetailDTO
                    {
                        TenDangNhap = "levanc",
                        Name = "Lê Văn C",
                        Email = "levanc@company.com",
                        VaiTro = "Quản lý",
                        PhongBan = "Marketing",
                        TrangThai = "Hoạt động",
                        // LastLogin = "22/10/2023 15:30"
                    },
                    new AccountDetailDTO
                    {
                        TenDangNhap = "phamthid",
                        Name = "Phạm Thị D",
                        Email = "phamthid@company.com",
                        VaiTro = "HR",
                        PhongBan = "HR",
                        TrangThai = "Khóa tài khoản",
                        // LastLogin = "22/10/2023 15:30"
                    },
                    new AccountDetailDTO
                    {
                        TenDangNhap = "hoangvane",
                        Name = "Hoàng Văn E",
                        Email = "hoangvane@company.com",
                        VaiTro = "Nhân viên",
                        PhongBan = "Finance",
                        TrangThai = "Khóa tài khoản",
                        // LastLogin = "22/10/2023 15:30"
                    }
                };

            dgAccounts.ItemsSource = allAccounts;
        }
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
                string searchText = txtSearch.Text.ToLower().Trim();
                var selectedColumn = (cboSearchColumn.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (string.IsNullOrEmpty(searchText))
                {
                    dgAccounts.ItemsSource = allAccounts;
                    return;
                }

                var filteredAccounts = allAccounts.Where(acc =>
                {
                    switch (selectedColumn)
                    {
                        case "Tên đăng nhập":
                            return acc.TenDangNhap.ToLower().Contains(searchText);
                        case "Họ tên":
                            return acc.Name.ToLower().Contains(searchText);
                        case "Email":
                            return acc.Email.ToLower().Contains(searchText);
                        case "Vai trò":
                            return acc.VaiTro.ToLower().Contains(searchText);
                        case "Phòng ban":
                            return acc.PhongBan.ToLower().Contains(searchText);
                        default: // "Tất cả"
                            return acc.TenDangNhap.ToLower().Contains(searchText) ||
                                   acc.Name.ToLower().Contains(searchText) ||
                                   acc.Email.ToLower().Contains(searchText) ||
                                   acc.VaiTro.ToLower().Contains(searchText) ||
                                   acc.PhongBan.ToLower().Contains(searchText);
                    }
                }).ToList();

                dgAccounts.ItemsSource = filteredAccounts;
        }
    }
}
