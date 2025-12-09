using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL.Report
{
    public class PayrollDAL
    {
        // Lấy báo cáo lương theo Tháng và Năm
        public List<PayrollReportModel> GetPayrollReport(int month, int year)
        {
            List<PayrollReportModel> list = new List<PayrollReportModel>();

            // Query logic:
            // 1. Lấy dữ liệu từ bảng `luong` (đây là bảng lưu lịch sử lương đã tính)
            // 2. Join với `nhanvien`, `phongban`, `chucvu` để lấy thông tin chi tiết
            // 3. Tính toán tổng phụ cấp và khấu trừ từ các bảng con tương ứng trong tháng đó

            string query = $@"
                SELECT 
                    l.MaNV, 
                    nv.HoTen, 
                    pb.TenPB,
                    cv.LuongCB,
                    l.TongNgayCong,
                    l.LuongThucNhan,
                    l.TrangThai,
                    -- Tính tổng phụ cấp đang áp dụng trong tháng này
                    (SELECT COALESCE(SUM(SoTien), 0) 
                     FROM phucap_nhanvien pc 
                     WHERE pc.MaNV = l.MaNV 
                       AND pc.ApDungTuNgay <= LAST_DAY('{year}-{month}-01') 
                       AND (pc.ApDungDenNgay IS NULL OR pc.ApDungDenNgay >= '{year}-{month}-01')
                    ) AS TongPhuCap,
                    -- Tính tổng khấu trừ (Khấu trừ + Thuế) trong tháng này
                    (SELECT COALESCE(SUM(SoTien), 0) FROM khautru kt WHERE kt.MaNV = l.MaNV AND MONTH(kt.Ngay) = {month} AND YEAR(kt.Ngay) = {year}) 
                    +
                    (SELECT COALESCE(SUM(SoTien), 0) FROM thue t WHERE t.MaNV = l.MaNV AND t.ApDungTuNgay <= LAST_DAY('{year}-{month}-01')) 
                    AS TongKhauTru
                FROM luong l
                JOIN nhanvien nv ON l.MaNV = nv.MaNV
                LEFT JOIN phongban pb ON nv.MaPB = pb.MaPB
                LEFT JOIN nhanvien_chucvu nvcv ON nv.MaNV = nvcv.MaNV AND nvcv.LoaiChucVu = 'Chính thức'
                LEFT JOIN chucvu cv ON nvcv.MaCV = cv.MaCV
                WHERE l.Thang = {month} AND l.Nam = {year}";

            DataTable dt = Database.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                var item = new PayrollReportModel();
                item.MaNV = Convert.ToInt32(row["MaNV"]);
                item.EmployeeName = row["HoTen"].ToString();
                item.Department = row["TenPB"] != DBNull.Value ? row["TenPB"].ToString() : "Chưa phân bổ";

                // Xử lý số liệu
                item.BaseSalary = row["LuongCB"] != DBNull.Value ? Convert.ToDecimal(row["LuongCB"]) : 0;
                item.TotalAllowance = row["TongPhuCap"] != DBNull.Value ? Convert.ToDecimal(row["TongPhuCap"]) : 0;
                item.TotalDeduction = row["TongKhauTru"] != DBNull.Value ? Convert.ToDecimal(row["TongKhauTru"]) : 0;
                item.NetPay = Convert.ToDecimal(row["LuongThucNhan"]);

                item.WorkDays = Convert.ToInt32(row["TongNgayCong"]);
                item.Status = row["TrangThai"].ToString();

                list.Add(item);
            }

            return list;
        }
        public Dictionary<string, decimal> GetWeeklyStats(int month, int year)
        {
            var data = new Dictionary<string, decimal>();
            string query = $@"
        SELECT 
            WEEK(cc.Ngay, 3) - WEEK(DATE_SUB(cc.Ngay, INTERVAL DAYOFMONTH(cc.Ngay) - 1 DAY), 3) + 1 AS TuanThu,
            SUM(
                (cv.LuongCB + cv.PhuCap) / 26 * (CASE 
                    WHEN cc.GioRa IS NOT NULL AND cc.GioVao IS NOT NULL THEN 
                        LEAST(1.0, TIME_TO_SEC(TIMEDIFF(cc.GioRa, cc.GioVao)) / 28800)
                    ELSE 0 
                 END)
            ) AS TongLuongUocTinh
        FROM chamcong cc
        JOIN nhanvien_chucvu nvcv ON cc.MaNV = nvcv.MaNV
        JOIN chucvu cv ON nvcv.MaCV = cv.MaCV
        WHERE MONTH(cc.Ngay) = {month} AND YEAR(cc.Ngay) = {year}
        GROUP BY TuanThu
        ORDER BY TuanThu";

            DataTable dt = Database.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                data.Add($"Tuần {row["TuanThu"]}", Convert.ToDecimal(row["TongLuongUocTinh"]));
            }
            return data;
        }

        // 2. Thống kê theo Tháng (Trong 1 năm cụ thể)
        public Dictionary<string, decimal> GetMonthlyStats(int year)
        {
            var data = new Dictionary<string, decimal>();
            string query = $@"
        SELECT Thang, SUM(LuongThucNhan) as TongLuong 
        FROM luong 
        WHERE Nam = {year} 
        GROUP BY Thang 
        ORDER BY Thang";

            DataTable dt = Database.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                data.Add($"Tháng {row["Thang"]}", Convert.ToDecimal(row["TongLuong"]));
            }
            return data;
        }

        // 3. Thống kê theo Năm (5 năm gần nhất)
        public Dictionary<string, decimal> GetYearlyStats()
        {
            var data = new Dictionary<string, decimal>();
            string query = @"
        SELECT Nam, SUM(LuongThucNhan) as TongLuong 
        FROM luong 
        GROUP BY Nam 
        ORDER BY Nam DESC LIMIT 5";

            DataTable dt = Database.ExecuteQuery(query);
            // Đảo ngược lại để hiển thị từ năm cũ đến mới trên biểu đồ
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                data.Add($"Năm {dt.Rows[i]["Nam"]}", Convert.ToDecimal(dt.Rows[i]["TongLuong"]));
            }
            return data;
        }

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

        public int CountPayrollEmployees()
        {
            string query = @"
                SELECT COUNT(DISTINCT MaNV)
                FROM luong
                WHERE (Thang = MONTH(CURRENT_DATE - INTERVAL 1 MONTH))
                    AND (Nam = YEAR(CURRENT_DATE - INTERVAL 1 MONTH));
            ";

            object result = Database.ExecuteScalar(query);

            return Convert.ToInt32(result);
        }

        public double CalcPrevMonthlyPayroll()
        {
            string query = "SELECT SUM(LuongThucNhan) AS TotalPayroll " +
                            "FROM luong " +
                            "WHERE (Thang = MONTH(DATE_SUB(CURDATE(), INTERVAL 2 MONTH)) " +
                            "AND Nam = YEAR(DATE_SUB(CURDATE(), INTERVAL 2 MONTH)))";

            object result = Database.ExecuteScalar(query);

            if (result == null || result == DBNull.Value)
                return 0.0;

            return Convert.ToDouble(result);
        }

        public double CalcMonthlyPaidPayroll()
        {
            string query = "SELECT SUM(LuongThucNhan) AS PaidPayroll " +
                            "FROM luong " +
                            "WHERE (Thang = MONTH(DATE_SUB(CURDATE(), INTERVAL 1 MONTH)) " +
                            "AND Nam = YEAR(DATE_SUB(CURDATE(), INTERVAL 1 MONTH))) " +
                            "AND TrangThai = 'Đã trả'";

            object result = Database.ExecuteScalar(query);

            if (result == null || result == DBNull.Value)
                return 0.0;

            return Convert.ToDouble(result);
        }

        public double[] GetDepartmentPayroll()
        {
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            string query = @"
                SELECT 
                    pb.TenPB,
                    IFNULL(SUM(l.LuongThucNhan), 0) AS TotalPayroll
                FROM phongban pb
                LEFT JOIN nhanvien nv ON pb.MaPB = nv.MaPB
                LEFT JOIN luong l ON nv.MaNV = l.MaNV 
                    AND l.Thang = @Thang AND l.Nam = @Nam
                GROUP BY pb.MaPB, pb.TenPB
                ORDER BY pb.MaPB;
            ";

            var parameters = new Dictionary<string, object>()
            {
                {"@Thang", month},
                {"@Nam", year}
            };

            DataTable dt = Database.ExecuteQuery(query, parameters);

            List<double> payrollList = new List<double>();

            foreach (DataRow row in dt.Rows)
            {
                payrollList.Add(Convert.ToDouble(row["TotalPayroll"]));
            }

            return payrollList.ToArray();
        }

        public double[] GetSalaryTrend()
        {
            double[] salaryTrend = new double[12];

            string query = @"
                SELECT Thang, SUM(LuongThucNhan) AS TotalSalary
                FROM luong
                GROUP BY Thang
                ORDER BY Thang;
            ";

            DataTable dt = Database.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                int month = Convert.ToInt32(row["Thang"]);
                double totalSalary = Convert.ToDouble(row["TotalSalary"]);

                if (month >= 1 && month <= 12)
                {
                    salaryTrend[month - 1] = totalSalary;
                }
            }

            return salaryTrend;
        }
    }
}