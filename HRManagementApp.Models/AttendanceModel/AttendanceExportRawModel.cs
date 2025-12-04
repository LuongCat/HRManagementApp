using System;

namespace HRManagementApp.models
{
    // Model để hứng dữ liệu thô từ SQL phục vụ xuất Excel
    public class AttendanceExportRawModel
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public string TenPB { get; set; }
        public DateTime? Ngay { get; set; }
        public TimeSpan? GioVao { get; set; }
        public TimeSpan? GioRa { get; set; }
    }
}