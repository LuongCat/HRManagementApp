namespace HRManagementApp.BLL.statistics
{
    using HRManagementApp.DAL;

    public class DashboardStatisticService
    {
        private readonly StatisticRepository _statisticRepository = new StatisticRepository();

        // Overview
        // Total Employee Card

        public int GetTotalEmployee()
        {
            return _statisticRepository.CountEmployees();
        }

        public int GetEmployeesCountJoinedThisMonth()
        {
            return _statisticRepository.CountJoinedThisMonth();
        }

        // Present Today Card
        public int GetPresentToday()
        {
            return _statisticRepository.CountPresentToday();
        }

        public double CalcAttendanceRate(double present, double total)
        {
            if (total == 0) return 0;

            double rate = present / total * 100;
            return Math.Round(rate, 2);
        }

        // On Leave Card
        public int GetLeaveCount()
        {
            return _statisticRepository.CountLeave();
        }

        public int GetPendingApprovalsCount()
        {
            return _statisticRepository.CountPendingApprovals();
        }

        // Monthly Payroll Card
        public double GetMonthlyPayroll()
        {
            return _statisticRepository.CalcMonthlyPayroll();
        }

        public bool IsPaidMonthlyPayroll()
        {
            return _statisticRepository.IsPaidMonthlyPayroll();
        }

        public int GetDaysToDueDate()
        {
            DateTime today = DateTime.Today;
            DateTime dueDate = new DateTime(today.Year, today.Month, 10);

            return (dueDate - today).Days;
        }
    }
}