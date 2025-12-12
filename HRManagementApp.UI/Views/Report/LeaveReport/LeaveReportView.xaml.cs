using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HRManagementApp.BLL.Report;
using HRManagementApp.models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SkiaSharp;

namespace HRManagementApp.UI.Views.Report
{
    public partial class LeaveReportView : UserControl
    {
        private bool _isLoaded = false;
        private readonly LeaveReportService leaveService = new LeaveReportService();

        public LeaveReportView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            LoadOverview();

            DepartmentReportService departmentReportService = new DepartmentReportService();
            string[] departmentNames = departmentReportService.GetDepartmentNames();

            var list = new List<string> { "Tất cả" };
            list.AddRange(departmentNames);

            CbLeaveDepartment.ItemsSource = list;
            CbLeaveDepartment.SelectedIndex = 0;

            LoadTable();
        }

        // ---------------------------------------------------------
        // LOAD TABLE
        // ---------------------------------------------------------
        public void LoadTable()
        {
            if (!_isLoaded) return;

            try
            {
                string name = TbLeaveSearchName.Text;
                DateOnly? date = DpLeaveDate.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(DpLeaveDate.SelectedDate.Value)
                    : null;

                string department = CbLeaveDepartment.SelectedItem?.ToString();
                string status = (CbLeaveStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();

                var list = leaveService.GetLeaveRecords(name, date, department, status);

                DgLeaveReport.ItemsSource = list ?? new List<LeaveReport>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load bảng nghỉ phép: " + ex.Message);
            }
        }

        private void TbLeaveSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLoaded) LoadTable();
        }

        private void DpLeaveDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded) LoadTable();
        }

        private void CbLeaveDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded) LoadTable();
        }

        private void CbLeaveStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded) LoadTable();
        }

        // ---------------------------------------------------------
        // LOAD OVERVIEW (SUMMARY + DONUT CHART)
        // ---------------------------------------------------------
        public void LoadOverview()
        {
            try
            {
                int totalLeaveToday = leaveService.GetTotalLeaveToday();
                int annualLeave = leaveService.GetAnnualLeaveToday();
                int sickLeave = leaveService.GetSickLeaveToday();
                int unpaidLeave = leaveService.GetUnpaidLeaveToday();

                TbLeaveTotal.Text = totalLeaveToday.ToString();
                TbAnnualLeave.Text = annualLeave.ToString();
                TbSickLeave.Text = sickLeave.ToString();
                TbUnpaidLeave.Text = unpaidLeave.ToString();

                ChartLeaveDonut.Series = new ISeries[]
                {
                    new PieSeries<double>
                    {
                        Name = "Nghỉ phép năm",
                        Values = new double[]{ annualLeave },
                        Fill = new SolidColorPaint(SKColors.DeepSkyBlue),
                        InnerRadius = 10
                    },
                    new PieSeries<double>
                    {
                        Name = "Nghỉ ốm",
                        Values = new double[]{ sickLeave },
                        Fill = new SolidColorPaint(SKColors.MediumSeaGreen),
                        InnerRadius = 10
                    },
                    new PieSeries<double>
                    {
                        Name = "Từ chối",
                        Values = new double[]{ unpaidLeave },
                        Fill = new SolidColorPaint(SKColors.OrangeRed),
                        InnerRadius = 10
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load tổng quan nghỉ phép: " + ex.Message);
            }
        }

        // ---------------------------------------------------------
        // EXPORT EXCEL
        // ---------------------------------------------------------
        private void BtnExportLeave_Click(object sender, RoutedEventArgs e)
        {
            var list = leaveService.GetLeaveRecords(
                TbLeaveSearchName.Text,
                DpLeaveDate.SelectedDate.HasValue ? DateOnly.FromDateTime(DpLeaveDate.SelectedDate.Value) : null,
                (CbLeaveDepartment.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                (CbLeaveStatus.SelectedItem as ComboBoxItem)?.Content?.ToString()
            );

            ExportLeaveToExcel(list);
        }

        public void ExportLeaveToExcel(List<LeaveReport> data)
        {
            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.");
                return;
            }

            string defaultFileName = $"LeaveReport_{DateTime.Now:yyyyMMdd}.xlsx";

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel File (*.xlsx)|*.xlsx",
                FileName = defaultFileName
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Leave Report");

                    // HEADER
                    string[] headers =
                    {
                "Mã NV", "Họ tên", "Loại nghỉ", "Từ ngày", "Đến ngày",
                "Số ngày", "Lý do", "Trạng thái"
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cells[1, i + 1].Value = headers[i];
                    }

                    // STYLE HEADER
                    using (var range = ws.Cells[1, 1, 1, headers.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    // BODY
                    int row = 2;
                    foreach (var item in data)
                    {
                        ws.Cells[row, 1].Value = item.MaNV;
                        ws.Cells[row, 2].Value = item.TenNV;
                        ws.Cells[row, 3].Value = item.LoaiNghi;
                        ws.Cells[row, 4].Value = item.TuNgay.ToString("dd/MM/yyyy");
                        ws.Cells[row, 5].Value = item.DenNgay.ToString("dd/MM/yyyy");
                        ws.Cells[row, 6].Value = item.SoNgay;
                        ws.Cells[row, 7].Value = item.LyDo;
                        ws.Cells[row, 8].Value = item.TrangThai;
                        row++;
                    }

                    // AUTO WIDTH
                    ws.Cells.AutoFitColumns();

                    File.WriteAllBytes(dialog.FileName, package.GetAsByteArray());
                }

                MessageBox.Show("Xuất Excel thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel: " + ex.Message);
            }
        }
        
        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!tb.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tb.Focus();
            }
        }
    }
}
