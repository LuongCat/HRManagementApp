namespace HRManagementApp.models;

public class ChamCong
{
    public int MaCC { get; set; }
    public int MaNV { get; set; }
    public DateTime? Ngay { get; set; }
    public DateTime? GioVao { get; set; }
    public DateTime? GioRa { get; set; }
    public string? TrangThai { get; set; }
}