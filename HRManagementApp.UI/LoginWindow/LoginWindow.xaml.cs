using HRManagementApp.BLL;
using System.Windows;
using System.Windows.Input;
using QRCoder; 
using System.Drawing; // Lưu ý: Cần add reference System.Drawing hoặc cài NuGet System.Drawing.Common
using System.Windows.Media.Imaging;
using System.IO;
using HRManagementApp.models;
namespace HRManagementApp.UI
{
    public partial class LoginWindow : Window
    {
        private AuthenticationBLL _authBLL = new AuthenticationBLL();
        private QrLoginBLL _qrBLL = new QrLoginBLL();
        public LoginWindow()
        {
            InitializeComponent();
            UsernameTextBox.Focus();
            _qrBLL.OnLoginSuccess += OnQrLoginSuccess;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        private void PerformLogin()
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string errorMsg;

            if (_authBLL.Login(username, password, out errorMsg))
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                ErrorText.Text = errorMsg;
                ErrorText.Visibility = Visibility.Visible;
            }
        }
        private void ShowQr_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                string url = _qrBLL.GenerateSessionUrl();
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                QrImage.Source = BitmapToImageSource(qrCodeImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tạo mã QR: " + ex.Message);
            }
        }

        // Hàm xử lý khi đăng nhập thành công từ QR
        private void OnQrLoginSuccess(string username, string password)
        {
            string errorMsg;
            // Bắt buộc dùng Dispatcher vì sự kiện này gọi từ luồng Server ảo
            this.Dispatcher.Invoke(() =>
            {
                bool isSuccess = _authBLL.Login(username,password, out  errorMsg);

                if (isSuccess)
                {
                    MessageBox.Show($"Đăng nhập QR thành công!", "Thông báo");
                    
                    MainWindow main = new MainWindow();
                    main.Show();
                    
                    _qrBLL.StopServer();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tài khoản quét được không tồn tại trong hệ thống!", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        // Nút tắt ứng dụng
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}