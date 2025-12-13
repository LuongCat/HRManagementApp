using HRManagementApp.DAL;
using HRManagementApp.models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace HRManagementApp.BLL
{
    public class AuthenticationBLL
    {
        private AuthenticationDAL dal = new AuthenticationDAL();

        public bool Login(string username, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Vui lòng nhập tên đăng nhập.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Vui lòng nhập mật khẩu.";
                return false;
            }

            string haspassword = HashPassword(password);
            //string haspassword = password;
            bool isSuccess = dal.Login(username, haspassword);

            if (!isSuccess)
            {
                errorMessage = "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.";
            }

            return isSuccess;
        }

        public void Logout()
        {
            UserSession.Clear();
        }

        public bool ChangePassword(int UserID,  string newPass, string oldPass)
        {

            // Gọi DAL kiểm tra
            string haspassword = HashPassword(oldPass);
            bool isSuccess = dal.ChangePassword(UserID,newPass,oldPass);
            
            return isSuccess;
        }
        public string HashPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2")); // Chuyển sang chuỗi Hex in hoa
                }
                return sb.ToString();
            }
        }
    }
}