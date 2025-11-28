namespace HRManagementApp.models
{
    // 1. Model cho TodayAttendanceView
    public class AttendanceTodayModel
    {
        public int Id { get; set; }           // Mã NV
        public string Name { get; set; }      // Họ tên
        public string Department { get; set; }// Phòng ban
        public string CheckIn { get; set; }   // Giờ vào
        public string CheckOut { get; set; }  // Giờ ra
        public string Location { get; set; }  // Tên địa điểm chấm công (chỉ là text hiển thị)
        public double WorkHours { get; set; } // Số giờ làm
        public string Status { get; set; }    // Trạng thái
    }
}