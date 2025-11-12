using System.Windows.Controls;
using HRManagementApp.BLL.statistics;

namespace HRManagementApp.UI.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            LoadOverviewStatistics();
        }

        public void LoadOverviewStatistics()
        {
            var statisticService = new DashboardStatisticService();
            
            int totalEmployees = statisticService.GetTotalEmployee();
            int joinedThisMonth = statisticService.GetEmployeesCountJoinedThisMonth();
            
            TxtTotalEmployee.Text = totalEmployees.ToString();
            TxtJoinedThisMonth.Text = joinedThisMonth + " joined this month";

            int presentToday = statisticService.GetPresentToday();
            double attendanceRate = statisticService.CalcAttendanceRate((double)presentToday, (double)totalEmployees);

            TxtPresentToday.Text = presentToday.ToString();
            TxtAttendanceRate.Text = attendanceRate + "% attendance";


        }
    }
}