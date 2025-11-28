using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class TodayAttendanceView : UserControl
    {
        private AttendanceBLL _bll = new AttendanceBLL();

        public TodayAttendanceView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // 1. Lấy danh sách từ BLL (BLL sẽ gọi DAL lấy từ Database)
                List<AttendanceTodayModel> list = _bll.GetTodayAttendanceList();

                // 2. Gán dữ liệu vào DataGrid
                dgTodayAttendance.ItemsSource = list;

                // 3. Lấy số liệu thống kê từ BLL
                var stats = _bll.GetTodayStatistics(list);

                // 4. Hiển thị lên các thẻ thống kê (đã đặt x:Name trong XAML)
                txtTotalStaff.Text = stats.Total.ToString();
                txtPresent.Text = stats.Present.ToString();
                txtAbsent.Text = stats.Absent.ToString();
                txtLate.Text = stats.Late.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu chấm công: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}