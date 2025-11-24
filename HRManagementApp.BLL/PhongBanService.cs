using HRManagementApp.models;

namespace HRManagementApp.BLL;
using HRManagementApp.DAL;
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

    public PhongBan GetPhongBanByName(String name)
    {
        return _phongBanReponsitory.GetPhongBanByName(name);
    }

    public PhongBan GetPhongBanByID(int id)
    {
        return _phongBanReponsitory.GetPhongBanById(id);
    }

    public bool UpdateDeparment(PhongBan phongBan)
    {
        return _phongBanReponsitory.UpdateDeparment(phongBan);
    }

    public bool InsertDepartment(PhongBan phongBan)
    {
        return _phongBanReponsitory.InsertDepartment(phongBan);
    }

    public bool DeleteDepartment(int maPB)
    {
        return _phongBanReponsitory.DeleteDepartment(maPB);
    }

    public NhanVien GetDeparmentHead(int maPB)
    {
        return _phongBanReponsitory.GetDeparmentHead(maPB);
    }
}