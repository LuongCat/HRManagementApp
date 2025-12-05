using System.Collections.Generic;
using System.IO;
using ZXing;
using ZXing.Common;
using System.Drawing; // Cần System.Drawing.Common
using System.Windows;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using ZXing.Windows.Compatibility;
using Microsoft.Win32; // Cho SaveFileDialog

namespace HRManagementApp.UI.Views
{
    /// <summary>
    /// Window hiển thị thông tin dạng key-value chung cho toàn app
    /// </summary>
    public partial class InfoDisplayWindow : Window
    {
        private string _currentBarcodeContent; // Lưu mã để dùng khi tải về
        public InfoDisplayWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set thông tin hiển thị
        /// </summary>
        /// <param name="title">Tiêu đề chính</param>
        /// <param name="subtitle">Tiêu đề phụ (có thể null)</param>
        /// <param name="infoItems">Danh sách thông tin dạng key-value</param>
        public void SetInfo(string title, string subtitle, List<InfoItem> infoItems)
        {
            TxtTitle.Text = title;
            TxtSubtitle.Text = subtitle ?? "";
            TxtSubtitle.Visibility = string.IsNullOrEmpty(subtitle) ? Visibility.Collapsed : Visibility.Visible;
            
            InfoItemsControl.ItemsSource = infoItems;
        }
        public void ShowBarcode(string content)
        {
            if (string.IsNullOrEmpty(content)) return;

            _currentBarcodeContent = content;
            TxtBarcodeValue.Text = content;
            BarcodeSection.Visibility = Visibility.Visible;

            try
            {
                // Cấu hình tạo mã vạch (Code 128 thông dụng)
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Height = 100,
                        Width = 300,
                        PureBarcode = false, // false để hiện số bên dưới mã vạch
                        Margin = 10
                    }
                };

                // Tạo Bitmap
                using (var bitmap = writer.Write(content))
                {
                    // Chuyển đổi Bitmap sang BitmapImage để hiện lên WPF
                    ImgBarcode.Source = BitmapToImageSource(bitmap);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo mã vạch: " + ex.Message);
            }
        }

        // Hàm hỗ trợ chuyển đổi System.Drawing.Bitmap -> System.Windows.Media.ImageSource
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        // --- SỰ KIỆN: Tải mã vạch về máy ---
        private void BtnDownloadBarcode_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentBarcodeContent)) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
            saveFileDialog.FileName = $"Barcode_{_currentBarcodeContent}.png";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var writer = new BarcodeWriter
                    {
                        Format = BarcodeFormat.CODE_128,
                        Options = new EncodingOptions { Height = 150, Width = 400, Margin = 10 }
                    };

                    using (var bitmap = writer.Write(_currentBarcodeContent))
                    {
                        bitmap.Save(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Tải mã vạch thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu file: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    /// <summary>
    /// Model cho một dòng thông tin
    /// </summary>
    public class InfoItem
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public System.Windows.Media.Brush ValueColor { get; set; } = System.Windows.Media.Brushes.Black;
        public FontWeight ValueWeight { get; set; } = FontWeights.Normal;

        public InfoItem(string label, string value)
        {
            Label = label;
            Value = value;
        }

        public InfoItem(string label, string value, string colorHex, bool isBold = false)
        {
            Label = label;
            Value = value;
            ValueColor = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom(colorHex);
            ValueWeight = isBold ? FontWeights.SemiBold : FontWeights.Normal;
        }
    }
}