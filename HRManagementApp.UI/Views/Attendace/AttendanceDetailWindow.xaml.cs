using System.Windows;
using HRManagementApp.DAL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class AttendanceDetailWindow : Window
    {
        private ChamCongRepository _repo = new ChamCongRepository();

        public AttendanceDetailWindow(int maNV, string tenNV, int thang, int nam)
        {
            InitializeComponent();

            // Set Title
            //txtTitle.Text = $"Chi tiết chấm công: {tenNV}";
            //txtSubTitle.Text = $"Dữ liệu tháng {thang}/{nam}";

            LoadData(maNV, thang, nam);
        }

        private void LoadData(int maNV, int thang, int nam)
        {
            // Gọi hàm GetChamCongByMonth từ Repository bạn đã cung cấp
            var list = _repo.GetChamCongByMonth(maNV, thang, nam);
            dgDetail.ItemsSource = list;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}