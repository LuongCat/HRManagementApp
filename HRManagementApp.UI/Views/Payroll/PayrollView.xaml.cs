using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views
{
    public partial class PayrollView : UserControl
    {
        public PayrollView()
        {
            InitializeComponent();
            LoadDemoData();
        }

        private void LoadDemoData()
        {
            var demoData = new List<EmployeePayroll>
            {
                new EmployeePayroll { MaNV = 1, HoTen = "Nguyễn Văn A", TongNgayCong = 26, LuongCoBan = 8000000, PhuCap = 1000000, LuongThucNhan = 9000000 },
                new EmployeePayroll { MaNV = 2, HoTen = "Trần Thị B", TongNgayCong = 24, LuongCoBan = 7500000, PhuCap = 800000, LuongThucNhan = 8300000 },
                new EmployeePayroll { MaNV = 3, HoTen = "Lê Văn C", TongNgayCong = 28, LuongCoBan = 9000000, PhuCap = 1200000, LuongThucNhan = 10200000 },
            };

            PayrollDataGrid.ItemsSource = demoData;
        }

        private void ViewDetail_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedItem = button?.DataContext as EmployeePayroll;

            if (selectedItem != null)
            {
                // Chuyển sang PayrollDetailView
                var parentWindow = Window.GetWindow(this) as MainWindow;
            }
        }
    }

    public class EmployeePayroll
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public int TongNgayCong { get; set; }
        public decimal LuongCoBan { get; set; }
        public decimal PhuCap { get; set; }
        public decimal LuongThucNhan { get; set; }
    }
}