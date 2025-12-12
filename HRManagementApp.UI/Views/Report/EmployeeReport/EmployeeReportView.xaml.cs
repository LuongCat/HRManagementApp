using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Measure;
using SkiaSharp;
using HRManagementApp.BLL.Report;
using HRManagementApp.models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Windows.Input;

namespace HRManagementApp.UI.Views.Report
{
    public partial class EmployeeReportView : UserControl
    {
        private bool _isLoaded = false;
        public ISeries[] AgeSeries { get; set; }
        public Axis[] AgeXAxes { get; set; }
        public Axis[] AgeYAxes { get; set; }

        public ISeries[] GenderSeries { get; set; }

        public EmployeeReportView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            DataContext = this;

            DepartmentReportService departmentReportService = new DepartmentReportService();
            string[] departmentNames = departmentReportService.GetDepartmentNames();

            var departmentList = new List<string> { "Tất cả" };
            departmentList.AddRange(departmentNames);

            CbEmployeeDepartment.ItemsSource = departmentList;
            CbEmployeeDepartment.SelectedIndex = 0;

            EmployeeReportBLL epmloyeeReportBLL = new EmployeeReportBLL();
            string[] positionNames = epmloyeeReportBLL.GetPositionNames();

            var positionList = new List<string> { "Tất cả" };
            positionList.AddRange(positionNames);

            CbEmployeePosition.ItemsSource = positionList;
            CbEmployeePosition.SelectedIndex = 0;

            LoadOverview();

            LoadTable();
        }

        private void LoadOverview()
        {
            try
            {
                // =============================
                //        CARD PLACEHOLDER
                // =============================
                EmployeeReportBLL employeeReportBLL = new EmployeeReportBLL();
                var totalEmployee = employeeReportBLL.GetTotalEmployee();
                var newEmployee = employeeReportBLL.GetEmployeesCountJoinedThisMonth();
                var averageAge = employeeReportBLL.GetAverageAge();

                txtTotalEmp.Text = totalEmployee.ToString();
                txtNewEmp.Text = newEmployee.ToString();
                txtAverageAge.Text = averageAge.ToString();

                // =============================
                //         AGE CHART DEMO
                // =============================
                var AgeStats = employeeReportBLL.GetAgeStats();

                CreateAgeChart(AgeStats);

                // =============================
                //     GENDER RATIO DEMO
                // =============================


                var (male, female) = employeeReportBLL.GetGenderStats();
                CreateGenderChart(male, female);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load placeholder: " + ex.Message);
            }
        }

        // ============================================
        //                 AGE CHART
        // ============================================
        private void CreateAgeChart(Dictionary<string, int> ageStats)
        {
            var labels = new List<string>();
            var values = new List<int>();

            foreach (var item in ageStats)
            {
                labels.Add(item.Key);
                values.Add(item.Value);
            }

            AgeSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = values.ToArray(),
                    Name = "Số lượng",
                    Fill = new SolidColorPaint(SKColors.SteelBlue),
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsSize = 14,
                    DataLabelsPosition = DataLabelsPosition.Top,
                    DataLabelsFormatter = p => p.Model.ToString()
                }
            };

            AgeXAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels.ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    TextSize = 12
                }
            };

            AgeYAxes = new Axis[]
            {
                new Axis
                {
                    MinStep = 1,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray)
                }
            };

            Raise(nameof(AgeSeries));
            Raise(nameof(AgeXAxes));
            Raise(nameof(AgeYAxes));
        }

        // ============================================
        //              GENDER PIE CHART
        // ============================================
        private void CreateGenderChart(int male, int female)
        {
            var list = new List<ISeries>();

            if (male > 0)
            {
                list.Add(new PieSeries<int>
                {
                    Name = "Nam",
                    Values = new[] { male },
                    Fill = new SolidColorPaint(SKColors.RoyalBlue),
                });
            }

            if (female > 0)
            {
                list.Add(new PieSeries<int>
                {
                    Name = "Nữ",
                    Values = new[] { female },
                    Fill = new SolidColorPaint(SKColors.HotPink),
                });
            }

            GenderSeries = list.ToArray();
            Raise(nameof(GenderSeries));
        }

        // ============================================
        //      NotifyPropertyChanged Binding
        // ============================================
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void Raise(string prop)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
        }

        private void TbEmployeeSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLoaded) LoadTable();
        }

        private void CbEmployeeDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded) LoadTable();
        }

        private void CbEmployeeGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded) LoadTable();
        }

        private void CbEmployeePosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded) LoadTable();
        }

        private void CbEmployeeStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded) LoadTable();
        }

        private void BtnExportEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded) ExportEmployeeReport();
        }

        public void LoadTable()
        {
            if (!_isLoaded) return;

            try
            {
                // ===== LẤY FILTER =====
                string name = TbEmployeeSearchName.Text?.Trim();

                string department = CbEmployeeDepartment.SelectedItem?.ToString();

                string gender = (CbEmployeeGender.SelectedItem as ComboBoxItem)?.Content?.ToString()
                                ?? CbEmployeeGender.SelectedItem?.ToString();

                string position = CbEmployeePosition.SelectedItem?.ToString();

                string status = (CbEmployeeStatus.SelectedItem as ComboBoxItem)?.Content?.ToString()
                                ?? CbEmployeeStatus.SelectedItem?.ToString();

                EmployeeReportBLL employeeReportBLL = new EmployeeReportBLL();
                // ===== GỌI SERVICE =====
                var list = employeeReportBLL.GetEmployeeReports(
                    name,
                    department,
                    gender,
                    position,
                    status
                );

                // ===== ĐỔ VÀO DATAGRID =====
                DgEmployeeReport.ItemsSource = list ?? new List<EmployeeReport>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load bảng nhân viên: " + ex.Message);
            }
        }

        private void ExportEmployeeReport()
        {
            // LẤY DỮ LIỆU TỪ DATAGRID
            var data = DgEmployeeReport.Items.Cast<EmployeeReport>().ToList();

            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.");
                return;
            }

            string defaultFileName = $"EmployeeReport_{DateTime.Now:yyyyMMdd}.xlsx";

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel File (*.xlsx)|*.xlsx",
                FileName = defaultFileName
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Employee Report");

                    // HEADER
                    string[] headers =
                    {
                        "Mã NV", "Họ tên", "Giới tính", "Ngày sinh",
                        "CCCD", "Điện thoại", "Phòng ban", "Chức vụ",
                        "Ngày vào làm", "Trạng thái"
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
                        ws.Cells[row, 2].Value = item.HoTen;
                        ws.Cells[row, 3].Value = item.GioiTinh;
                        ws.Cells[row, 4].Value = item.NgaySinh?.ToString("dd/MM/yyyy");

                        ws.Cells[row, 5].Value = item.SoCCCD;
                        ws.Cells[row, 6].Value = item.DienThoai;

                        ws.Cells[row, 7].Value = item.PhongBan;
                        ws.Cells[row, 8].Value = item.ChucVu;

                        ws.Cells[row, 9].Value = item.NgayVaoLam?.ToString("dd/MM/yyyy");
                        ws.Cells[row, 10].Value = item.TrangThai;

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
