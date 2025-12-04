using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRManagementApp.models
{ 
    public class AccountModel
    {
        public AccountModel()
        {
        }

        public AccountModel(String TenDangNhap, String Name, String SDT, String VaiTro, String PhongBan, String TrangThai)
        {
            this.TenDangNhap = TenDangNhap;
            this.Name = Name; 
            this.SDT = SDT;
            this.VaiTro = VaiTro;
            this.PhongBan = PhongBan;
            this.TrangThai = TrangThai;
        }

        public String TenDangNhap { get; set; }
        public String Name { get; set; }
        public String SDT { get; set; } 
        public String VaiTro { get; set; }
        public String PhongBan { get; set; }
        public String TrangThai { get; set; }
    }
}