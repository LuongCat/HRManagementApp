using System;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using Microsoft.Win32; // Cho SaveFileDialog
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using HRManagementApp.BLL.Report;

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
            LoadOverview();
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
                var data = _bll.GetPayrollReport(month, year);
                dgPayroll.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
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

        private void LoadOverview()
        {
            string[] monthLabels = ["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"];

            double monthlyPayroll = _bll.GetMonthlyPayroll();
            int payrollEmployees = _bll.GetMonthlyPayrollEmployees();

            double avgPayroll = payrollEmployees == 0 ? 0 : monthlyPayroll / payrollEmployees;

            double prevMonthlyPayroll = _bll.GetPrevMonthlyPayroll();
            double payrollChange = monthlyPayroll - prevMonthlyPayroll;
            double monthlyPaidPayroll = _bll.GetMonthlyPaidPayroll();

            double paidPercent = monthlyPayroll == 0 ? 100 : monthlyPaidPayroll * 100 / monthlyPayroll;
            double unpaidPercent = 100 - paidPercent;

            DepartmentReportService departmentService = new DepartmentReportService();
            string[] departmentNames = departmentService.GetDepartmentNames();
            double[] departmentPayroll = _bll.GetDepartmentPayroll();

            double[] salaryTrend = _bll.GetSalaryTrend();

            txtMonthlyPayroll.Text = monthlyPayroll.ToString() + "K";
            txtAvgPayroll.Text = avgPayroll.ToString() + "K";
            txtPayrollChange.Text = payrollChange > 0 ? "+" +  payrollChange.ToString() + "K" : payrollChange.ToString() + "K";
            txtPayrollEmployees.Text = payrollEmployees.ToString();

            chartPayrollDonut.Series = new ISeries[]
            {
                new PieSeries<double>
                {
                    Name = "Đã trả",
                    Values = new double[] { Math.Round(paidPercent,1) },
                    InnerRadius = 0,
                    Fill = new SolidColorPaint(SKColors.MediumSeaGreen),
                    DataLabelsFormatter = p => $"{p.PrimaryValue}%"
                },
                new PieSeries<double>
                {
                    Name = "Chưa trả",
                    Values = new double[] { Math.Round(unpaidPercent,1) },
                    InnerRadius = 0,
                    Fill = new SolidColorPaint(SKColors.IndianRed),
                    DataLabelsFormatter = p => $"{p.PrimaryValue}%"
                }
            };

            chartSalaryDept.Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Lương",
                    Values = departmentPayroll,
                    Fill = new SolidColorPaint(SKColors.SteelBlue)
                }
            };

            chartSalaryDept.XAxes = new[]
            {
                new Axis
                {
                    Labels = departmentNames,
                    LabelsRotation = 0,
                    TextSize = 8
                }
            };

            chartSalaryTrend.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = salaryTrend,
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 8
                }
            };

            chartSalaryTrend.XAxes = new[]
            {
                new Axis
                {
                    Labels = monthLabels,
                    Padding = new LiveChartsCore.Drawing.Padding(0),
                    MinStep = 1
                }
            };
        }
    }
}