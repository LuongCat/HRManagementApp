using System.Windows;
using HRManagementApp.UI;

namespace HRManagementApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Khởi chạy màn hình đăng nhập trước
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}