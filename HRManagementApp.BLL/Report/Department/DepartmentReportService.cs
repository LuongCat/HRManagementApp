namespace HRManagementApp.BLL.Report
{
    using HRManagementApp.DAL.Report;
    using HRManagementApp.models;
    using System.Data;

    public class DepartmentReportService
    {
        public readonly DepartmentReportRepository departmentReportRepository = new DepartmentReportRepository();

        public string[] GetDepartmentNames()
        {
            return departmentReportRepository.GetDepartmentNames();
        }

        public Dictionary<string, int> GetEmployeesCountByDepartment()
        {
            return departmentReportRepository.GetEmployeesCountByDepartment();
        }

        public int GetDepartmentsCount()
        {
            return departmentReportRepository.CountDepartments();
        }

        public List<DepartmentReport> GetDepartmentReports(string? name)
        {
            // Xử lý filter giống Employee
            name = string.IsNullOrWhiteSpace(name) || name == "Tìm phòng ban..." ? null : name;

            // Gọi DAL lấy DataTable
            DataTable dt = departmentReportRepository.GetDepartmentReports(name);

            var list = new List<DepartmentReport>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new DepartmentReport
                {
                    MaPB = row["MaPB"] == DBNull.Value ? 0 : Convert.ToInt32(row["MaPB"]),
                    TenPB = row["TenPB"]?.ToString(),
                    TruongPhong = row["TruongPhong"]?.ToString(),
                    SoNhanVien = row["SoNhanVien"] == DBNull.Value ? 0 : Convert.ToInt32(row["SoNhanVien"])
                };

                list.Add(item);
            }

            return list;
        }
    }
}
