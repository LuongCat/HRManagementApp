using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace HRManagementApp.UI.Views
{
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();
            DefaultReport.IsChecked = true;
        }

        private void ReportMenu_Checked(object sender, RoutedEventArgs e)
        {
            var clicked = sender as ToggleButton;

            // Uncheck other toggle buttons
            foreach (var child in ReportMenuPanel.Children)
            {
                if (child is ToggleButton btn && btn != clicked)
                    btn.IsChecked = false;
            }

            switch (clicked.Tag.ToString())
            {
                case "Employee":
                    ReportContentArea.Content = new Report.EmployeeReportView();
                    break;

                case "Attendance":
                    ReportContentArea.Content = new Report.AttendanceReportView();
                    break;

                case "Payroll":
                    ReportContentArea.Content = new Report.PayrollReportView();
                    break;

                case "Leave":
                    ReportContentArea.Content = new Report.LeaveReportView();
                    break;

                case "Department":
                    ReportContentArea.Content = new Report.DepartmentReportView();
                    break;
            }
        }
    }
}