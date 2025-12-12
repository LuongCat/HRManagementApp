using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL.Report
{
    public class LeaveReportRepository
    {
        public DataTable GetLeaveRecords(string? name,
                                DateOnly? date,
                                string? department,
                                string? status)
        {
            string query = @"
                SELECT 
                    nv.MaNV,
                    nv.HoTen,
                    pb.TenPB,
                    ld.TenLoaiDon AS LoaiDon,
                    dt.NgayBatDau,
                    dt.NgayKetThuc,
                    TIMESTAMPDIFF(DAY, dt.NgayBatDau, dt.NgayKetThuc) + 1 AS SoNgay,
                    dt.NgayGui,
                    dt.LyDo,
                    dt.TrangThai,
                    dt.NguoiDuyet
                FROM dontu dt
                JOIN nhanvien nv ON dt.MaNV = nv.MaNV
                JOIN phongban pb ON nv.MaPB = pb.MaPB
                LEFT JOIN loaidon ld ON dt.MaLoaiDon = ld.MaLoaiDon
                WHERE 1 = 1
            ";

            var parameters = new Dictionary<string, object>();

            // theo tên
            if (!string.IsNullOrWhiteSpace(name))
            {
                query += " AND (nv.HoTen LIKE @name OR nv.MaNV LIKE @name)";
                parameters.Add("@name", $"%{name}%");
            }

            // theo phòng ban
            if (!string.IsNullOrWhiteSpace(department))
            {
                query += " AND pb.TenPB = @department";
                parameters.Add("@department", department);
            }

            // lọc ngày theo dạng giao thoa
            if (date.HasValue)
            {
                query += @"
                    AND DATE(@date) >= DATE(dt.NgayBatDau)
                    AND DATE(@date) <= DATE(dt.NgayKetThuc)
                ";
                parameters.Add("@date", date.Value.ToDateTime(TimeOnly.MinValue));
            }

            // lọc trạng thái
            if (!string.IsNullOrWhiteSpace(status))
            {
                query += " AND dt.TrangThai = @status";
                parameters.Add("@status", status);
            }

            query += " ORDER BY dt.NgayGui";

            return Database.ExecuteQuery(query, parameters);
        }

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

        public int CountAnnualLeaveToday()
        {
            string query = "SELECT COUNT(*) as AnnualCount FROM dontu " +
                           "JOIN loaidon ON dontu.MaLoaiDon = loaidon.MaLoaiDon " +
                           "AND NOW() BETWEEN NgayBatDau AND NgayKetThuc " +
                           "WHERE TrangThai = 'Đã duyệt' " + 
                           "AND loaidon.TenLoaiDon = 'Nghỉ phép năm'";

            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public int CountSickLeaveToday()
        {
            string query = "SELECT COUNT(*) as SickCount FROM dontu " +
                           "JOIN loaidon ON dontu.MaLoaiDon = loaidon.MaLoaiDon " +
                           "AND NOW() BETWEEN NgayBatDau AND NgayKetThuc " +
                           "WHERE TrangThai = 'Đã duyệt' " + 
                           "AND loaidon.TenLoaiDon = 'Nghỉ ốm'";

            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public int CountUnpaidLeaveToday()
        {
            string query = "SELECT COUNT(*) as UnpaidCount FROM dontu " +
                           "JOIN loaidon ON dontu.MaLoaiDon = loaidon.MaLoaiDon " + 
                           "AND NOW() BETWEEN NgayBatDau AND NgayKetThuc " +
                           "WHERE TrangThai = 'Từ chối' ";

            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }
    }
}