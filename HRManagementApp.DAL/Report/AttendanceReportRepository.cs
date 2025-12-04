using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL.Report
{
    public class AttendanceReportRepository
    {
        public DataTable GetAttendanceRecord(string? name,
                                             DateTime? date,
                                             string? department,
                                             string? status)
        {
            string query = @"
            SELECT 
                nv.MaNV,
                nv.HoTen,
                cc.Ngay,
                cc.CheckIn,
                cc.CheckOut,
                CASE 
                    WHEN cc.CheckIn IS NULL THEN 'Absent'
                    WHEN cc.CheckIn > '08:00:00' THEN 'Late'
                    WHEN cc.CheckOut < '16:00:00' THEN 'Left Early'
                    ELSE 'On Time'
                END AS Status,
                nv.PhongBan
            FROM chamcong cc
            INNER JOIN nhanvien nv ON cc.MaNV = nv.MaNV
            WHERE 1=1";

            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query += " AND nv.HoTen LIKE @name";
                parameters.Add("@name", $"%{name}%");
            }

            if (date.HasValue)
            {
                query += " AND cc.Ngay = @date";
                parameters.Add("@date", date.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query += " AND nv.PhongBan = @department";
                parameters.Add("@department", department);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query += @"
                AND CASE 
                        WHEN cc.CheckIn IS NULL THEN 'Absent'
                        WHEN cc.CheckIn > '08:00:00' THEN 'Late'
                        WHEN cc.CheckOut < '16:00:00' THEN 'Left Early'
                        ELSE 'On Time'
                    END = @status";
                parameters.Add("@status", status);
            }

            return Database.ExecuteQuery(query, parameters);
        }
    }

}