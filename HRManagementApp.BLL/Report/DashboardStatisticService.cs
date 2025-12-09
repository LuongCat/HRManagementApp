namespace HRManagementApp.BLL.Report
{
    using HRManagementApp.DAL.Report;

    public class DashboardStatisticService
    {
        private readonly StatisticRepository _statisticRepository = new StatisticRepository();
        private readonly AttendanceReportRepository _attendanceRepository = new AttendanceReportRepository();
        private readonly EmployeeDAL _employeeRepository = new EmployeeDAL();
        private readonly LeaveReportRepository _leaveRepository = new LeaveReportRepository();
        // Overview
        // Total Employee Card

        public int GetTotalEmployee()
        {
            return _employeeRepository.CountEmployees();
        }

        public int GetEmployeesCountJoinedThisMonth()
        {
            return _employeeRepository.CountJoinedThisMonth();
        }

        // Present Today Card
        public int GetPresentToday()
        {
            return _attendanceRepository.CountPresentToday();
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
            return _leaveRepository.CountLeave();
        }

        public int GetPendingApprovalsCount()
        {
            return _leaveRepository.CountPendingApprovals();
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