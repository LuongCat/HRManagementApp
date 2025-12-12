using System.Windows;
using System.Windows.Controls;
using HRManagementApp.models;
using HRManagementApp.BLL.Report;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace HRManagementApp.UI.Views.Report
{
    public partial class AttendanceReportView : UserControl
    {
        private bool _isLoaded = false;

        public readonly AttendanceReportService attendanceReportService = new AttendanceReportService();

        public AttendanceReportView()
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

            CbAttendanceReportDepartment.ItemsSource = list;
            CbAttendanceReportDepartment.SelectedIndex = 0;

            LoadTable();
        }

        public void LoadTable()
        {
            if (!_isLoaded) return;

            try
            {
                var list = GetAttendanceRecords();

                DgAttendanceReport.ItemsSource = list ?? new List<ChamCongReport>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table: " + ex.Message);
            }
        }

        public List<ChamCongReport> GetAttendanceRecords()
        {
            string department = CbAttendanceReportDepartment.SelectedItem?.ToString();
            string status = (CbAttendanceReportStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();

            return attendanceReportService.GetAttendanceRecords(
                TbAttendanceReportSearchName.Text,
                DateOnly.FromDateTime(DpAttendanceReportDate.SelectedDate.Value),
                department,
                status);
        }

        private void TbAttendanceReportSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded) return;
            LoadTable();
        }

        private void DpAttendanceReportDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            LoadTable();
        }

        private void CbAttendanceReportDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            LoadTable();
        }

        private void CbAttendanceReportStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            LoadTable();
        }

        private void LoadOverview()
        {
            AttendanceReportService attendanceService = new AttendanceReportService();
            EmployeeReportBLL employeeReportService = new EmployeeReportBLL();

            var totalEmployees = employeeReportService.GetTotalEmployee();
            var lateToday = attendanceReportService.GetLateToday();
            var presentToday = attendanceReportService.GetPresentToday();

            var absentToday = totalEmployees - presentToday;

            TbSummaryPresent.Text = presentToday.ToString();
            TbSummaryLate.Text = lateToday.ToString();
            TbSummaryAbsent.Text = absentToday.ToString();

            double ontimePercent = (double)(presentToday - lateToday) * 100 / totalEmployees;
            double latePercent = (double)lateToday * 100 / totalEmployees;
            double absentPercent = (double)absentToday * 100 / totalEmployees;

            ChartAttendanceDonut.Series = new ISeries[]
            {
                new PieSeries<double>
                {
                    Name = "Đúng giờ (%)",
                    Values = new double[]{ Math.Round(ontimePercent,1) },
                    InnerRadius = 0,
                    Fill = new SolidColorPaint(SKColors.DeepSkyBlue),
                },
                new PieSeries<double>
                {
                    Name = "Đi trễ (%)",
                    Values = new double[]{ Math.Round(latePercent,1) },
                    InnerRadius = 0,
                    Fill = new SolidColorPaint(SKColors.Goldenrod),
                },
                new PieSeries<double>
                {
                    Name = "Nghỉ (%)",
                    Values = new double[]{ Math.Round(absentPercent,1) },
                    InnerRadius = 0,
                    Fill = new SolidColorPaint(SKColors.OrangeRed),
                }
            };
        }

        public void ExportAttendanceToExcel(List<ChamCongReport> data)
        {
            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.");
                return;
            }

            // Tạo sẵn tên file
            string defaultFileName = $"AttendanceReport_{DateTime.Now:yyyyMMdd}.xlsx";

            string filePath = "";

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel File|*.xlsx",
                Title = "Xuất báo cáo chấm công",
                FileName = defaultFileName   // ← đặt tên mặc định
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                filePath = saveFileDialog.FileName;
            }
            else
            {
                return;
            }

            try
            {
                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Attendance Report");

                    // TIÊU ĐỀ CỘT
                    ws.Cells[1, 1].Value = "Mã NV";
                    ws.Cells[1, 2].Value = "Tên nhân viên";
                    ws.Cells[1, 3].Value = "Phòng ban";
                    ws.Cells[1, 4].Value = "Ngày";
                    ws.Cells[1, 5].Value = "Giờ vào";
                    ws.Cells[1, 6].Value = "Giờ ra";
                    ws.Cells[1, 7].Value = "Tên ca";
                    ws.Cells[1, 8].Value = "Trạng thái";

                    using (var range = ws.Cells[1, 1, 1, 8])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // ĐỔ DỮ LIỆU
                    int row = 2;
                    foreach (var item in data)
                    {
                        ws.Cells[row, 1].Value = item.MaNV;
                        ws.Cells[row, 2].Value = item.TenNV;
                        ws.Cells[row, 3].Value = item.TenPB;
                        ws.Cells[row, 4].Value = item.Ngay?.ToString("yyyy-MM-dd");
                        ws.Cells[row, 5].Value = item.GioVao?.ToString(@"hh\:mm") ?? "";
                        ws.Cells[row, 6].Value = item.GioRa?.ToString(@"hh\:mm") ?? "";
                        ws.Cells[row, 7].Value = item.TenCa;
                        ws.Cells[row, 8].Value = item.TrangThai;

                        row++;
                    }

                    ws.Cells.AutoFitColumns();

                    // Lưu file
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                }

                MessageBox.Show("Xuất Excel thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel: " + ex.Message);
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var list = GetAttendanceRecords();
            ExportAttendanceToExcel(list);
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
