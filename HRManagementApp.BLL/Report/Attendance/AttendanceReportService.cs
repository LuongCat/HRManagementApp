namespace HRManagementApp.BLL.Report
{
    using HRManagementApp.DAL.Report;
    using HRManagementApp.models;
    using System.Data;

    public class AttendanceReportService
    {
        public readonly AttendanceReportRepository attendanceReportRepository = new AttendanceReportRepository();

        public List<ChamCongReport> GetAttendanceRecords(string? name,
                                                  DateOnly? date,
                                                  string? department,
                                                  string? status)
        {
            name = string.IsNullOrWhiteSpace(name) || name == "Tìm nhân viên..." ? null : name;
            department = string.IsNullOrWhiteSpace(department) || department == "Tất cả" ? null : department;
            status = string.IsNullOrWhiteSpace(status) || status == "Tất cả" ? null : status;

            DataTable dt = attendanceReportRepository.GetAttendanceRecords(name, date, department, status);

            var list = new List<ChamCongReport>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new ChamCongReport
                {
                    MaNV = row["MaNV"] == DBNull.Value ? 0 : Convert.ToInt32(row["MaNV"]),
                    TenNV = row["HoTen"]?.ToString(),
                    TenPB = row["TenPB"]?.ToString(),
                    Ngay = row["Ngay"] == DBNull.Value ? null : DateOnly.FromDateTime((DateTime)row["Ngay"]),
                    GioVao = row["GioVao"] == DBNull.Value ? null : (TimeSpan?)TimeSpan.Parse(row["GioVao"].ToString()),
                    GioRa = row["GioRa"] == DBNull.Value ? null : (TimeSpan?)TimeSpan.Parse(row["GioRa"].ToString()),
                    TenCa = row["TenCa"]?.ToString(),
                    TrangThai = row["TrangThai"]?.ToString()
                };

                list.Add(item);
            }

            return list;
        }

        public int GetPresentToday()
        {
            return attendanceReportRepository.CountPresentToday();
        }

        public int GetLateToday()
        {
            return attendanceReportRepository.CountLateToday();
        }
    }
}
