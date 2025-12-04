using System;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;
using Microsoft.Win32; // Cần thêm cái này cho SaveFileDialog
namespace HRManagementApp.UI.Views
{
    public partial class ReportsView : UserControl
    {
        private AttendanceBLL _bll = new AttendanceBLL();

        public ReportsView()
        {
            InitializeComponent();
            
            // 1. Cài đặt ngày mặc định là hôm nay
            ReportCalendar.SelectedDate = DateTime.Now;
            ReportCalendar.DisplayDate = DateTime.Now;
            
            // 2. Load dữ liệu ngay khi mở form
            LoadReportData(DateTime.Now);
        }

        // Sự kiện khi người dùng chọn ngày trên lịch
        private void ReportCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportCalendar.SelectedDate.HasValue)
            {
                // Lấy ngày người dùng vừa click
                DateTime selectedDate = ReportCalendar.SelectedDate.Value;
                LoadReportData(selectedDate);
            }
        }

        private void LoadReportData(DateTime date)
        {
            try
            {
                // --- PHẦN 1: THỐNG KÊ THÁNG ---
                // Cập nhật tiêu đề
                txtReportTitle.Text = $"Báo cáo chấm công tháng {date:MM/yyyy}";
                
                // Gọi BLL lấy số liệu tổng hợp của tháng
                var reportStat = _bll.GetReport(date);
                
                // Đổ dữ liệu vào 4 thẻ Card
                txtTotalDays.Text = $"{reportStat.TotalWorkDays} công";
                txtRate.Text = $"{reportStat.AvgAttendanceRate}%";
                txtTotalHours.Text = $"{reportStat.TotalWorkHours} giờ";
                txtOvertime.Text = $"{reportStat.OvertimeHours} giờ";

                // --- PHẦN 2: CHI TIẾT NGÀY ---
                // Cập nhật tiêu đề danh sách
                txtDetailTitle.Text = $"Chi tiết chấm công ngày {date:dd/MM/yyyy}";

                // Gọi BLL lấy danh sách nhân viên chấm công ngày hôm đó
                var dailyList = _bll.GetDailyAttendanceList(date);
                
                // Đổ vào DataGrid
                dgDailyDetail.ItemsSource = dailyList;
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu có (ví dụ mất kết nối DB)
                // MessageBox.Show($"Lỗi tải báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportCalendar.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn tháng cần xuất báo cáo.");
                return;
            }

            DateTime selectedDate = ReportCalendar.SelectedDate.Value;

            // Mở hộp thoại lưu file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = $"BaoCaoChamCong_T{selectedDate.Month}_{selectedDate.Year}.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Gọi BLL để xuất file
                    _bll.ExportAttendanceToExcel(selectedDate.Month, selectedDate.Year, saveFileDialog.FileName);
                    MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}