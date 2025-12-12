using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL
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
        public PayrollStatsDTO GetPayrollSummary(int month, int year)
        {
            var stats = new PayrollStatsDTO();
            string query = $@"
                SELECT 
                    SUM(l.LuongThucNhan) as TongLuong,
                    (SELECT COALESCE(SUM(SoTien),0) FROM thue t WHERE MONTH(t.ApDungTuNgay) <= {month} AND (t.ApDungDenNgay IS NULL OR MONTH(t.ApDungDenNgay) >= {month})) as TongThue,
                    (SELECT COALESCE(SUM(SoTien),0) FROM khautru kt WHERE MONTH(kt.Ngay) = {month} AND YEAR(kt.Ngay) = {year}) as TongKhauTru
                FROM luong l
                WHERE l.Thang = {month} AND l.Nam = {year}";

            DataTable dt = Database.ExecuteQuery(query);
            if (dt.Rows.Count > 0)
            {
                stats.TongLuong = dt.Rows[0]["TongLuong"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["TongLuong"]) : 0;
                stats.TongThue = dt.Rows[0]["TongThue"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["TongThue"]) : 0;
                stats.TongKhauTru = dt.Rows[0]["TongKhauTru"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["TongKhauTru"]) : 0;
            }
            return stats;
        }

        // 2. Biểu đồ tròn: Cơ cấu thu nhập (Lương CB vs Phụ Cấp vs Thưởng)
        public List<ChartDataDTO> GetIncomeStructure(int month, int year)
        {
            // Đơn giản hóa: Lấy Tổng Lương CB và Tổng Phụ Cấp của tháng đó
            var list = new List<ChartDataDTO>();
            string query = $@"
                SELECT 
                    SUM(cv.LuongCB) as TongLuongCB,
                    (SELECT COALESCE(SUM(SoTien),0) FROM phucap_nhanvien pc 
                     WHERE pc.ApDungTuNgay <= LAST_DAY('{year}-{month}-01')) as TongPhuCap
                FROM luong l
                JOIN nhanvien_chucvu nvcv ON l.MaNV = nvcv.MaNV
                JOIN chucvu cv ON nvcv.MaCV = cv.MaCV
                WHERE l.Thang = {month} AND l.Nam = {year}";

            DataTable dt = Database.ExecuteQuery(query);
            if (dt.Rows.Count > 0)
            {
                list.Add(new ChartDataDTO { Label = "Lương Cơ Bản", Value = Convert.ToDouble(dt.Rows[0]["TongLuongCB"]) });
                list.Add(new ChartDataDTO { Label = "Các Phụ Cấp", Value = Convert.ToDouble(dt.Rows[0]["TongPhuCap"]) });
            }
            return list;
        }

        // 3. Biểu đồ miền: Phân bổ mức lương (Số người theo khoảng lương)
        public List<ChartDataDTO> GetSalaryDistribution(int month, int year)
        {
            var list = new List<ChartDataDTO>();
            string query = $@"
                SELECT 
                    CASE 
                        WHEN LuongThucNhan < 10000000 THEN '< 10 Triệu'
                        WHEN LuongThucNhan BETWEEN 10000000 AND 20000000 THEN '10 - 20 Triệu'
                        WHEN LuongThucNhan > 20000000 THEN '> 20 Triệu'
                    END as MucLuong,
                    COUNT(*) as SoLuong
                FROM luong
                WHERE Thang = {month} AND Nam = {year}
                GROUP BY MucLuong";

            DataTable dt = Database.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ChartDataDTO { 
                    Label = row["MucLuong"].ToString(), 
                    Value = Convert.ToDouble(row["SoLuong"]) 
                });
            }
            return list;
        }

        // 4. Biểu đồ cột/miền: Thu nhập trung bình theo phòng ban
        public List<ChartDataDTO> GetAvgSalaryByDept(int month, int year)
        {
            var list = new List<ChartDataDTO>();
            string query = $@"
                SELECT pb.TenPB, AVG(l.LuongThucNhan) as LuongTB
                FROM luong l
                JOIN nhanvien nv ON l.MaNV = nv.MaNV
                JOIN phongban pb ON nv.MaPB = pb.MaPB
                WHERE l.Thang = {month} AND l.Nam = {year}
                GROUP BY pb.TenPB";

            DataTable dt = Database.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ChartDataDTO { 
                    Label = row["TenPB"].ToString(), 
                    Value = Convert.ToDouble(row["LuongTB"]) 
                });
            }
            return list;
        }
    }

}