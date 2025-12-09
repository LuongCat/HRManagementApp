using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL.Report
{
    public class StatisticRepository
    {
        // Monthly Payroll Card
        public double CalcMonthlyPayroll()
        {
            string query = "SELECT SUM(LuongThucNhan) AS TotalPayroll " +
                            "FROM luong " +
                            "WHERE (Thang = MONTH(DATE_SUB(CURDATE(), INTERVAL 1 MONTH)) " +
                            "AND Nam = YEAR(DATE_SUB(CURDATE(), INTERVAL 1 MONTH)))";

            object result = Database.ExecuteScalar(query);

            if (result == null || result == DBNull.Value)
                return 0.0;

            return Convert.ToDouble(result);
        }

        public bool IsPaidMonthlyPayroll()
        {
            string query = "SELECT EXISTS( " +
                                "SELECT 1 " +
                                "FROM luong " +
                                "WHERE Thang = MONTH(DATE_SUB(CURDATE(), INTERVAL 1 MONTH)) " +
                                "AND Nam = YEAR(DATE_SUB(CURDATE(), INTERVAL 1 MONTH)) " +
                                "AND TrangThai = 'Chưa trả' " +
                                ")";

            object result = Database.ExecuteScalar(query);

            return Convert.ToInt32(result) == 0;
        }
    
    }
}