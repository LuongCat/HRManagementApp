namespace HRManagementApp.BLL;

using HRManagementApp.models;

public class PayrollResultService
{
    public NhanVienService NhanVienService { get; set; }
    public LuongService LuongService { get; set; }
    public PhuCapService PhuCapService { get; set; }
    public ThueService ThueService { get; set; }

    public PayrollResultService()
    {
        PhuCapService = new PhuCapService();
    }

    public PayrollResult GetPayrollResultForEmployee(NhanVien nhanVien, int Month, int Year)
    {
        var result = new PayrollResult
        {
            maNV = nhanVien.MaNV,
            TenNV = nhanVien.HoTen,
            Thang = Month,
            Nam = Year
        };

        // ============================
        // 1. TÍNH LƯƠNG CƠ BẢN & HỆ SỐ
        // ============================
        decimal? luongCoBan = 0;
        decimal? heSoLuongCoBan = 0;
        decimal? tongTienKiemNhiem = 0;

        foreach (var vt in nhanVien.DanhSachChucVu)
        {
            heSoLuongCoBan += vt.HeSoLuongCoBan ?? 0;
            luongCoBan += vt.ChucVu.LuongCB;

            // Phụ cấp kiêm nhiệm từ hệ số + số tiền cố định
            tongTienKiemNhiem +=
                (vt.HeSoPhuCapKiemNhiem ?? 0) * vt.ChucVu.LuongCB
                + vt.ChucVu.TienPhuCapKiemNhiem;
        }

        result.LuongCoBan = luongCoBan;
        result.HeSoLuongCB = heSoLuongCoBan;
        result.TongTienKiemNhiem = tongTienKiemNhiem;


        // ============================
        // 2. PHỤ CẤP (lọc theo tháng)
        // ============================
        decimal tongPhuCap = nhanVien.PhuCaps
            .Where(p =>
                p.ApDungTuNgay <= new DateTime(Year, Month, 1) &&
                (p.ApDungDenNgay == null || p.ApDungDenNgay >= new DateTime(Year, Month, 1)))
            .Sum(p => p.SoTien);

        result.TongPhuCap = tongPhuCap;


        // ============================
        // 3. THUẾ (tính theo thời gian hiệu lực)
        // ============================
        decimal tongThue = nhanVien.Thues
            .Where(t =>
                t.ApDungTuNgay <= new DateTime(Year, Month, 1) &&
                (t.ApDungDenNgay == null || t.ApDungDenNgay >= new DateTime(Year, Month, 1)))
            .Sum(t => t.SoTien);

        result.TongThue = tongThue;


        // ============================
        // 4. KHAU TRỪ (lọc trong tháng)
        // ============================
        decimal tongKhauTru = nhanVien.KhauTrus
            .Where(k => k.Ngay.Month == Month && k.Ngay.Year == Year)
            .Sum(k => k.SoTien);

        result.TongKhauTru = tongKhauTru;


        // ============================
        // 5. SỐ NGÀY CÔNG (từ bảng Luongs)
        // ============================
        var luongRecord = nhanVien.Luongs
            .FirstOrDefault(l => l.Thang == Month && l.Nam == Year);

        result.TongNgayCong = luongRecord?.TongNgayCong ?? 0;


        // ============================
        // 6. TÍNH LƯƠNG CUỐI
        // ============================
        decimal? luongChinh = luongCoBan * heSoLuongCoBan;

        result.LuongThucNhan =
            luongChinh
            + tongPhuCap
            + tongTienKiemNhiem
            - tongKhauTru
            - tongThue;

        return result;
    }
}