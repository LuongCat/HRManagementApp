using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HRManagementApp.Converters
{
    // Converter cho màu text của trạng thái
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString()?.ToLower() ?? "";

            switch (status)
            {
                case "đang làm":
                case "đang làm việc":
                case "active":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#047857")); // Green

                case "nghỉ phép":
                case "leave":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D97706")); // Orange

                case "nghỉ việc":
                case "resigned":
                case "đã nghỉ":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626")); // Red

                case "thử việc":
                case "probation":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2563EB")); // Blue

                default:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")); // Gray
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter cho màu nền của trạng thái
    public class StatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString()?.ToLower() ?? "";

            switch (status)
            {
                case "đang làm":
                case "đang làm việc":
                case "active":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1FAE5")); // Light Green

                case "nghỉ phép":
                case "leave":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF3C7")); // Light Orange

                case "nghỉ việc":
                case "resigned":
                case "đã nghỉ":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEE2E2")); // Light Red

                case "thử việc":
                case "probation":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DBEAFE")); // Light Blue

                default:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F4F6")); // Light Gray
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter để ẩn placeholder khi TextBox có text
    public class TextToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            return string.IsNullOrEmpty(text) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}