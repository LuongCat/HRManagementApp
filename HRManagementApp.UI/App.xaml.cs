using System.Windows;
using HRManagementApp.UI;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try 
            {
                SynsPermissionBLL sysBLL = new SynsPermissionBLL();
                sysBLL.SynsPermissionSystem();
            }
            catch (System.Exception ex)
            {
                // Ghi log lỗi nếu database chưa kết nối được (tránh crash app)
                MessageBox.Show("Không thể đồng bộ quyền hạn: " + ex.Message);
            }
            // Khởi chạy màn hình đăng nhập trước
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}