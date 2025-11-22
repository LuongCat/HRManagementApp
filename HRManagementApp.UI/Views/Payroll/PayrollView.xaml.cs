using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views
{
    public partial class PayrollView : UserControl
    {

        public NhanVienService NhanVienService;
        public PayrollView()
        {
            InitializeComponent();
            NhanVienService = new NhanVienService(); 
            LoadDemoData();
        }

        private void LoadDemoData()
        {
            var lisnhanvien = NhanVienService.GetListNhanVien();

            PayrollDataGrid.ItemsSource = lisnhanvien;
        }

        private void ViewDetail_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedItem = button?.DataContext;

            if (selectedItem != null)
            {
                // Chuyển sang PayrollDetailView
                var parentWindow = Window.GetWindow(this) as MainWindow;
            }
        }
        
        
        
    }

    public class PayrollItem
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public decimal LuongCoBan { get; set; }
        public int NgayCong { get; set; }
        public decimal PhuCap { get; set; }
        public decimal TangCa { get; set; }
        public decimal Thuong { get; set; }
        public decimal KhauTru { get; set; }
        public decimal Tax { get; set; }
    }

}