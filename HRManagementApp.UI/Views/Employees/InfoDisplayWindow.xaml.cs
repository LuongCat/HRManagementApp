using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace HRManagementApp.Views
{
    /// <summary>
    /// Window hiển thị thông tin dạng key-value chung cho toàn app
    /// </summary>
    public partial class InfoDisplayWindow : Window
    {
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
        public Brush ValueColor { get; set; } = Brushes.Black;
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
            ValueColor = (Brush)new BrushConverter().ConvertFrom(colorHex);
            ValueWeight = isBold ? FontWeights.SemiBold : FontWeights.Normal;
        }
    }
}