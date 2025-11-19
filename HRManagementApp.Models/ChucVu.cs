namespace HRManagementApp.models;

public class ChucVu
{
    public int MaCV { get; set; }
    public string TenCV { get; set; }
    public decimal? PhuCap { get; set; }
    public decimal? LuongCB { get; set; }
    
    public decimal? TienPhuCapKiemNhiem { get; set; }

    public override string ToString()
    {
        return TenCV;
    }
}