using HRManagementApp.DAL;
using HRManagementApp.models;
namespace HRManagementApp.BLL;

public class LuongService
{
    private readonly LuongRepository _luongRepository;
    
    public LuongService()
    {
        _luongRepository = new LuongRepository();
    }

    public List<Luong> GetAllSalary()
    {
        return _luongRepository.GetAllSalary();
    }

    public List<Luong> GetSalaryByMaNV(int maNV)
    {
        return _luongRepository.GetSalaryByMaNV(maNV);
    }

    public Luong GetSalaryByMonthYear(int maNV, int thang, int nam)
    {
        return _luongRepository.GetSalaryByMonthYear(maNV, thang, nam);
    }

    public bool AddSalary(Luong luong)
    {
        return _luongRepository.AddSalary(luong);
    }

    public bool UpdateSalary(Luong luong)
    {
        return _luongRepository.UpdateSalary(luong);
    }
}