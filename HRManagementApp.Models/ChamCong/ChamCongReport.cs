namespace HRManagementApp.models
{
    public class ChamCongReport
    {
        public int MaNV { get; set; }
        public string? TenNV { get; set; }
        public string? TenPB { get; set; }
        public DateOnly? Ngay { get; set; }
        public TimeSpan? GioVao { get; set; }
        public TimeSpan? GioRa { get; set; }
        public string? TenCa { get; set; }
        public string? TrangThai { get; set; }
    }
}
