using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL.Report
{
    public class EmployeeDAL
    {
        public Dictionary<string, int> GetEmployeeByDepartment()
        {
            var result = new Dictionary<string, int>();

            string query = @"
                SELECT pb.TenPB, COUNT(nv.MaNV) as SoLuong
                FROM phongban pb
                LEFT JOIN nhanvien nv ON pb.MaPB = nv.MaPB AND nv.TrangThai = 'Còn làm việc'
                GROUP BY pb.TenPB";

            DataTable dt = Database.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                string tenPB = row["TenPB"] != DBNull.Value ? row["TenPB"].ToString() : "Chưa phân bổ";
                int soLuong = row["SoLuong"] != DBNull.Value ? Convert.ToInt32(row["SoLuong"]) : 0;

                if (!result.ContainsKey(tenPB))
                {
                    result.Add(tenPB, soLuong);
                }
            }

            return result;
        }

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

        public int CalcAverageAge()
        {
            string query = @"
                SELECT AVG(TIMESTAMPDIFF(YEAR, NgaySinh, CURDATE()))
                FROM nhanvien
                WHERE NgaySinh IS NOT NULL;
            ";

            object result = Database.ExecuteScalar(query);

            if (result == DBNull.Value) return 0;

            double avgAge = Convert.ToDouble(result);

            return (int)Math.Ceiling(avgAge);
        }

        public Dictionary<string, int> GetAgeStats()
        {
            Dictionary<string, int> ageStats = new Dictionary<string, int>()
            {
                { "18 - 25", 0 },
                { "26 - 30", 0 },
                { "31 - 40", 0 },
                { "41 - 50", 0 },
                { "50+", 0 }
            };

            string query = @"
                SELECT AgeGroup, COUNT(*) AS Total
                FROM (
                    SELECT 
                        CASE
                            WHEN TIMESTAMPDIFF(YEAR, NgaySinh, CURDATE()) BETWEEN 18 AND 25 THEN '18 - 25'
                            WHEN TIMESTAMPDIFF(YEAR, NgaySinh, CURDATE()) BETWEEN 26 AND 30 THEN '26 - 30'
                            WHEN TIMESTAMPDIFF(YEAR, NgaySinh, CURDATE()) BETWEEN 31 AND 40 THEN '31 - 40'
                            WHEN TIMESTAMPDIFF(YEAR, NgaySinh, CURDATE()) BETWEEN 41 AND 50 THEN '41 - 50'
                            WHEN TIMESTAMPDIFF(YEAR, NgaySinh, CURDATE()) > 50 THEN '50+'
                        END AS AgeGroup
                    FROM nhanvien
                    WHERE NgaySinh IS NOT NULL
                ) AS T
                WHERE AgeGroup IS NOT NULL
                GROUP BY AgeGroup;
            ";

            DataTable dt = Database.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                string group = row["AgeGroup"].ToString();
                int count = Convert.ToInt32(row["Total"]);

                if (ageStats.ContainsKey(group))
                {
                    ageStats[group] = count;
                }
            }

            return ageStats;
        }


        public (int Male, int Female) GetGenderStats()
        {
            string query = @"
                SELECT GioiTinh, COUNT(*) AS Total
                FROM nhanvien
                GROUP BY GioiTinh;
            ";

            DataTable dt = Database.ExecuteQuery(query);

            int male = 0;
            int female = 0;

            foreach (DataRow row in dt.Rows)
            {
                string gender = row["GioiTinh"].ToString();
                int count = Convert.ToInt32(row["Total"]);

                if (gender == "Nam")
                    male = count;
                else if (gender == "Nữ")
                    female = count;
            }

            return (male, female);
        }

        public DataTable GetEmployeeReports(
            string? name,
            string? department,
            string? gender,
            string? position,
            string? status)
        {
            string query = @"
                SELECT
                    nv.MaNV,
                    nv.HoTen,
                    nv.GioiTinh,
                    nv.NgaySinh,
                    nv.SoCCCD,
                    nv.DienThoai,
                    
                    nv.TrangThai,
                    nv.NgayVaoLam,

                    pb.TenPB,
                    cv.TenCV

                FROM nhanvien nv
                LEFT JOIN phongban pb ON nv.MaPB = pb.MaPB

                LEFT JOIN nhanvien_chucvu nvcv
                    ON nv.MaNV = nvcv.MaNV AND nvcv.LoaiChucVu = 'Chính thức'

                LEFT JOIN chucvu cv
                    ON nvcv.MaCV = cv.MaCV

                WHERE 1 = 1
            ";

            var parameters = new Dictionary<string, object>();

            // --- Lọc theo tên ---
            if (!string.IsNullOrWhiteSpace(name))
            {
                query += " AND (nv.HoTen LIKE @name OR nv.MaNV LIKE @name)";
                parameters.Add("@name", $"%{name}%");
            }

            // --- Lọc theo phòng ban ---
            if (!string.IsNullOrWhiteSpace(department))
            {
                query += " AND pb.TenPB = @department";
                parameters.Add("@department", department);
            }

            // --- Lọc theo giới tính ---
            if (!string.IsNullOrWhiteSpace(gender))
            {
                query += " AND nv.GioiTinh = @gender";
                parameters.Add("@gender", gender);
            }

            // --- Lọc theo chức vụ (chức vụ chính thức) ---
            if (!string.IsNullOrWhiteSpace(position))
            {
                query += " AND cv.TenCV = @position";
                parameters.Add("@position", position);
            }

            // --- Lọc theo trạng thái làm việc ---
            if (!string.IsNullOrWhiteSpace(status))
            {
                query += " AND nv.TrangThai = @status";
                parameters.Add("@status", status);
            }

            return Database.ExecuteQuery(query, parameters);
        }

        public string[] GetPositionNames()
        {
            string query = "SELECT TenCV FROM chucvu ORDER BY MaCV ASC";

            DataTable dt = Database.ExecuteQuery(query);

            if (dt.Rows.Count == 0)
                return Array.Empty<string>();

            return dt.AsEnumerable()
                     .Select(row => row["TenCV"].ToString()!)
                     .ToArray();
        }
    }
};