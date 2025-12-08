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
    public (bool Success, string Message, string EmployeeName, string ActionType) ProcessSmartAttendance(int maNV)
    {
        NhanVienService nvsv = new NhanVienService();

        // 🔥 1. Lấy thông tin nhân viên – phải kiểm tra null trước
        var employee = nvsv.GetEmployeeById(maNV);
        if (employee == null)
        {
            return (false, $"Không tìm thấy nhân viên với mã {maNV}.", "Unknown", "Error");
        }

        string empName = string.IsNullOrWhiteSpace(employee.HoTen) ? $"NV-{maNV}" : employee.HoTen;

        // 🔥 2. Lấy record hôm nay – có thể null (hợp lệ)
        var todayRecord = _chamCongRepository.GetTodayRecord(maNV);

        // 🔥 3. Nếu chưa có record → Check-in
        if (todayRecord == null)
        {
            bool result = _chamCongRepository.CheckIn(maNV);

            return (
                result,
                result ? "Chấm công VÀO thành công!" : "Không thể chấm công vào.",
                empName,
                "CheckIn"
            );
        }

        // 🔥 4. Nếu có GioVao nhưng chưa có GioRa → Check-out
        if (todayRecord.GioRa == null)
        {
            // Check null safety: GioVao có thể null nếu DB lỗi
            if (todayRecord.GioVao.HasValue)
            {
                var diff = (DateTime.Now - todayRecord.GioVao.Value).Minute;
                if (diff < 1)
                {
                    return (false, "Bạn vừa chấm công vào, vui lòng chờ thêm!", empName, "Wait");
                }
            }

            bool result = _chamCongRepository.CheckOut(todayRecord.MaCC);

            return (
                result,
                result ? "Chấm công RA thành công!" : "Không thể chấm công ra.",
                empName,
                "CheckOut"
            );
        }

        // 🔥 5. Đã có GioVao + GioRa đầy đủ → Không cho chấm thêm
        return (false, "Bạn đã hoàn thành ca làm việc hôm nay!", empName, "Done");
    }



    public AttendanceMonthlyResult GetAttendanceStatistics(int maNV, int thang, int nam)
    {
        return _chamCongRepository.GetAttendanceStatistics(maNV, thang, nam);
    }
}
