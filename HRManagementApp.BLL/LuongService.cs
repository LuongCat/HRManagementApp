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

    public bool DeleteSalary(int maLuong)
    {
        return _luongRepository.DeleteSalary(maLuong);
    }

    public bool UpdateTrangThai(int maLuong, string trangThaiMoi)
    {
        return _luongRepository.UpdateTrangThai(maLuong, trangThaiMoi);
    }

    public bool UpdateChotLuong(int maLuong, string trangThaiChot)
    {
        return _luongRepository.UpdateChotLuong(maLuong, trangThaiChot);
    }

    public string GetChotLuongStatus(int maNV, int thang, int nam)
    {
        return _luongRepository.GetChotLuongStatus(maNV, thang, nam);
    }

    public string GetChotLuongStatusByMaLuong(int maLuong)
    {
        return _luongRepository.GetChotLuongStatusByMaLuong(maLuong);
    }

    public int AddSalaryReturnID(Luong luong)
    {
        return _luongRepository.AddSalaryReturnID(luong);
    }
    public bool UnLockSalary(int maNV, DateTime date)
    {
        try
        {
            int month = date.Month;
            int year = date.Year;

            // 1. Tìm bảng lương
            var luong = GetSalaryByMonthYear(maNV, month, year);

            // 2. Kiểm tra
            if (luong != null)
            {
                if (luong.ChotLuong == "Chưa chốt") return false;

                // 3. Thực hiện update
                // Đồng bộ chữ hoa/thường với DB: "Chưa chốt", "Chưa thanh toán"
                bool kq1 = UpdateChotLuong(luong.MaLuong, "Chưa chốt");
                bool kq2 = UpdateTrangThai(luong.MaLuong, "Chưa trả");

                return kq1 && kq2; // Trả về true nếu update thành công
            }

            return false; // Không có bảng lương tháng này
        }
        catch (Exception ex)
        {
            // Ghi log lỗi nếu cần
            Console.WriteLine("Lỗi Service Unlock: " + ex.Message);
            return false;
        }
    }
    
}