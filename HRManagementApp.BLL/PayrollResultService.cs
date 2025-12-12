namespace HRManagementApp.BLL;

using HRManagementApp.models;
using System.Linq; 
using System.Collections.Generic;
using System;

public class PayrollResultService
{
    
    public ChamCongService _chamCongService;
    public DonTuService _donTuService;
    private LuongService _luongService;
    public PayrollResultService()
    {
        _chamCongService = new ChamCongService();
        _donTuService = new DonTuService();
        _luongService = new LuongService();
    }

    public PayrollResult GetPayrollResultForEmployee(NhanVien nhanVien, int Month, int Year)
    {
        var result = new PayrollResult
        {
            maNV = nhanVien.MaNV,
            TenNV = nhanVien.HoTen,
            Thang = Month,
            Nam = Year,
            maPB = nhanVien.MaPB,
        };

        // ============================
        // 1. TÍNH LƯƠNG CƠ BẢN & HỆ SỐ
        // ============================
        decimal? luongCoBan = 0;
        decimal? heSoLuongCoBan = 0;
        decimal? tongTienKiemNhiem = 0;
        decimal? heSoKiemNhiem = 0;    
        // Kiểm tra null cho danh sách chức vụ
        if (nhanVien.DanhSachChucVu != null)
        {
            foreach (var vt in nhanVien.DanhSachChucVu)
            {
                if (vt.ChucVu != null)
                {
                    heSoLuongCoBan += vt.HeSoLuongCoBan ?? 0;
                    luongCoBan += vt.ChucVu.LuongCB * (vt.HeSoLuongCoBan ?? 0);

                    heSoKiemNhiem += vt.HeSoPhuCapKiemNhiem ?? 0;
                    tongTienKiemNhiem +=
                        (vt.HeSoPhuCapKiemNhiem ?? 0)  * vt.ChucVu.TienPhuCapKiemNhiem;
                }
            }
        }
        result.HeSoLuongCB = heSoLuongCoBan;
        result.LuongCoBan = luongCoBan;
        result.TongTienKiemNhiem = tongTienKiemNhiem;
        result.HeSoKiemNhiem = heSoKiemNhiem;
        
        // ============================
        // 2. PHỤ CẤP (lọc theo tháng) - Thêm check null (?.)
        // ============================
        decimal tongPhuCap = nhanVien.PhuCaps?
            .Where(p =>
                p.ApDungTuNgay <= new DateTime(Year, Month, 1) &&
                (p.ApDungDenNgay == null || p.ApDungDenNgay >= new DateTime(Year, Month, 1)))
            .Sum(p => p.SoTien) ?? 0; // Nếu null thì trả về 0

        result.TongPhuCap = tongPhuCap;

        // ============================
        // 3. THUẾ - Thêm check null (?.)
        // ============================
        decimal tongThue = nhanVien.Thues?
            .Where(t =>
                t.ApDungTuNgay <= new DateTime(Year, Month, 1) &&
                (t.ApDungDenNgay == null || t.ApDungDenNgay >= new DateTime(Year, Month, 1)))
            .Sum(t => t.SoTien) ?? 0;

        result.TongThue = tongThue;

        // ============================
        // 4. KHẤU TRỪ - Thêm check null (?.)
        // ============================
        decimal tongKhauTru = nhanVien.KhauTrus?
            .Where(k => k.Ngay.Month == Month && k.Ngay.Year == Year)
            .Sum(k => k.SoTien) ?? 0;

        result.TongKhauTru = tongKhauTru;

        // ============================
        // 5. SỐ NGÀY CÔNG & TRẠNG THÁI 
        // ============================
        var luongRecord = nhanVien.Luongs?
            .FirstOrDefault(l => l.Thang == Month && l.Nam == Year);
        
        if (luongRecord != null)
        {
            result.maLuong = luongRecord.MaLuong; 
            result.TrangThai = luongRecord.TrangThai; 
        }
        else
        {
            result.TrangThai = "Chưa trả";
        }
        // ============================
        //  TÍNH TỔNG NGÀY CÔNG 
        // ============================
        
        KetQuaChamCong ketQuachamcong = new KetQuaChamCong();
        ketQuachamcong = _chamCongService.GetChamCongStatistics(nhanVien.MaNV,Month,Year);

        KetQuaNghi ketQuaNghi = new KetQuaNghi();
        ketQuaNghi = _donTuService.GetSoNgayNghi(nhanVien.MaNV, Month, Year);

        ketQuachamcong.SoNgayDiLam += ketQuaNghi.NghiCoLuong;
        ketQuachamcong.SoGioDiLam += 8* ketQuaNghi.NghiCoLuong;
        result.TongNgayCong = ketQuachamcong.SoNgayDiLam;
        
        // ============================
        // 6. TÍNH LƯƠNG CUỐI
        // ============================
        decimal? luongChinh = luongCoBan + tongTienKiemNhiem;
        luongChinh = luongChinh / (26*8) * ketQuachamcong.SoGioDiLam  ; 
        
        result.luongchinh = luongChinh;
        
        //viết hàm lưu các hệ số cần thiết vào bảng lương ở đây 
        result.LuongThucNhan =
            luongChinh
            + tongPhuCap
            - tongKhauTru
            - tongThue;

        
        return result;
    }
}