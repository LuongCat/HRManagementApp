using HRManagementApp.models;

namespace HRManagementApp.Services;
using HRManagementApp.Data;
public class PhongBanService
{
    private readonly PhongBanReponsitory _phongBanReponsitory;

    public PhongBanService()
    {
        _phongBanReponsitory = new PhongBanReponsitory();
    }
    public List<PhongBan> GetListPhongBan()
    {
        try
        {
            return _phongBanReponsitory.GetAllPhongBan();
        }
        catch(Exception ex)
        {
            Console.WriteLine("lỗi lấy danh sách phong ban:" + ex.Message);
            return new List<PhongBan>();
        }
    }
}