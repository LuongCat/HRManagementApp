namespace HRManagementApp.models
{
    public class AnalyticsModel
    {
        public int TotalEmployees { get; set; }      // Tổng nhân viên
        public int ActiveEmployees { get; set; }     // Đang làm việc
        public int LateToday { get; set; }           // Đi muộn hôm nay
        public int AbsentToday { get; set; }         // Vắng mặt hôm nay
        public int PendingRequests { get; set; }     // Đơn chờ duyệt
    }
}