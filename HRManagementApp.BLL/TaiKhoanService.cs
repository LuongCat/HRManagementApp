using HRManagementApp.DAL;
using HRManagementApp.Models;
using System.Security.Cryptography;
using System.Text;

namespace HRManagementApp.BLL
{
    public class TaiKhoanService
    {
        private readonly TaiKhoanRepository _repo = new TaiKhoanRepository();

        public bool DangNhap(string username, string password)
        {
            
            var tk = _repo.GetTaiKhoanByUsername(username);
            if (tk == null || tk.TrangThai != "Hoạt động") return false;

            // hash dạng SHA256
            string hash = ComputeSha256Hash(password);
            //debug
            Console.WriteLine(hash);
            return hash == tk.MatKhau;
        }

    //hàm hash
    private string ComputeSha256Hash(string rawData)
    {
    using SHA256 sha256Hash = SHA256.Create();
    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

    StringBuilder builder = new StringBuilder();
    foreach (byte b in bytes)
    {
        builder.Append(b.ToString("x2"));
    }

    return builder.ToString();
    }


    }
}
