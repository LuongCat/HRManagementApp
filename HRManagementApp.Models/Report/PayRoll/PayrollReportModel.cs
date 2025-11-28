using System;

namespace HRManagementApp.models
{
    public class PayrollReportModel
    {
        public int MaNV { get; set; }
        public string EmployeeName { get; set; } // Họ tên
        public string Department { get; set; }   // Phòng ban
        public decimal BaseSalary { get; set; }  // Lương cơ bản
        public decimal TotalAllowance { get; set; } // Tổng phụ cấp
        public decimal TotalDeduction { get; set; } // Tổng khấu trừ (Bảo hiểm + Thuế)
        public decimal NetPay { get; set; }      // Thực lãnh (Lương thực nhận)

        // Các thuộc tính phụ để hiển thị trạng thái
        public int WorkDays { get; set; }        // Tổng ngày công
        public string Status { get; set; }       // Trạng thái trả lương
        public string StatusColor
        {
            get
            {
                if (Status == "Đã trả") return "#16A34A"; // Xanh lá
                if (Status == "Chưa trả") return "#F59E0B"; // Cam
                return "#64748B"; // Xám
            }
        }
    }
}