using System;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.models;
using HRManagementApp.BLL; // Để gọi hàm Logout nếu cần

namespace HRManagementApp.UI.Views
{
    public partial class AccountInfoView : UserControl
    {
        public AccountInfoView()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            // 1. Thông tin tổng quan (Cột trái)
            string fullName = UserSession.HoTen ?? "User";
            txtName.Text = fullName;
            txtRole.Text = UserSession.VaiTro ?? "Guest";
            
            // Lấy chữ cái đầu làm Avatar
            if (!string.IsNullOrEmpty(fullName))
            {
                txtAvatar.Text = fullName.Substring(0, 1).ToUpper();
            }

            txtUsernameDisplay.Text = UserSession.TenDangNhap;
            txtPhoneDisplay.Text = UserSession.DienThoai ?? "Chưa cập nhật";

            // 2. Thông tin chi tiết (Cột phải)
            txtFullNameDetail.Text = fullName;
            txtDob.Text = UserSession.NgaySinh.HasValue ? UserSession.NgaySinh.Value.ToString("dd/MM/yyyy") : "Chưa cập nhật";
            txtGender.Text = UserSession.GioiTinh ?? "Chưa cập nhật";
            txtCCCD.Text = UserSession.SoCCCD ?? "Chưa cập nhật";

            // 3. Thông tin công việc
            txtDepartment.Text = UserSession.TenPB ?? "Chưa phân bổ";
            txtEmployeeCode.Text = UserSession.MaNV.HasValue ? $"NV{UserSession.MaNV.Value:D4}" : "N/A";
            txtWorkPhone.Text = UserSession.DienThoai ?? "Chưa cập nhật";
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ChangePasswordDialog();
            dialog.ShowDialog();
        }
        
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Gọi lại logic logout từ MainWindow (hoặc xử lý trực tiếp)
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Tìm cửa sổ cha (MainWindow) để đóng và mở lại Login
                var mainWindow = Window.GetWindow(this);
                if (mainWindow != null)
                {
                    var authBLL = new AuthenticationBLL();
                    authBLL.Logout();

                    var loginWin = new LoginWindow();
                    loginWin.Show();
                    mainWindow.Close();
                }
            }
        }
    }
}