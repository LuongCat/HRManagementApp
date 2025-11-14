using System;
using System.Collections.Generic;
using System.Linq;  // ← THÊM DÒNG NÀY
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;
namespace HRManagementApp.UI.Views
{
    public partial class AccountManagementView : UserControl
    {
        private List<AccountModel> allAccounts;
        private AccountBLL accBLL = new AccountBLL();

        public AccountManagementView()
        {
                InitializeComponent();
                LoadAccounts();
                txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadAccounts()
        { 
        List<AccountModel> acc = accBLL.getAllAccountModelBLL();
        
        Console.WriteLine(acc);
        allAccounts = accBLL.getAllAccountModelBLL();
        {/* allAccounts =new List<AccountModel>
                {
                    new AccountModel
                    {
                        TenDangNhap = "nguyenvana",
                        Name = "Nguyễn Văn A",
                        SDT = "nguyenvana@company.com",
                        VaiTro = "Quản lý",
                        PhongBan = "IT",
                        TrangThai = "Hoạt động",
                        // LastLogin = "22/10/2023 15:30"
                    },
                    new AccountModel
                    {
                        TenDangNhap = "tranthib",
                        Name = "Trần Thị B",
                        SDT = "tranthib@company.com",
                        VaiTro = "Nhân viên",
                        PhongBan = "Sales",
                        TrangThai = "Hoạt động",
                        // LastLogin = "22/10/2023 15:30"
                    },
                    new AccountModel
                    {
                        TenDangNhap = "levanc",
                        Name = "Lê Văn C",
                        SDT = "levanc@company.com",
                        VaiTro = "Quản lý",
                        PhongBan = "Marketing",
                        TrangThai = "Hoạt động",
                        // LastLogin = "22/10/2023 15:30"
                    },
                    new AccountModel
                    {
                        TenDangNhap = "phamthid",
                        Name = "Phạm Thị D",
                        SDT = "phamthid@company.com",
                        VaiTro = "HR",
                        PhongBan = "HR",
                        TrangThai = "Khóa tài khoản",
                        // LastLogin = "22/10/2023 15:30"
                    },
                    new AccountModel
                    {
                        TenDangNhap = "hoangvane",
                        Name = "Hoàng Văn E",
                        SDT = "hoangvane@company.com",
                        VaiTro = "Nhân viên",
                        PhongBan = "Finance",
                        TrangThai = "Khóa tài khoản",
                        // LastLogin = "22/10/2023 15:30"
                    }
                };}*/}

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
                        case "Số Điện Thoại":
                            return acc.SDT.ToLower().Contains(searchText);
                        case "Vai trò":
                            return acc.VaiTro.ToLower().Contains(searchText);
                        case "Phòng ban":
                            return acc.PhongBan.ToLower().Contains(searchText);
                        default: // "Tất cả"
                            return acc.TenDangNhap.ToLower().Contains(searchText) ||
                                   acc.Name.ToLower().Contains(searchText) ||
                                   acc.SDT.ToLower().Contains(searchText) ||
                                   acc.VaiTro.ToLower().Contains(searchText) ||
                                   acc.PhongBan.ToLower().Contains(searchText);
                    }
                }).ToList();

                dgAccounts.ItemsSource = filteredAccounts;
        }
    }
}
