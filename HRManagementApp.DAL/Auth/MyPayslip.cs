using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL
{
    public class MyPayslipDAL
    {
        // 1. Lấy danh sách Tháng/Năm có dữ liệu lương của nhân viên này
        public DataTable GetAvailableMonths(int maNV)
        {
            string query = "SELECT DISTINCT Thang, Nam FROM luong WHERE MaNV = @MaNV ORDER BY Nam DESC, Thang DESC";
            var param = new Dictionary<string, object> { { "@MaNV", maNV } };
            return Database.ExecuteQuery(query, param);
        }

        // 2. Lấy chi tiết phiếu lương
        public MyPayslipDTO GetPayslipDetail(int maNV, int month, int year)
        {
            // Query phức tạp để lấy tổng hợp
            string query = $@"
                SELECT 
                    l.Thang, l.Nam, l.TongNgayCong, l.TienLuong, l.LuongThucNhan, l.TrangThai,
                    nv.HoTen, pb.TenPB, cv.TenCV, cv.LuongCB,
                    nvcv.HeSoPhuCapKiemNhiem,
                    -- Tính tổng phụ cấp trong tháng
                    (SELECT COALESCE(SUM(SoTien), 0) FROM phucap_nhanvien pc 
                     WHERE pc.MaNV = l.MaNV AND pc.ApDungTuNgay <= LAST_DAY('{year}-{month}-01')) AS TongPhuCap,
                    -- Tính tổng thuế
                    (SELECT COALESCE(SUM(SoTien), 0) FROM thue t 
                     WHERE t.MaNV = l.MaNV AND MONTH(t.ApDungTuNgay) = {month}) AS TongThue,
                    -- Tính tổng khấu trừ (Bảo hiểm + Khác + Ứng)
                    (SELECT COALESCE(SUM(SoTien), 0) FROM khautru kt 
                     WHERE kt.MaNV = l.MaNV AND MONTH(kt.Ngay) = {month} AND YEAR(kt.Ngay) = {year}) AS TongKhauTru
                FROM luong l
                JOIN nhanvien nv ON l.MaNV = nv.MaNV
                LEFT JOIN phongban pb ON nv.MaPB = pb.MaPB
                LEFT JOIN nhanvien_chucvu nvcv ON nv.MaNV = nvcv.MaNV AND nvcv.LoaiChucVu = 'Chính thức'
                LEFT JOIN chucvu cv ON nvcv.MaCV = cv.MaCV
                WHERE l.MaNV = @MaNV AND l.Thang = @Month AND l.Nam = @Year";

            var param = new Dictionary<string, object>
            {
                { "@MaNV", maNV }, { "@Month", month }, { "@Year", year }
            };

            DataTable dt = Database.ExecuteQuery(query, param);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                var dto = new MyPayslipDTO();

                dto.Thang = month;
                dto.Nam = year;
                dto.TuNgay = new DateTime(year, month, 1);
                dto.DenNgay = dto.TuNgay.AddMonths(1).AddDays(-1); // Ngày cuối tháng

                dto.HoTen = row["HoTen"].ToString();
                dto.TenPB = row["TenPB"] != DBNull.Value ? row["TenPB"].ToString() : "N/A";
                dto.TenChucVu = row["TenCV"] != DBNull.Value ? row["TenCV"].ToString() : "N/A";
                
                // Thu nhập
                dto.LuongCoBan = Convert.ToDecimal(row["LuongCB"]);
                // Giả sử 1 ngày công = 8 giờ
                int ngayCong = Convert.ToInt32(row["TongNgayCong"]);
                dto.TongGioLam = ngayCong * 8; 
                dto.LuongTheoNgayCong = Convert.ToDecimal(row["TienLuong"]); // Lương tính theo ngày đi làm thực tế
                dto.TongPhuCap = Convert.ToDecimal(row["TongPhuCap"]);

                // Kiêm nhiệm
                double heso = row["HeSoPhuCapKiemNhiem"] != DBNull.Value ? Convert.ToDouble(row["HeSoPhuCapKiemNhiem"]) : 0;
                dto.CoKiemNhiem = heso > 0;
                dto.HeSoKiemNhiem = heso;
                // Giả sử tiền kiêm nhiệm tính = % Lương CB (hoặc logic riêng của bạn)
                if(dto.CoKiemNhiem) dto.TienKiemNhiem = dto.LuongCoBan * (decimal)heso;

                // Khấu trừ
                dto.TongThue = Convert.ToDecimal(row["TongThue"]);
                decimal tongKhauTruRaw = Convert.ToDecimal(row["TongKhauTru"]);
                
                // Giả định: Trong bảng KhauTru, nếu TenKhoanTru like 'Ung%' thì là Tiền Ứng
                // Ở đây tách đơn giản để demo, thực tế cần query kỹ hơn
                dto.TienUng = 0; // Cần query riêng nếu muốn tách bạch hoàn toàn
                dto.TongBaoHiem = tongKhauTruRaw; // Tạm gán tổng
                
                dto.ThucLanh = Convert.ToDecimal(row["LuongThucNhan"]);
                dto.TrangThai = row["TrangThai"].ToString();

                return dto;
            }

            return null;
        }
    }
}