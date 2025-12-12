using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL.Report;
using HRManagementApp.models;
using System.IO;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Measure;
using SkiaSharp;

namespace HRManagementApp.UI.Views.Report
{
    public partial class DepartmentReportView : UserControl
    {
        private bool _isLoaded = false;
        public ISeries[] DepartmentEmployeeSeries { get; set; }
        public Axis[] DepartmentXAxes { get; set; }
        public Axis[] DepartmentYAxes { get; set; }
        public DepartmentReportView()
        {
            InitializeComponent();
        }

        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            DataContext = this;

            LoadOverview();

            LoadTable();
        }

        public void TxtSearchDepartment_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLoaded) LoadTable();
        }

        private void LoadOverview()
        {
            DepartmentReportService departmentReportService = new DepartmentReportService();
            var departmentsCount = departmentReportService.GetDepartmentsCount();
            TxtTotalDepartments.Text = departmentsCount.ToString();

            Dictionary<string, int> employeesCountByDepartment = departmentReportService.GetEmployeesCountByDepartment();
            CreateDepartmentChart(employeesCountByDepartment);
        }

        private void CreateDepartmentChart(Dictionary<string, int> departmentStats)
        {
            var labels = new List<string>();
            var values = new List<int>();

            foreach (var item in departmentStats)
            {
                labels.Add(item.Key);
                values.Add(item.Value);
            }

            DepartmentEmployeeSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = values.ToArray(),
                    Name = "Số nhân viên",
                    Fill = new SolidColorPaint(SKColors.SteelBlue),
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsSize = 14,
                    DataLabelsPosition = DataLabelsPosition.Top,
                    DataLabelsFormatter = p => p.Model.ToString()
                }
            };

            DepartmentXAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels.ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    TextSize = 12
                }
            };

            DepartmentYAxes = new Axis[]
            {
                new Axis
                {
                    MinStep = 1,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray)
                }
            };

            Raise(nameof(DepartmentEmployeeSeries));
            Raise(nameof(DepartmentXAxes));
            Raise(nameof(DepartmentYAxes));
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void Raise(string prop)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
        }

        public void LoadTable()
        {
            if (!_isLoaded) return;

            try
            {
                // ===== LẤY FILTER =====
                string name = txtSearchDepartment.Text?.Trim();   // tìm theo tên phòng ban

                DepartmentReportService departmentReportService = new DepartmentReportService();

                // ===== GỌI SERVICE =====
                var list = departmentReportService.GetDepartmentReports(name);

                // ===== ĐỔ VÀO DATAGRID =====
                DgDepartmentReport.ItemsSource = list ?? new List<DepartmentReport>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load bảng phòng ban: " + ex.Message);
            }
        }

        private void BtnExportDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded) ExportDepartmentReport();
        }

        private void ExportDepartmentReport()
        {
            var data = DgDepartmentReport.Items.Cast<DepartmentReport>().ToList();

            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.");
                return;
            }

            string defaultFileName = $"DepartmentReport_{DateTime.Now:yyyyMMdd}.xlsx";

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel File (*.xlsx)|*.xlsx",
                FileName = defaultFileName
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Department Report");

                    // HEADER
                    string[] headers =
                    {
                        "Mã PB", "Tên phòng ban", "Trưởng phòng", "Số nhân viên"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cells[1, i + 1].Value = headers[i];
                    }

                    // STYLE HEADER
                    using (var range = ws.Cells[1, 1, 1, headers.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    // BODY
                    int row = 2;

                    foreach (var item in data)
                    {
                        ws.Cells[row, 1].Value = item.MaPB;
                        ws.Cells[row, 2].Value = item.TenPB;
                        ws.Cells[row, 3].Value = item.TruongPhong;
                        ws.Cells[row, 4].Value = item.SoNhanVien;

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