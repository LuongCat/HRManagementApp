using HRManagementApp.DAL;
using HRManagementApp.models;
using System;

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

            // Gọi DAL kiểm tra
            bool isSuccess = dal.Login(username, password);

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
            bool isSuccess = dal.ChangePassword(UserID,newPass,oldPass);
            
            return isSuccess;
        }
    }
}