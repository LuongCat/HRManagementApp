using HRManagementApp.DAL;
using HRManagementApp.models;
namespace  HRManagementApp.BLL
{
    public class EmployeeReportBLL
    {

        public EmployeeModel GetDashboarEmployee()
        {
            // Gọi DAL (giả sử bạn viết hàm trên trong AttendanceDAL hoặc ReportDAL)
            return new EmployeeDAL().GetEmployeeSummary();
        }
        // Trong file AnalyticsBLL.cs

        public Dictionary<string, int> GetDepartmentStats()
        {
            return new EmployeeDAL().GetEmployeeByDepartment();
        }
    }
};

