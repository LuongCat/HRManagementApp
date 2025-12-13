namespace HRManagementApp.models;

using System;

public class SystemLog
{
    public int LogID { get; set; }
    public DateTime ThoiGian { get; set; }
    public string NguoiThucHien { get; set; }
    public string HanhDong { get; set; }      // VD: INSERT, UPDATE, DELETE
    public string BangLienQuan { get; set; }  // VD: ChamCong, NhanVien
    public string MaBanGhi { get; set; }      // VD: "1001" (ID của dòng bị sửa)
    public string MoTa { get; set; }

    // Constructor mặc định (khởi tạo thời gian là hiện tại)
    public SystemLog()
    {
        ThoiGian = DateTime.Now;
    }

    // Constructor tiện lợi để tạo nhanh đối tượng log
    public SystemLog(string nguoiThucHien, string hanhDong, string bang, string maBanGhi, string moTa)
    {
        ThoiGian = DateTime.Now;
        NguoiThucHien = nguoiThucHien;
        HanhDong = hanhDong;
        BangLienQuan = bang;
        MaBanGhi = maBanGhi;
        MoTa = moTa;
    }
}