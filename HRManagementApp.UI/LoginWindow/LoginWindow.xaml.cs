using HRManagementApp.BLL;
using System.Windows;

namespace HRManagementApp.UI
{
    public partial class LoginWindow : Window
    {
        private readonly TaiKhoanService _service = new TaiKhoanService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
{
    string username = UsernameTextBox.Text.Trim();
    string password = PasswordBox.Password.Trim();

    // Kiểm tra trống trước 
    if (string.IsNullOrEmpty(username))
    {
        ErrorText.Text = "Vui lòng nhập tên đăng nhập!";
        UsernameTextBox.Focus();
        return;
    }

    if (string.IsNullOrEmpty(password))
    {
        ErrorText.Text = "Vui lòng nhập mật khẩu!";
        PasswordBox.Focus();
        return;
    }

    // Nếu đã nhập đầy đủ -> mới gọi BLL
    if (_service.DangNhap(username, password))
    {
        MainWindow main = new MainWindow();
        main.Show();
        this.Close();
    }
    else
    {
        ErrorText.Text = "Tên đăng nhập hoặc mật khẩu không đúng!";
    }
}

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
{
    PasswordPlaceholder.Visibility =
        string.IsNullOrEmpty(PasswordBox.Password)
        ? Visibility.Visible
        : Visibility.Collapsed;
}

    }
}
