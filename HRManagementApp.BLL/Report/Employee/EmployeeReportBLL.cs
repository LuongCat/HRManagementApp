using HRManagementApp.DAL.Report;
using HRManagementApp.models;
using System.Data;

namespace HRManagementApp.BLL.Report
{
    public class EmployeeReportBLL
    {
        private readonly EmployeeDAL employeeDAL = new EmployeeDAL();
        public Dictionary<string, int> GetDepartmentStats()
        {
            return employeeDAL.GetEmployeeByDepartment();
        }

        public int GetTotalEmployee()
        {
            return employeeDAL.CountEmployees();
        }

        public int GetEmployeesCountJoinedThisMonth()
        {
            return employeeDAL.CountJoinedThisMonth();
        }

        public int GetAverageAge()
        {
            return employeeDAL.CalcAverageAge();
        }

        public Dictionary<string, int> GetAgeStats()
        {
            return employeeDAL.GetAgeStats();
        }

        public (int Male, int Female) GetGenderStats()
        {
            return employeeDAL.GetGenderStats();
        }

        public List<EmployeeReport> GetEmployeeReports(string? name,
                                               string? department,
                                               string? gender,
                                               string? position,
                                               string? status)
        {
            name = string.IsNullOrWhiteSpace(name) || name == "Tìm nhân viên..." ? null : name;
            department = string.IsNullOrWhiteSpace(department) || department == "Tất cả" ? null : department;
            gender = string.IsNullOrWhiteSpace(gender) || gender == "Tất cả" ? null : gender;
            position = string.IsNullOrWhiteSpace(position) || position == "Tất cả" ? null : position;
            status = string.IsNullOrWhiteSpace(status) || status == "Tất cả" ? null : status;

            DataTable dt = employeeDAL.GetEmployeeReports(
                name,
                department,
                gender,
                position,
                status
            );

            var list = new List<EmployeeReport>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new EmployeeReport
                {
                    MaNV = row["MaNV"] == DBNull.Value ? 0 : Convert.ToInt32(row["MaNV"]),
                    HoTen = row["HoTen"]?.ToString(),
                    GioiTinh = row["GioiTinh"]?.ToString(),
                    NgaySinh = row["NgaySinh"] == DBNull.Value ? null : (DateTime?)row["NgaySinh"],
                    SoCCCD = row["SoCCCD"]?.ToString(),
                    DienThoai = row["DienThoai"]?.ToString(),
                    PhongBan = row["TenPB"]?.ToString(),
                    ChucVu = row["TenCV"]?.ToString(),
                    NgayVaoLam = row["NgayVaoLam"] == DBNull.Value ? null : (DateTime?)row["NgayVaoLam"],
                    TrangThai = row["TrangThai"]?.ToString()
                };

                list.Add(item);
            }

            return list;
        }

        public string[] GetPositionNames()
        {
            return employeeDAL.GetPositionNames();
        }
    }
};

