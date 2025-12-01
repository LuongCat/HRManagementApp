using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HRManagementApp.Converters
{
    // Converter màu nền Badge
    public class LeaveStatusBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString() ?? "";
            return status switch
            {
                "Đã duyệt" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F2937")), // Đen
                "Từ chối" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626")),  // Đỏ
                _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#adadad"))           // Xám (Chờ duyệt)
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // Converter màu chữ Badge
    public class LeaveStatusForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString() ?? "";
            return status == "Chờ duyệt" ? Brushes.Black : Brushes.White;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}