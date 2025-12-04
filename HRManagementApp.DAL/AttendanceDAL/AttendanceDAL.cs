using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL
{
    public class AttendanceDAL
    {
        // 1. Lấy danh sách chấm công hôm nay
        public List<AttendanceTodayModel> GetTodayAttendance()
        {
            List<AttendanceTodayModel> list = new List<AttendanceTodayModel>();

            // SỬA LẠI QUERY: Đưa (SELECT * FROM calam LIMIT 1) vào trong ngoặc
            string query = @"
        SELECT 
            nv.MaNV, nv.HoTen, pb.TenPB, 
            cc.GioVao, cc.GioRa, 
            cal.GioBatDau
        FROM nhanvien nv
        LEFT JOIN phongban pb ON nv.MaPB = pb.MaPB
        LEFT JOIN chamcong cc ON nv.MaNV = cc.MaNV AND cc.Ngay = CURRENT_DATE()
        LEFT JOIN (SELECT GioBatDau FROM calam LIMIT 1) cal ON 1=1 
        WHERE nv.TrangThai = 'Còn làm việc'";

            DataTable dt = Database.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                AttendanceTodayModel model = new AttendanceTodayModel();
                model.Id = Convert.ToInt32(row["MaNV"]);
                model.Name = row["HoTen"].ToString();
                model.Department = row["TenPB"] != DBNull.Value ? row["TenPB"].ToString() : "Chưa phân bổ";

                TimeSpan? gioVao = row["GioVao"] != DBNull.Value ? (TimeSpan?)row["GioVao"] : null;
                TimeSpan? gioRa = row["GioRa"] != DBNull.Value ? (TimeSpan?)row["GioRa"] : null;
                TimeSpan? caBatDau = row["GioBatDau"] != DBNull.Value ? (TimeSpan?)row["GioBatDau"] : null;

                model.CheckIn = gioVao?.ToString(@"hh\:mm") ?? "--:--";
                model.CheckOut = gioRa?.ToString(@"hh\:mm") ?? "--:--";

                model.Location = "Văn phòng";

                if (gioVao.HasValue && gioRa.HasValue)
                    model.WorkHours = Math.Round((gioRa.Value - gioVao.Value).TotalHours, 1);
                else
                    model.WorkHours = 0;

                // Logic trạng thái
                if (!gioVao.HasValue) model.Status = "Vắng";
                else if (caBatDau.HasValue && gioVao.Value > caBatDau.Value.Add(TimeSpan.FromMinutes(15)))
                    model.Status = "Đi muộn";
                else if (!gioRa.HasValue) model.Status = "Đang làm";
                else model.Status = "Đúng giờ";

                list.Add(model);
            }
            return list;
        }

        // 2. Lấy danh sách chờ duyệt

        public List<ApprovalModel> GetPendingApprovals()
        {
            List<ApprovalModel> list = new List<ApprovalModel>();

            // LEFT JOIN loaidon để lấy tên loại đơn
            string query = @"
        SELECT 
            d.MaDon, 
            nv.HoTen, 
            d.NgayBatDau, 
            d.NgayKetThuc, 
            d.LyDo, 
            d.TrangThai, 
            d.NguoiDuyet,
            d.NgayGui,
            ld.TenLoaiDon
        FROM dontu d
        JOIN nhanvien nv ON d.MaNV = nv.MaNV
        LEFT JOIN loaidon ld ON d.MaLoaiDon = ld.MaLoaiDon
        WHERE d.TrangThai = 'Chưa duyệt'
        ORDER BY d.NgayGui DESC";

            DataTable dt = Database.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                DateTime start = Convert.ToDateTime(row["NgayBatDau"]);
                DateTime end = Convert.ToDateTime(row["NgayKetThuc"]);

                list.Add(new ApprovalModel
                {
                    MaDon = Convert.ToInt32(row["MaDon"]),
                    Employee = row["HoTen"].ToString(),
                    Date = start.ToString("dd/MM/yyyy"),
                    CheckIn = start.ToString("HH:mm"),
                    CheckOut = end.ToString("HH:mm"),
                    Reason = row["LyDo"].ToString(),
                    Status = row["TrangThai"].ToString(),

                    // Map dữ liệu mới
                    LoaiDon = row["TenLoaiDon"] != DBNull.Value ? row["TenLoaiDon"].ToString() : "Khác",
                    NguoiDuyet = row["NguoiDuyet"] != DBNull.Value ? row["NguoiDuyet"].ToString() : "",
                    NgayGui = Convert.ToDateTime(row["NgayGui"])
                });
            }
            return list;
        }

        public bool UpdateApprovalStatus(int maDon, string newStatus, string nguoiDuyet)
        {
            string query = @"UPDATE dontu 
                     SET TrangThai = @Status, 
                         NguoiDuyet = @NguoiDuyet 
                     WHERE MaDon = @MaDon";

            var parameters = new Dictionary<string, object>
    {
        { "@Status", newStatus },
        { "@NguoiDuyet", nguoiDuyet },
        { "@MaDon", maDon }
    };

            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        // 3. Báo cáo thống kê
        public ReportStatModel GetMonthlyReport(int month, int year)
        {
            ReportStatModel report = new ReportStatModel();

            // A. Tổng số ngày công (Đếm số lượt chấm công trong tháng)
            string queryCount = $"SELECT COUNT(*) FROM chamcong WHERE MONTH(Ngay) = {month} AND YEAR(Ngay) = {year}";
            DataTable dtCount = Database.ExecuteQuery(queryCount);
            if (dtCount.Rows.Count > 0)
                report.TotalWorkDays = Convert.ToInt32(dtCount.Rows[0][0]);

            // B. Tổng giờ làm & Giờ tăng ca
            // Logic: Tính tổng giờ làm. Nếu > 8 tiếng/ngày thì phần dư là tăng ca.
            string queryHours = $@"
                SELECT 
                    SUM(TIME_TO_SEC(TIMEDIFF(GioRa, GioVao)))/3600 AS TongGio,
                    SUM(CASE 
                        WHEN (TIME_TO_SEC(TIMEDIFF(GioRa, GioVao))/3600) > 8 
                        THEN (TIME_TO_SEC(TIMEDIFF(GioRa, GioVao))/3600) - 8 
                        ELSE 0 
                    END) AS TangCa
                FROM chamcong 
                WHERE MONTH(Ngay) = {month} AND YEAR(Ngay) = {year} AND GioRa IS NOT NULL";

            DataTable dtHours = Database.ExecuteQuery(queryHours);
            if (dtHours.Rows.Count > 0)
            {
                if (dtHours.Rows[0]["TongGio"] != DBNull.Value)
                    report.TotalWorkHours = Math.Round(Convert.ToDouble(dtHours.Rows[0]["TongGio"]), 1);

                if (dtHours.Rows[0]["TangCa"] != DBNull.Value)
                    report.OvertimeHours = Math.Round(Convert.ToDouble(dtHours.Rows[0]["TangCa"]), 1);
            }

            // C. Tỷ lệ đi làm (AvgAttendanceRate)
            // Công thức: (Số ngày công thực tế / (Số nhân viên * 26 ngày công chuẩn)) * 100
            string queryNv = "SELECT COUNT(*) FROM nhanvien WHERE TrangThai = 'Còn làm việc'";
            DataTable dtNv = Database.ExecuteQuery(queryNv);
            int totalStaff = 1;
            if (dtNv.Rows.Count > 0) totalStaff = Convert.ToInt32(dtNv.Rows[0][0]);
            if (totalStaff == 0) totalStaff = 1; // Tránh chia cho 0

            int standardWorkDays = 26; // Giả sử tháng làm 26 ngày
            double rate = ((double)report.TotalWorkDays / (totalStaff * standardWorkDays)) * 100;
            report.AvgAttendanceRate = Math.Round(rate, 1);

            return report;
        }

        // 2. Lấy chi tiết chấm công theo ngày (Đã fix lỗi cú pháp SQL LIMIT)
        public List<AttendanceTodayModel> GetAttendanceByDate(DateTime date)
        {
            List<AttendanceTodayModel> list = new List<AttendanceTodayModel>();

            string query = @"
                SELECT 
                    nv.MaNV, nv.HoTen, pb.TenPB, 
                    cc.GioVao, cc.GioRa, 
                    cal.GioBatDau
                FROM nhanvien nv
                LEFT JOIN phongban pb ON nv.MaPB = pb.MaPB
                LEFT JOIN chamcong cc ON nv.MaNV = cc.MaNV AND cc.Ngay = @Ngay
                LEFT JOIN (SELECT GioBatDau FROM calam LIMIT 1) cal ON 1=1 
                WHERE nv.TrangThai = 'Còn làm việc'";

            var parameters = new Dictionary<string, object>
            {
                { "@Ngay", date.ToString("yyyy-MM-dd") }
            };

            DataTable dt = Database.ExecuteQuery(query, parameters);

            foreach (DataRow row in dt.Rows)
            {
                AttendanceTodayModel model = new AttendanceTodayModel();
                model.Id = Convert.ToInt32(row["MaNV"]);
                model.Name = row["HoTen"].ToString();
                model.Department = row["TenPB"] != DBNull.Value ? row["TenPB"].ToString() : "Chưa phân bổ";

                TimeSpan? gioVao = row["GioVao"] != DBNull.Value ? (TimeSpan?)row["GioVao"] : null;
                TimeSpan? gioRa = row["GioRa"] != DBNull.Value ? (TimeSpan?)row["GioRa"] : null;
                TimeSpan? caBatDau = row["GioBatDau"] != DBNull.Value ? (TimeSpan?)row["GioBatDau"] : null;

                model.CheckIn = gioVao?.ToString(@"hh\:mm") ?? "--:--";
                model.CheckOut = gioRa?.ToString(@"hh\:mm") ?? "--:--";
                model.Location = "Văn phòng";

                if (gioVao.HasValue && gioRa.HasValue)
                    model.WorkHours = Math.Round((gioRa.Value - gioVao.Value).TotalHours, 1);
                else
                    model.WorkHours = 0;

                // Logic trạng thái
                if (!gioVao.HasValue)
                    model.Status = "Vắng";
                else if (caBatDau.HasValue && gioVao.Value > caBatDau.Value.Add(TimeSpan.FromMinutes(15)))
                    model.Status = "Đi muộn";
                else if (!gioRa.HasValue)
                    model.Status = "Đang làm";
                else
                    model.Status = "Đúng giờ";

                list.Add(model);
            }
            return list;
        }
        // Trong file AttendanceDAL.cs

        public List<AttendanceExportRawModel> GetExportData(int month, int year)
        {
            List<AttendanceExportRawModel> list = new List<AttendanceExportRawModel>();

            // Query lấy tất cả nhân viên và dữ liệu chấm công của họ trong tháng
            string query = $@"
        SELECT 
            nv.MaNV, nv.HoTen, pb.TenPB, 
            cc.Ngay, cc.GioVao, cc.GioRa
        FROM nhanvien nv
        LEFT JOIN phongban pb ON nv.MaPB = pb.MaPB
        LEFT JOIN chamcong cc ON nv.MaNV = cc.MaNV 
            AND MONTH(cc.Ngay) = {month} 
            AND YEAR(cc.Ngay) = {year}
        WHERE nv.TrangThai = 'Còn làm việc'
        ORDER BY pb.TenPB, nv.MaNV, cc.Ngay";

            DataTable dt = Database.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                var item = new AttendanceExportRawModel
                {
                    MaNV = Convert.ToInt32(row["MaNV"]),
                    HoTen = row["HoTen"].ToString(),
                    TenPB = row["TenPB"] != DBNull.Value ? row["TenPB"].ToString() : "Khác",
                    Ngay = row["Ngay"] != DBNull.Value ? (DateTime?)row["Ngay"] : null,
                    GioVao = row["GioVao"] != DBNull.Value ? (TimeSpan?)row["GioVao"] : null,
                    GioRa = row["GioRa"] != DBNull.Value ? (TimeSpan?)row["GioRa"] : null
                };
                list.Add(item);
            }
            return list;
        }
    }
}