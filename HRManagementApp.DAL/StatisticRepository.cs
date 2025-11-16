using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL
{
    public class StatisticRepository
    {
        // Dashboard - Overview 
        // Total Employee Card
        public int CountEmployees()
        {
            string query = "SELECT COUNT(*) as TotalEmployee FROM NhanVien";
            object result = Database.ExecuteScalar(query);

            return Convert.ToInt32(result);
        }

        public int CountJoinedThisMonth()
        {
            string query = "SELECT COUNT(*) as TotalEmployee FROM NhanVien " +
                           "WHERE NgayVaoLam >= DATE_FORMAT(CURDATE(), '%Y-%m-01') " +
                           "AND NgayVaoLam < DATE_ADD(DATE_FORMAT(CURDATE(), '%Y-%m-01'), INTERVAL 1 MONTH)";

            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        // Present Today Card
        public int CountPresentToday()
        {
            string query = "SELECT COUNT(DISTINCT MaNV) as PresentToday FROM chamcong " +
                           "WHERE Ngay = CURDATE() ";

            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        // On Leave Card
        public int CountLeave()
        {
            string query = "SELECT COUNT(*) as LeaveCount FROM dontu " +
                           "WHERE TrangThai = 'Đã duyệt' " +
                           "AND NOW() BETWEEN NgayBatDau AND NgayKetThuc";

            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public int CountPendingApprovals()
        {
            string query = "SELECT COUNT(*) as PendingApprovalsCount FROM dontu " +
                           "WHERE TrangThai = 'Chưa duyệt'";

            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        // Monthly Payroll Card
        public double CalcMonthlyPayroll()
        {
            string query =  "SELECT SUM(LuongThucNhan) AS TotalPayroll " +
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