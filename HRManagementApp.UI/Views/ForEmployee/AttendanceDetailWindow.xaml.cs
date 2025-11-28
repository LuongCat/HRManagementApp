using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.DAL;

namespace HRManagementApp.UI.Views
{
    public partial class AttendanceDetailWindow2 : Window
    {
        private ChamCongService _chamCongService= new ChamCongService();

        public AttendanceDetailWindow2(int maNV, DateTime date)
        {
            InitializeComponent();
            txtTitle.Text = $"Chi tiết chấm công ngày {date:dd/MM/yyyy}";
            LoadData(maNV, date);
        }

        private void LoadData(int maNV, DateTime date)
        {
            // Gọi hàm repository bạn đã cung cấp để lấy dữ liệu theo ngày cụ thể
            var list = _chamCongService.GetAllAttendancByMonthYear(date.Day, date.Month, date.Year);
            dgDetails.ItemsSource = list;
        }
    }
}