using System;
using System.Drawing; // Cần System.Drawing.Common
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using HRManagementApp.BLL;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using OpenCvSharp.Extensions;
using ZXing;
using ZXing.Windows.Compatibility;
namespace HRManagementApp.UI.Views
{
    public partial class ScanAttendanceView : UserControl
    {
        private VideoCapture _capture;
        private CancellationTokenSource _cancellationTokenSource;
        private ChamCongService _service = new ChamCongService();
        private bool _isProcessing = false; 
        private BarcodeReader _barcodeReader;

        public ScanAttendanceView()
        {
            InitializeComponent();
            InitializeCamera();
            
            _barcodeReader = new BarcodeReader();
            _barcodeReader.Options.TryHarder = true;
        }

        private async void InitializeCamera()
        {
            try
            {
                _capture = new VideoCapture(0);
                
                if (!_capture.IsOpened())
                {
                    MessageBox.Show("Không tìm thấy Camera!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _cancellationTokenSource = new CancellationTokenSource();
                
                await Task.Run(() => CameraLoop(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi động Camera: " + ex.Message);
            }
        }

        private void CameraLoop(CancellationToken token)
        {
            using (Mat frame = new Mat())
            {
                while (!token.IsCancellationRequested)
                {
                    _capture.Read(frame);

                    if (!frame.Empty())
                    {
                        Cv2.Flip(frame, frame, FlipMode.Y);
                        Dispatcher.Invoke(() =>
                        {
                            CameraImage.Source = frame.ToBitmapSource();
                        });

                        if (!_isProcessing)
                        {
                            try 
                            {
                                using (var bitmap = BitmapConverter.ToBitmap(frame)) 
                                {
                                    var result = _barcodeReader.Decode(bitmap);
                                    if (result != null)
                                    {
                                        string code = result.Text;
                                        
                                        Dispatcher.Invoke(() => ProcessCode(code));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi khởi động Camera: " + ex.Message);
                            }
                        }
                    }
                    Thread.Sleep(33); 
                }
            }
        }

        private async void ProcessCode(string code)
        {
            if (_isProcessing) return;
            _isProcessing = true;

            if (int.TryParse(code, out int maNV))
            {
                var result = _service.ProcessSmartAttendance(maNV);

                ShowStatusResult(result.Success, result.Message, result.EmployeeName, result.ActionType);

                await Task.Delay(3000);
                
                ResetStatus();
            }
            else
            {

            }

            _isProcessing = false;
        }

        private void ShowStatusResult(bool success, string message, string name, string type)
        {
            pnlStatus.Visibility = Visibility.Visible;
            txtEmployeeName.Text = name;
            txtEmployeeID.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtTime.Text = DateTime.Now.ToString("HH:mm:ss");
            txtMessage.Text = message;

            if (success)
            {
                txtStatusIcon.Text = "✅";
                txtMessage.Foreground = System.Windows.Media.Brushes.Green;
                
                if (type == "CheckIn")
                {
                    txtAttendanceType.Text = "CHECK IN";
                    borderType.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#DCFCE7");
                    txtAttendanceType.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#166534");
                }
                else
                {
                    txtAttendanceType.Text = "CHECK OUT";
                    borderType.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#DBEAFE");
                    txtAttendanceType.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#1E40AF");
                }
            }
            else
            {
                txtStatusIcon.Text = "❌";
                txtAttendanceType.Text = "ERROR";
                txtMessage.Foreground = System.Windows.Media.Brushes.Red;
                borderType.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#FEE2E2");
                txtAttendanceType.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#991B1B");
            }
        }

        private void ResetStatus()
        {
            pnlStatus.Visibility = Visibility.Collapsed;
            txtManualCode.Clear();
            txtManualCode.Focus(); 
        }

        private void TxtManualCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessCode(txtManualCode.Text.Trim());
            }
        }

        private void BtnManualEnter_Click(object sender, RoutedEventArgs e)
        {
            ProcessCode(txtManualCode.Text.Trim());
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }

            if (_capture != null)
            {
                _capture.Release();
                _capture.Dispose();
            }
        }
    }
}