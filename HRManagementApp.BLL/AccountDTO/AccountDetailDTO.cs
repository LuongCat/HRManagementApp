using System;

namespace HRManagementApp.BLL.AccountDetailDTO
{
    public class AccountDetailDTO
    {
        public AccountDetailDTO()
        {
        }

        public AccountDetailDTO(String TenDangNhap, String Name, String Email, String VaiTro, String PhongBan, String TrangThai)
        {
            this.TenDangNhap = TenDangNhap;
            this.Name = Name; 
            this.Email = Email;
            this.VaiTro = VaiTro;
            this.PhongBan = PhongBan;
            this.TrangThai = TrangThai;
        }

        public String TenDangNhap { get; set; }
        public String Name { get; set; }
        public String Email { get; set; } 
        public String VaiTro { get; set; }
        public String PhongBan { get; set; }
        public String TrangThai { get; set; }
    }
}