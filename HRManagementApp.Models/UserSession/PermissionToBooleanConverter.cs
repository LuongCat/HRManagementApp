using System;
using System.Globalization;
using System.Windows.Data;
using HRManagementApp.models;

namespace HRManagementApp.Converters
{
    public class PermissionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string permissionKey = parameter as string;

            if (string.IsNullOrEmpty(permissionKey))
                return false; // Không có key -> Khóa nút

            // Nếu có quyền -> trả về True (Sáng, bấm được)
            // Nếu không -> trả về False (Mờ, không bấm được)
            return UserSession.HasPermission(permissionKey);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}