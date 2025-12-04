using System;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using Microsoft.Win32; // Cho SaveFileDialog

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
        
    }
}