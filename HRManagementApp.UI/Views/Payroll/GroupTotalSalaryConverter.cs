using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;


namespace HRManagementApp.UI.Views.Converters
{
    public class GroupTotalSalaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Value ở đây chính là CollectionViewGroup.Items
            if (value is IEnumerable<object> items)
            {
                decimal total = 0;
                foreach (var item in items)
                {
                    if (item is DepartmentPayrollItem payrollItem)
                    {
                        total += payrollItem.LuongThucNhan;
                    }
                }
                return $"Tổng: {total:N0} VNĐ";
            }
            return "0 VNĐ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}