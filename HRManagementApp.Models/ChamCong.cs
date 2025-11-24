namespace HRManagementApp.models;

public class ChamCong
{
    public int MaCC { get; set; }
    public int MaNV { get; set; }
    public DateTime? Ngay { get; set; }
    public TimeSpan? GioVao { get; set; }
    public TimeSpan? GioRa { get; set; }
    
    public TimeSpan? ThoiGianLam {get; set;}
}