using HRManagementApp.BLL;
using System.Windows;
using System.Windows.Input;

namespace HRManagementApp.UI
{
    public partial class LoginWindow : Window
    {
        private AuthenticationBLL _authBLL = new AuthenticationBLL();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        // Cho phép nhấn Enter để đăng nhập
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        private void PerformLogin()
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string errorMsg;

            if (_authBLL.Login(username, password, out errorMsg))
            {
                // Đăng nhập thành công -> Mở MainWindow
                MainWindow main = new MainWindow();
                main.Show();
                
                // Đóng cửa sổ đăng nhập
                this.Close();
            }
            else
            {
                ErrorText.Text = errorMsg;
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordPlaceholder != null)
            {
                PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }
}