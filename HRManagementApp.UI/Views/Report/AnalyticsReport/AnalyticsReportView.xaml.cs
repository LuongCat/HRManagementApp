using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;
using HRManagementApp.BLL; // Nhớ sửa namespace
using HRManagementApp.models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace HRManagementApp.UI.Views.Report
{
    public partial class AnalyticsReportView : UserControl
    {
        private AnalyticsBLL _bll = new AnalyticsBLL();

        // Properties cho Chart Binding
        public ISeries[] MainSeries { get; set; }
        public Axis[] XAxes { get; set; }
        public ISeries[] PieSeries { get; set; }

        public AnalyticsReportView()
        {
            InitializeComponent();
            DataContext = this; // Quan trọng để Binding Chart
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                // 1. Lấy số liệu thống kê (Gọi BLL -> DAL)
                var data = _bll.GetDashboardAnalytics();

                // 2. Gán dữ liệu vào Card
                txtTotalEmp.Text = data.TotalEmployees.ToString();
                txtActiveEmp.Text = $"Đang làm việc: {data.ActiveEmployees}";
                
                txtLate.Text = data.LateToday.ToString();
                txtAbsent.Text = data.AbsentToday.ToString();
                txtPending.Text = data.PendingRequests.ToString();

                // Tính tỷ lệ đi làm
                double rate = 0;
                if (data.ActiveEmployees > 0)
                {
                    int present = data.ActiveEmployees - data.AbsentToday;
                    rate = Math.Round((double)present / data.ActiveEmployees * 100, 1);
                }
                txtRate.Text = $"{rate}%";

                // 3. Cấu hình Biểu đồ Tròn (Pie Chart) - Trạng thái hôm nay
                PieSeries = new ISeries[]
                {
                    new PieSeries<int> { Values = new int[] { data.ActiveEmployees - data.AbsentToday - data.LateToday }, Name = "Đúng giờ", Fill = new SolidColorPaint(SKColors.MediumSeaGreen) },
                    new PieSeries<int> { Values = new int[] { data.LateToday }, Name = "Đi muộn", Fill = new SolidColorPaint(SKColors.Orange) },
                    new PieSeries<int> { Values = new int[] { data.AbsentToday }, Name = "Vắng mặt", Fill = new SolidColorPaint(SKColors.PaleVioletRed) }
                };

                // 4. Cấu hình Biểu đồ Cột (Column Chart) - Ví dụ dữ liệu giả lập hoặc lấy thêm từ DB
                // Bạn có thể viết thêm query lấy số nhân viên theo phòng ban để hiển thị ở đây
                MainSeries = new ISeries[]
                {
                    new ColumnSeries<int>
                    {
                        Name = "Nhân viên",
                        Values = new int[] { 15, 22, 10, 5 }, // Thay bằng dữ liệu thật: số nv theo phòng ban
                        Fill = new SolidColorPaint(SKColors.CornflowerBlue)
                    }
                };

                XAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = new string[] { "Nhân Sự", "Kỹ Thuật", "Kinh Doanh", "Kế Toán" }, // Thay bằng tên PB thật
                        LabelsRotation = 0
                    }
                };

                // Cập nhật giao diện
                chartPie.Series = PieSeries;
                chartMain.Series = MainSeries;
                chartMain.XAxes = XAxes;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                System.Windows.MessageBox.Show("Lỗi tải dashboard: " + ex.Message);
            }
        }
    }
}