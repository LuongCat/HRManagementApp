using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

// --- USING CỦA LIVECHARTS2 ---
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting; // Để dùng SolidColorPaint
using SkiaSharp; // Để dùng SKColors
// -----------------------------

namespace HRManagementApp.UI.Views.Report
{
    public partial class EmployeeReportView : UserControl
    {
        private EmployeeReportBLL _bll = new EmployeeReportBLL();

        // Properties cho Binding LiveCharts2
        public ISeries[] MainSeries { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }
        public ISeries[] PieSeries { get; set; }

        public EmployeeReportView()
        {
            InitializeComponent();
            DataContext = this; // Quan trọng để Binding hoạt động
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                var data = _bll.GetDashboarEmployee();

                // 1. Gán số liệu Card
                txtTotalEmp.Text = data.TotalEmployees.ToString();
                txtActiveEmp.Text = $"Đang làm việc: {data.ActiveEmployees}";
                txtLate.Text = data.LateToday.ToString();
                txtAbsent.Text = data.AbsentToday.ToString();
                txtPending.Text = data.PendingRequests.ToString();

                double rate = 0;
                if (data.ActiveEmployees > 0)
                {
                    int present = data.ActiveEmployees - data.AbsentToday;
                    rate = Math.Round((double)present / data.ActiveEmployees * 100, 1);
                }
                txtRate.Text = $"{rate}%";

                // 2. Lấy dữ liệu Phòng ban từ DB
                var deptStats = _bll.GetDepartmentStats();

                // 3. Tạo Biểu Đồ
                CreateMainChart(deptStats);
                CreatePieChart(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dashboard: " + ex.Message);
            }
        }

        private void CreateMainChart(Dictionary<string, int> deptStats)
        {
            // Chuyển Dictionary thành các mảng dữ liệu
            var labels = new List<string>();
            var values = new List<int>();

            foreach(var item in deptStats)
            {
                labels.Add(item.Key);
                values.Add(item.Value);
            }

            // Cấu hình Series (Cột)
            MainSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Name = "Số lượng",
                    Values = values.ToArray(),
                    Fill = new SolidColorPaint(SKColors.CornflowerBlue), // Màu xanh hiện đại
                    DataLabelsSize = 14,
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsFormatter = point => point.Model.ToString()
                }
            };

            // Cấu hình Trục X (Tên phòng ban)
            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels.ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    TextSize = 12
                }
            };

            // Cấu hình Trục Y (Số lượng)
            YAxes = new Axis[]
            {
                new Axis
                {
                    MinStep = 1, // Chỉ hiện số nguyên
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    TextSize = 12
                }
            };

            // Cập nhật Binding
            OnPropertyChanged(nameof(MainSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
        }

        private void CreatePieChart(EmployeeModel data)
        {
            // Tính toán số liệu
            int onTime = data.ActiveEmployees - data.AbsentToday - data.LateToday;
            if (onTime < 0) onTime = 0;

            var seriesList = new List<ISeries>();

            // Chỉ thêm các phần tử có giá trị > 0 để biểu đồ đẹp hơn
            if (onTime > 0)
            {
                seriesList.Add(new PieSeries<int> 
                { 
                    Values = new int[] { onTime }, 
                    Name = "Đúng giờ", 
                    Fill = new SolidColorPaint(SKColors.MediumSeaGreen),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12,
                    DataLabelsFormatter = point => $"{point.Model} ({point.StackedValue.Share:P1})"
                });
            }

            if (data.LateToday > 0)
            {
                seriesList.Add(new PieSeries<int> 
                { 
                    Values = new int[] { data.LateToday }, 
                    Name = "Đi muộn", 
                    Fill = new SolidColorPaint(SKColors.Orange),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12,
                    DataLabelsFormatter = point => $"{point.Model}"
                });
            }

            if (data.AbsentToday > 0)
            {
                seriesList.Add(new PieSeries<int> 
                { 
                    Values = new int[] { data.AbsentToday }, 
                    Name = "Vắng", 
                    Fill = new SolidColorPaint(SKColors.PaleVioletRed),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12,
                    DataLabelsFormatter = point => $"{point.Model}"
                });
            }

            PieSeries = seriesList.ToArray();
            OnPropertyChanged(nameof(PieSeries));
        }

        // Implement INotifyPropertyChanged để UI tự cập nhật khi dữ liệu thay đổi
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
    }
}