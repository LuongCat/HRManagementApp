namespace HRManagementApp.BLL;
using HRManagementApp.models;
public class PayrollResultService
{
    public NhanVienService NhanVienService { get; set; }
    public LuongService LuongService { get; set; }
    public PhuCapService PhuCapService { get; set; }
    public ThueService ThueService { get; set; }
    
    public PayrollResultService()
    {
        PhuCapService = new PhuCapService();
    }

    public PayrollResult GetPayrollResultForEmployee(NhanVien nhanVien)
    {
        var VaitroNhanVien = nhanVien.DanhSachChucVu;

        decimal tongPhucaptemp = 0;
        
        foreach (var tmp in VaitroNhanVien)
        {
            
        }
    }
    
}