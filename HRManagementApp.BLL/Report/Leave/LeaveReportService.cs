namespace HRManagementApp.BLL.Report
{
    using HRManagementApp.DAL.Report;
    using HRManagementApp.models;
    using System.Data;

    public class LeaveReportService
    {
        public readonly LeaveReportRepository leaveReportRepository = new LeaveReportRepository();

        public List<LeaveReport> GetLeaveRecords(string? name,
                                        DateOnly? date,
                                        string? department,
                                        string? status)
        {
            name = string.IsNullOrWhiteSpace(name) || name == "Tìm nhân viên..." ? null : name;
            department = string.IsNullOrWhiteSpace(department) || department == "Tất cả" ? null : department;
            status = string.IsNullOrWhiteSpace(status) || status == "Tất cả" ? null : status;

            DataTable dt = leaveReportRepository.GetLeaveRecords(name, date, department, status);

            var list = new List<LeaveReport>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new LeaveReport
                {
                    MaNV = row["MaNV"]?.ToString(),
                    TenNV = row["HoTen"]?.ToString(),
                    LoaiNghi = row["LoaiDon"]?.ToString(),   // Loại đơn từ bảng loaidon
                    TuNgay = row["NgayBatDau"] == DBNull.Value ? DateTime.MinValue : (DateTime)row["NgayBatDau"],
                    DenNgay = row["NgayKetThuc"] == DBNull.Value ? DateTime.MinValue : (DateTime)row["NgayKetThuc"],
                    SoNgay = row["SoNgay"] == DBNull.Value ? 0 : Convert.ToInt32(row["SoNgay"]),
                    LyDo = row["LyDo"]?.ToString(),
                    TrangThai = row["TrangThai"]?.ToString()
                };

                list.Add(item);
            }

            return list;
        }

        public int GetTotalLeaveToday()
        {
            return leaveReportRepository.CountLeave();
        }

        public int GetAnnualLeaveToday()
        {
            return leaveReportRepository.CountAnnualLeaveToday();
        }

        public int GetSickLeaveToday()
        {
            return leaveReportRepository.CountSickLeaveToday();
        }

        public int GetUnpaidLeaveToday()
        {
            return leaveReportRepository.CountUnpaidLeaveToday();
        }
    }
}
