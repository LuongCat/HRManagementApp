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
        // 1. Lấy thông tin nhân viên để hiển thị tên
        // (Ở đây gọi tạm hàm, thực tế bạn nên có hàm GetEmployeeInfo trong NhanVienDAL)
        // Giả sử ta lấy được tên, nếu không thì hiển thị Mã
        NhanVienService nvsv = new NhanVienService();
        string empName = nvsv.GetEmployeeById(maNV).HoTen;

        // 2. Lấy trạng thái hôm nay
        var todayRecord = _chamCongRepository.GetTodayRecord(maNV);

        if (todayRecord == null)
        {
            // Chưa có dữ liệu -> Chấm công VÀO
            bool result = _chamCongRepository.CheckIn(maNV);
            return (result, result ? "Chấm công VÀO thành công!" : "Lỗi hệ thống", empName, "CheckIn");
        }
        else if (todayRecord.GioRa == null)
        {
            // Đã vào, chưa ra -> Chấm công RA
            // Kiểm tra: Nếu vừa check-in cách đây ít hơn 1 phút thì chặn (tránh quét đúp)
            if (todayRecord.GioVao.HasValue && (DateTime.Now.TimeOfDay - todayRecord.GioVao.Value).TotalMinutes < 1)
            {
                return (false, "Bạn vừa chấm công vào, vui lòng chờ thêm!", empName, "Wait");
            }

            bool result = _chamCongRepository.CheckOut(todayRecord.MaCC);
            return (result, result ? "Chấm công RA thành công!" : "Lỗi hệ thống", empName, "CheckOut");
        }
        else
        {
            // Đã xong cả vào và ra
            return (false, "Bạn đã hoàn thành ca làm việc hôm nay!", empName, "Done");
        }
    }
}
