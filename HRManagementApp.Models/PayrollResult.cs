namespace HRManagementApp.models;

public class PayrollResult
{
    public int maNV {get; set;}
    public int maLuong {get; set;}
    public string TenNV {get; set;}
    
    public int Thang { get; set; }
    public int Nam { get; set; }
    
    public decimal LuongCoBan { get; set; }
    public decimal HeSoLuongCB {get; set;}
    public decimal TongTienKiemNhiem {get; set;}//phải có tính toán đầy đủ các hệ số rồi
    
    public decimal TongPhuCap { get; set; }
    public decimal TongKhauTru { get; set; }
    public decimal TongThue { get; set; }
    
    public int TongNgayCong { get; set; }
    public decimal LuongThucNhan { get; set; }
}
