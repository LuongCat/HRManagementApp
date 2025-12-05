namespace HRManagementApp.models
{
    // Model cũ giữ nguyên, thêm các class hỗ trợ thống kê
    public class PayrollStatsDTO
    {
        public decimal TongLuong { get; set; }
        public decimal TongThue { get; set; }
        public decimal TongKhauTru { get; set; }
    }

    public class ChartDataDTO
    {
        public string Label { get; set; }
        public double Value { get; set; }
    }
}