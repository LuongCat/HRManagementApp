namespace HRManagementApp.models
{
    public class PayrollSummary
    {
        public decimal TotalSalary { get; set; } // Tổng lương thực nhận của tất cả nhân viên
        public decimal PaidSalary { get; set; }  // Tổng lương Đã trả
        public decimal UnpaidSalary { get; set; }// Tổng lương Chưa trả
        public int TotalEmployees { get; set; }  // Tổng số nhân viên đang có trong hệ thống
    }
}