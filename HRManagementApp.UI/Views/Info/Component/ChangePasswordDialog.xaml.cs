using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.models;
namespace HRManagementApp.UI.Views
{
    public partial class ChangePasswordDialog : Window
    {
        public ChangePasswordDialog()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string oldPass = txtOldPass.Password.Trim();
            string newPass = txtNewPass.Password.Trim();
            string confirm = txtConfirmPass.Password.Trim();

            // Validate
            if (string.IsNullOrEmpty(oldPass) ||
                string.IsNullOrEmpty(newPass) ||
                string.IsNullOrEmpty(confirm))
            {
                ShowError("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            if (newPass.Length < 6)
            {
                ShowError("Mật khẩu mới phải có ít nhất 6 ký tự.");
                return;
            }

            if (newPass != confirm)
            {
                ShowError("Mật khẩu xác nhận không khớp.");
                return;
            }

            // Gọi BLL đổi mật khẩu
            var authBLL = new AuthenticationBLL();
            bool success = authBLL.ChangePassword(UserSession.MaTK, oldPass ,newPass );

            if (!success)
            {
                ShowError("Mật khẩu cũ không chính xác.");
                return;
            }

            MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            Close();
        }

        private void ShowError(string msg)
        {
            txtError.Text = msg;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
