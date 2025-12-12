using System.Windows;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class ShiftManagementDialog : Window
    {
        public ShiftManagementDialog(NhanVien nv)
        {
            InitializeComponent();
            // Gọi hàm load dữ liệu của UserControl con
            UcShiftDetail.LoadDataForEmployee(nv);
        }
    }
}