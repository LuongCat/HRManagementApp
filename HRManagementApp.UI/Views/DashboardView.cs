using System.Windows.Controls;
using System.Windows;
using HRManagementApp.BLL.Report;

namespace HRManagementApp.UI.Views
{
    public partial class DashboardView : UserControl
    {
        private readonly DashboardStatisticService statisticService = new DashboardStatisticService();

        // Quick Action Events
        public event Action? AddNewEmployeeRequested;

        public DashboardView()
        {
            InitializeComponent();
            LoadOverviewStatistics();
        }

        public void LoadOverviewStatistics()
        {
            TxtToday.Text = "Today: " + DateTime.Now.ToString("MMMM dd, yyyy");

            // Total Employee Card 
            int totalEmployees = statisticService.GetTotalEmployee();
            int joinedThisMonth = statisticService.GetEmployeesCountJoinedThisMonth();

            TxtTotalEmployee.Text = totalEmployees.ToString();
            TxtJoinedThisMonth.Text = joinedThisMonth + " joined this month";

            // Present Today Card
            int presentToday = statisticService.GetPresentToday();
            double attendanceRate = statisticService.CalcAttendanceRate((double)presentToday, (double)totalEmployees);

            TxtPresentToday.Text = presentToday.ToString();
            TxtAttendanceRate.Text = attendanceRate + "% attendance";

            // On Leave Card
            int onLeaveCount = statisticService.GetLeaveCount();
            int pendingApprovalsCount = statisticService.GetPendingApprovalsCount();

            TxtOnLeave.Text = onLeaveCount.ToString();
            TxtPendingApprovals.Text = pendingApprovalsCount + (pendingApprovalsCount == 1 ? " pending approval" : " pending approvals");

            // Monthly Payroll Card
            double monthlyPayroll = statisticService.GetMonthlyPayroll();
            TxtMonthlyPayroll.Text = (monthlyPayroll / 1000).ToString("0.#") + "k";

            bool isPaid = Math.Round(monthlyPayroll, 2) == 0.0 || statisticService.IsPaidMonthlyPayroll();
            if (isPaid)
            {
                TxtDueDate.Text = "Paid";
            }
            else
            {
                int DaysToDueDate = statisticService.GetDaysToDueDate();

                TxtDueDate.Text =
                    DaysToDueDate < 0
                        ? "Overdue by " + (DaysToDueDate * -1) + (DaysToDueDate == -1 ? " day" : " days")
                        : DaysToDueDate == 0
                            ? "Due today"
                            : "Due in " + DaysToDueDate + (DaysToDueDate == 1 ? " day" : " days");
            }
        }

        public void GoToAddNewEmployee(object sender, RoutedEventArgs e)
        {
            AddNewEmployeeRequested?.Invoke();
        }
    }
}