using HRManagementApp.BLL;
using HRManagementApp.models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HRManagementApp.UI.Views
{
    public partial class MyPayslipView : UserControl
    {
        private MyPayslipBLL _bll = new MyPayslipBLL();
        private bool _isLoaded = false;

        public MyPayslipView()
        {
            InitializeComponent();
            LoadAvailableMonths();
        }

        private void LoadAvailableMonths()
        {
            if (!UserSession.IsLoggedIn || UserSession.MaNV == null)
            {
                MessageBox.Show("Vui lòng đăng nhập bằng tài khoản nhân viên.");
                return;
            }

            // Lấy danh sách tháng có lương của nhân viên này
            List<string> months = _bll.GetMonthList(UserSession.MaNV.Value);
            
            cboMonth.ItemsSource = months;
            String nowMonths = months[0];
            if (months.Count > 0)
            {
                cboMonth.SelectedIndex = 0; // Chọn tháng mới nhất
            }
            else
            {
                txtNoData.Visibility = Visibility.Visible;
                pnlPayslipContent.Visibility = Visibility.Collapsed;
                txtNoData.Text = "Chưa có dữ liệu lương nào.";
            }

            LoadPayslipData(nowMonths);
            _isLoaded = true;
        }

        private void CboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded || cboMonth.SelectedItem == null) return;

            string selectedMonth = cboMonth.SelectedItem.ToString();
            LoadPayslipData(selectedMonth);
        }

        private void LoadPayslipData(string monthStr)
        {
            var dto = _bll.GetPayslip(UserSession.MaNV.Value, monthStr);

            if (dto != null)
            {
                // Binding dữ liệu vào UI
                pnlPayslipContent.DataContext = dto;
                
                // Xử lý hiển thị phần Kiêm nhiệm
                grdKiemNhiem.Visibility = dto.CoKiemNhiem ? Visibility.Visible : Visibility.Collapsed;

                // Xử lý màu sắc badge trạng thái
                if (dto.TrangThai == "Đã trả")
                {
                    badgeStatus.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(209, 250, 229)); // Xanh lá nhạt
                    ((TextBlock)badgeStatus.Child).Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(5, 150, 105));
                }
                else
                {
                    badgeStatus.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 226, 226)); // Đỏ nhạt
                    ((TextBlock)badgeStatus.Child).Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(220, 38, 38));
                    ((TextBlock)badgeStatus.Child).Text = "Chưa Thanh Toán";
                }

                pnlPayslipContent.Visibility = Visibility.Visible;
                txtNoData.Visibility = Visibility.Collapsed;
            }
            else
            {
                pnlPayslipContent.Visibility = Visibility.Collapsed;
                txtNoData.Visibility = Visibility.Visible;
            }
        }
    }
}