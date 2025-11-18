namespace HRManagementApp.BLL.statistics
{
    using HRManagementApp.DAL;

    public class DashboardStatisticService
    {
        private readonly StatisticRepository _statisticRepository = new StatisticRepository();

        public int GetTotalEmployee()
        {
            return _statisticRepository.CountEmployees();
        }

        public int GetEmployeesCountJoinedThisMonth()
        {
            return _statisticRepository.CountJoinedThisMonth();
        }

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
    }
}