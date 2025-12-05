using System;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using Microsoft.Win32; // Cho SaveFileDialog
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting; // Để dùng SolidColorPaint
using SkiaSharp; // Để dùng SKColors

namespace HRManagementApp.UI.Views.Report
{
    public partial class PayrollReportView : UserControl
    {
        private PayrollBLL _bll = new PayrollBLL();
        private bool _isLoaded = false; // Cờ để tránh load dữ liệu khi chưa khởi tạo xong

        public PayrollReportView()
        {
            InitializeComponent();
            LoadComboBoxes();
            _isLoaded = true;
            LoadData(); // Load lần đầu
        }

        private void LoadComboBoxes()
        {
            // Load Tháng (1 -> 12)
            cboMonth.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                cboMonth.Items.Add($"Tháng {i}");
            }
            cboMonth.SelectedIndex = DateTime.Now.Month - 1; // Chọn tháng hiện tại

            // Load Năm (5 năm gần nhất)
            cboYear.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear; i >= currentYear - 5; i--)
            {
                cboYear.Items.Add(i);
            }
            cboYear.SelectedItem = currentYear;
        }

        // Sự kiện dùng chung cho cả 2 ComboBox
        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            if (cboMonth.SelectedIndex < 0 || cboYear.SelectedItem == null) return;

            int month = cboMonth.SelectedIndex + 1; // Index bắt đầu từ 0
            int year = (int)cboYear.SelectedItem;

            try
            {
                // 1. Load DataGrid (Cũ)
                var listData = _bll.GetPayrollReport(month, year);
                dgPayroll.ItemsSource = listData;

                // 2. Load Thống kê (Mới)
                LoadDashboardStats(month, year);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
            
            
        }
        private void LoadDashboardStats(int month, int year)
        {
            // A. Load Cards Tổng
            var stats = _bll.GetPayrollSummary(month, year);
            txtTotalSalary.Text = stats.TongLuong.ToString("N0") + " đ";
            txtTotalTax.Text = stats.TongThue.ToString("N0") + " đ";
            txtTotalDeduction.Text = stats.TongKhauTru.ToString("N0") + " đ";

            // B. Load Biểu đồ tròn (Cơ cấu thu nhập)
            var incomeStruct = _bll.GetIncomeStructure(month, year);
            chartIncomeStructure.Series = incomeStruct.Select(x =>
                new PieSeries<double>
                {
                    Name = x.Label,
                    Values = new double[] { x.Value },
                    DataLabelsPaint = new SolidColorPaint(new SKColor(0, 0, 0)),
                    DataLabelsSize = 14,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer
                }
            ).ToArray();

            // C. Load Biểu đồ miền (Phân bổ lương)
            var salaryDist = _bll.GetSalaryDistribution(month, year);
            chartSalaryDist.Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Số nhân viên",
                    Values = salaryDist.Select(x => (double)x.Value).ToArray()
                }
            };

            chartSalaryDist.XAxes = new[]
            {
                new Axis
                {
                    Labels = salaryDist.Select(x => x.Label).ToArray()
                }
            };


            // D. Load Biểu đồ cột (Lương TB phòng ban)
            var deptStats = _bll.GetAvgSalaryByDept(month, year);
            chartDeptAvg.Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Lương TB",
                    Values = deptStats.Select(x => (double)x.Value).ToArray(),
                    Fill = new SolidColorPaint(SKColors.DodgerBlue)
                }
            };

            chartDeptAvg.XAxes = new[]
            {
                new Axis
                {
                    Labels = deptStats.Select(x => x.Label).ToArray()
                }
            };


            // E. Load Biểu đồ xu hướng (Đã có sẵn logic trong DAL cũ, tái sử dụng)
            var monthlyStats = _bll.GetPayrollStatistics("Theo Tháng", month, year); // Gọi hàm Dictionary cũ
            chartYearlyTrend.Series = new ISeries[]
            {
                new LineSeries<decimal>
                {
                    Name = "Tổng lương tháng",
                    Values = monthlyStats.Values.ToArray(),
                    GeometrySize = 10,
                    GeometryFill = new SolidColorPaint(SKColors.DodgerBlue),
                    GeometryStroke = new SolidColorPaint(SKColors.White, 2),
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue, 3)
                }
            };

            chartYearlyTrend.XAxes = new[]
            {
                new Axis
                {
                    Labels = monthlyStats.Keys.ToArray()
                }
            };

        }
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            int month = cboMonth.SelectedIndex + 1;
            int year = (int)cboYear.SelectedItem;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = $"BangLuong_T{month}_{year}.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _bll.ExportPayrollToExcel(month, year, saveFileDialog.FileName);
                    MessageBox.Show("Xuất file thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
    }
}