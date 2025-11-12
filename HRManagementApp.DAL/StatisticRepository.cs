using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL
{
    public class StatisticRepository
    {
        public int CountEmployees()
        {
            string query = "SELECT COUNT(*) as TotalEmployee FROM NhanVien";
            object result = Database.ExecuteScalar(query);
            
            return  Convert.ToInt32(result);
        }

        public int CountJoinedThisMonth()
        {
            string query = "SELECT COUNT(*) as TotalEmployee FROM NhanVien " +
                           "WHERE NgayVaoLam >= DATE_FORMAT(CURDATE(), '%Y-%m-01') " +
                           "AND NgayVaoLam < DATE_ADD(DATE_FORMAT(CURDATE(), '%Y-%m-01'), INTERVAL 1 MONTH)";

            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public int CountPresentToday()
        {
            string query = "SELECT COUNT(*) as PresentToday FROM chamcong " +
                           "WHERE Ngay = CURDATE() ";

            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }
    }
}