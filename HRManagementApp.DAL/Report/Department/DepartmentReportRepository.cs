using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL.Report
{
    public class DepartmentReportRepository
    {
        public string[] GetDepartmentNames()
        {
            string query = "SELECT TenPB FROM phongban ORDER BY MaPB ASC";

            DataTable dt = Database.ExecuteQuery(query);

            if (dt.Rows.Count == 0)
                return Array.Empty<string>();

            return dt.AsEnumerable()
                     .Select(row => row["TenPB"].ToString()!)
                     .ToArray();
        }

        public Dictionary<string, int> GetEmployeesCountByDepartment()
        {
            string query = @"
                SELECT 
                    pb.TenPB,
                    COUNT(nv.MaNV) AS SoNhanVien
                FROM phongban pb
                LEFT JOIN nhanvien nv ON pb.MaPB = nv.MaPB
                GROUP BY pb.TenPB
                ORDER BY pb.TenPB;
            ";

            DataTable dt = Database.ExecuteQuery(query);

            var result = new Dictionary<string, int>();

            foreach (DataRow row in dt.Rows)
            {
                string tenPB = row["TenPB"]?.ToString() ?? "Không xác định";
                int soNV = row["SoNhanVien"] == DBNull.Value ? 0 : Convert.ToInt32(row["SoNhanVien"]);

                result.Add(tenPB, soNV);
            }

            return result;
        }

        public int CountDepartments()
        {
            string query = "SELECT COUNT(*) as TotalDepartments FROM phongban";
            object result = Database.ExecuteScalar(query);

            return Convert.ToInt32(result);
        }

        public DataTable GetDepartmentReports(string? name)
        {
            string query = @"
                SELECT 
                    pb.MaPB,
                    pb.TenPB,

                    tp.HoTen AS TruongPhong,

                    (
                        SELECT COUNT(*)
                        FROM nhanvien nv
                        WHERE nv.MaPB = pb.MaPB AND nv.TrangThai = 'Còn làm việc'
                    ) AS SoNhanVien

                FROM phongban pb
                LEFT JOIN nhanvien tp ON pb.MaTruongPhong = tp.MaNV

                WHERE 1 = 1
            ";

            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query += " AND (pb.TenPB LIKE @name OR pb.MaPB LIKE @name)";
                parameters.Add("@name", $"%{name}%");
            }

            return Database.ExecuteQuery(query, parameters);
        }
    }
}