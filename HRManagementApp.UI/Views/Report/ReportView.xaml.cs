using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using HRManagementApp.UI.Views.Report; // Đảm bảo namespace này đúng với folder chứa các View con

namespace HRManagementApp.UI.Views
{
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();

            // Kích hoạt tab mặc định khi mở trang
            DefaultReport.IsChecked = true;
        }

        private void ReportMenu_Checked(object sender, RoutedEventArgs e)
        {
            var clickedBtn = sender as ToggleButton;

            if (clickedBtn == null) return;

            // Logic: Tắt các nút khác để chỉ có 1 nút được Active
            foreach (var child in ReportMenuPanel.Children)
            {
                if (child is ToggleButton btn && btn != clickedBtn)
                {
                    btn.IsChecked = false;
                }
            }

            // Chống việc click lại vào nút đang active thì bị uncheck (giữ nó luôn active)
            clickedBtn.IsChecked = true;

            // Load View tương ứng
            switch (clickedBtn.Tag.ToString())
            {
                case "Attendance":
                    ReportContentArea.Content = new AttendanceReportView();
                    break;
                case "Payroll":
                    ReportContentArea.Content = new PayrollReportView();
                    break;
                case "Leave":
                    ReportContentArea.Content = new LeaveReportView();
                    break;
                case "Employee":
                    ReportContentArea.Content = new EmployeeReportView();
                    break;
                case "Department":
                    ReportContentArea.Content = new DepartmentReportView();
                    break;
            }
        }
    }
}