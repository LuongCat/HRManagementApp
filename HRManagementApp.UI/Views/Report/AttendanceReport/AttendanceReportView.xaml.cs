using System.Windows;
using System.Windows.Controls;
using HRManagementApp.models;
using HRManagementApp.BLL.statistics;

namespace HRManagementApp.UI.Views.Report
{
    public partial class AttendanceReportView : UserControl
    {
        public readonly AttendanceReportService attendanceReportService = new AttendanceReportService();
        public AttendanceReportView()
        {
            InitializeComponent();
        }

        public void LoadTable()
        {

        }

        public List<ChamCong> GetAttendanceRecord()
        {
            string department = (CbAttendanceReportDepartment.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string status = (CbAttendanceReportStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();
            
            return attendanceReportService.GetAttendanceRecord(
                TbAttendanceReportSearchName.Text,
                DpAttendanceReportDate.SelectedDate,
                department,
                status);
        }
    }
}