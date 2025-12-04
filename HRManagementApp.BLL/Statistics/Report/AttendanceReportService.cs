namespace HRManagementApp.BLL.statistics
{
    using HRManagementApp.DAL.Report;
    using HRManagementApp.models;
    using System.Data;

    public class AttendanceReportService
    {
        public readonly AttendanceReportRepository attendanceReportRepository = new AttendanceReportRepository();

        public List<ChamCong> GetAttendanceRecord(string? name,
                                                  DateTime? date,
                                                  string? department,
                                                  string? status)
        {
            name = string.IsNullOrWhiteSpace(name) || name == "Search name..." ? null : name;
            department = string.IsNullOrWhiteSpace(department) || department == "All departments" ? null : department;
            status = string.IsNullOrWhiteSpace(status) || status == "All status" ? null : status;

            DataTable dt = attendanceReportRepository.GetAttendanceRecord(name, date, department, status);

            var list = new List<ChamCong>();

            foreach (DataRow row in dt.Rows)
            {
                var chamCong = new ChamCong
                {
                    MaNV = row["MaNV"] == DBNull.Value ? 0 : Convert.ToInt32(row["MaNV"]),
                    Ngay = row["Ngay"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["Ngay"]),
                    GioVao = row["CheckIn"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["CheckIn"]),
                    GioRa = row["CheckOut"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["CheckOut"]),
                    TrangThai = row["Status"]?.ToString()
                };

                list.Add(chamCong);
            }

            return list;
        }
    }
}
