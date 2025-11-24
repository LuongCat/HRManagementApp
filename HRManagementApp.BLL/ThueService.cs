using HRManagementApp.DAL;
using System.Data;
using HRManagementApp.models;
namespace HRManagementApp.BLL;

public class ThueService
{
    private readonly ThueRepository _thueRepository;
    
    public ThueService()
    {
        _thueRepository = new ThueRepository();
    }

    public List<Thue> GetAllTax()
    {
        return _thueRepository.GetAllTax();
    }

    public List<Thue> GetTaxByMaNV(int maNV)
    {
        return _thueRepository.GetTaxByMaNV(maNV);
    }

    public bool AddTax(Thue thue)
    {
        return _thueRepository.AddTax(thue);
    }

    public bool UpdateTax(Thue thue)
    {
        return _thueRepository.UpdateTax(thue);
    }

    public bool DeleteTax(int maThue, int maNV)
    {
        return _thueRepository.DeleteTax(maThue, maNV);
    }

    public bool Delete(int maThue)
    {
        return _thueRepository.Delete(maThue);
    }
}