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

    public List<ChamCong> GetAllAttendancByMonthYear(int day, int month, int year)
    {
        if (UserSession.MaNV == null) return new List<ChamCong>();
            
        DateTime date = new DateTime(year, month, day);
        return _chamCongRepository.GetByDate(UserSession.MaNV.Value, date);
    }

    public ChamCong GetChamCongToday(int maNV)
    {
        return _chamCongRepository.GetTodayRecord(maNV);
    }

    public bool CheckIn(int maNV)
    {
        // Có thể thêm logic kiểm tra xem đã checkin chưa ở đây nếu muốn an toàn hơn
        return _chamCongRepository.CheckIn(maNV);
    }

    public bool CheckOut(int maCC)
    {
        return _chamCongRepository.CheckOut(maCC);
    }

    public List<ChamCong> GetChamCongByMonth(int maNV, int month, int year)
    {
        return _chamCongRepository.GetByMonth(maNV, month, year);
    }
}