using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL.Report
{
    public class AttendanceReportRepository
    {
        public DataTable GetAttendanceRecords(string? name,
                                             DateOnly? date,
                                             string? department,
                                             string? status)
        {
            string query = @"
            SELECT
                nv.MaNV,
                nv.HoTen,
                pb.TenPB,
                cc.Ngay,
                cc.GioVao,
                cc.GioRa,
                ca.TenCa,
                CASE
                    WHEN cc.GioVao > ca.GioBatDau THEN 'Đi trễ'
                    ELSE 'Đúng giờ'
                END AS TrangThai
            FROM chamcong cc
            JOIN nhanvien nv ON cc.MaNV = nv.MaNV
            JOIN phongban pb ON nv.MaPB = pb.MaPB
            JOIN calam ca ON ca.MaCa = COALESCE((
                    SELECT nvc.MaCa
                    FROM nhanvien_calam nvc
                    JOIN calam c2 ON c2.MaCa = nvc.MaCa
                    WHERE nvc.MaNV = nv.MaNV
                        AND c2.GioKetThuc >= cc.GioVao
                    ORDER BY
                        (cc.GioVao BETWEEN c2.GioBatDau AND c2.GioKetThuc) DESC,
                        CASE WHEN c2.GioBatDau >= cc.GioVao THEN 0 ELSE 1 END,
                        ABS(TIME_TO_SEC(TIMEDIFF(c2.GioBatDau, cc.GioVao))) ASC
                    LIMIT 1
            ), '0')
            WHERE cc.GioVao IS NOT NULL
            ";

            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query += " AND (nv.HoTen LIKE @name OR nv.MaNV LIKE @name)";
                parameters.Add("@name", $"%{name}%");
            }

            if (date.HasValue)
            {
                query += " AND cc.Ngay = @date";
                parameters.Add("@date", date.Value.ToDateTime(TimeOnly.MinValue));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query += " AND pb.TenPB = @department";
                parameters.Add("@department", department);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query += @"
                AND CASE 
                        WHEN cc.GioVao > ca.GioBatDau THEN 'Đi trễ'
                        ELSE 'Đúng giờ'
                    END = @status";
                parameters.Add("@status", status);
            }
            
            query += @" ORDER BY cc.GioVao";

            return Database.ExecuteQuery(query, parameters);
        }
        public int CountPresentToday()
        {
            string query = "SELECT COUNT(DISTINCT MaNV) as PresentToday FROM chamcong " +
                            "WHERE Ngay = CURDATE() ";

            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public int CountLateToday()
        {
            string query = @"SELECT
                COUNT(*) as TotalLateToday
                FROM chamcong cc
                JOIN nhanvien nv ON cc.MaNV = nv.MaNV
                JOIN calam ca ON ca.MaCa = (
                        SELECT nvc.MaCa
                        FROM nhanvien_calam nvc
                        JOIN calam c2 ON c2.MaCa = nvc.MaCa
                        WHERE nvc.MaNV = nv.MaNV
                            AND c2.GioKetThuc >= cc.GioVao
                        ORDER BY
                            (cc.GioVao BETWEEN c2.GioBatDau AND c2.GioKetThuc) DESC,

                            CASE WHEN c2.GioBatDau >= cc.GioVao THEN 0 ELSE 1 END,

                            ABS(TIME_TO_SEC(TIMEDIFF(c2.GioBatDau, cc.GioVao))) ASC

                        LIMIT 1
                )
                WHERE cc.GioVao IS NOT NULL 
                AND cc.Ngay = CURDATE() 
                AND cc.GioVao > ca.GioBatDau";
            object result = Database.ExecuteScalar(query);

            return Convert.ToInt32(result);
        }
    }
}