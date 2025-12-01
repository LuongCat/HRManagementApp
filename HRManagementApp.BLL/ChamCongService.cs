using HRManagementApp.DAL;

namespace HRManagementApp.BLL;
using HRManagementApp.models;
public class ChamCongService
{
    private readonly ChamCongRepository _chamCongRepository;

    public ChamCongService()
    {
        _chamCongRepository = new ChamCongRepository();
    }
    public List<ChamCong> GetAllChamCongByNhanVienId(int maNV)
    {
        return _chamCongRepository.GetAllChamCongByNhanVienId(maNV);
    }

    public KetQuaChamCong GetChamCongStatistics(int maNV, int thang, int nam)
    {
        return _chamCongRepository.GetChamCongStatistics(maNV, thang, nam); 
    }

    public List<ChamCong> GetAllAttendancByMonthYear(int Day,int Month, int Year)
    {
        return _chamCongRepository.GetAllAttendancByMonthYear(Day,Month, Year);
    }
}