using HRManagementApp.DAL;
using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
namespace HRManagementApp.BLL
{
    public class AccountBLL
    {
        private AccountManagementDAL dal = new AccountManagementDAL();

        // Hàm lấy danh sách (Tên hàm khớp với AccountManagementView.xaml.cs)
        public List<AccountManagementModel> getAllAccountModelBLL()
        {
            return dal.GetAllAccounts();
        }

        // Hàm tìm kiếm (Nếu view gọi logic tìm kiếm DB, nhưng hiện tại View đang filter trên List nên hàm này optional)
        public List<AccountManagementModel> SearchAccounts(string keyword)
        {
            // Logic tìm kiếm nếu cần thiết chuyển xuống DB
            return getAllAccountModelBLL();
        }

        // Thêm vào class AccountBLL

        // Hàm cập nhật thông tin (Sửa)
        // Thêm vào class AccountBLL


        public List<string> GetRoleList()
        {
            List<string> roles = new List<string>();
            DataTable dt = dal.GetRoles();
            foreach (DataRow row in dt.Rows)
            {
                roles.Add(row["TenVaiTro"].ToString());
            }
            return roles;
        }

        public List<string> GetDepartmentList()
        {
            List<string> depts = new List<string>();
            DataTable dt = dal.GetDepartments();
            foreach (DataRow row in dt.Rows)
            {
                depts.Add(row["TenPB"].ToString());
            }
            return depts;
        }

        public bool UpdateFullAccountBLL(AccountManagementModel acc)
        {
            if (acc == null || acc.MaTK == 0) return false;
            try
            {
                return dal.UpdateFullAccountDetails(acc);
            }
            catch (Exception ex)
            {
                throw ex; // Ném lỗi (ví dụ trùng username) ra cho View xử lý
            }
        }
        // Hàm khóa/mở khóa
        public bool LockUnlockAccountBLL(int maTK, string currentStatus)
        {
            string newStatus = (currentStatus == "Hoạt động") ? "Đã khóa" : "Hoạt động";
            return dal.UpdateAccountStatus(maTK, newStatus);
        }

        public DataTable GetEmployeesForNewAccount()
        {
            return dal.GetEmployeesWithoutAccount();
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

        // 2. Lấy danh sách nhân viên cho Combobox
        public DataTable GetEmployeesForCombo()
        {
            return dal.GetEmployeesWithoutAccount();
        }

        // 3. Lấy thông tin chi tiết 1 nhân viên
        public DataRow GetEmployeeInfo(int maNV)
        {
            DataTable dt = dal.GetEmployeeDetails(maNV);
            if (dt.Rows.Count > 0) return dt.Rows[0];
            return null;
        }

        // 4. Lấy danh sách vai trò
        public List<string> GetRoles()
        {
            List<string> roles = new List<string>();
            DataTable dt = dal.GetRoles();
            foreach (DataRow row in dt.Rows)
            {
                roles.Add(row["TenVaiTro"].ToString());
            }
            return roles;
        }

        // 5. Thêm tài khoản (Có kiểm tra và Hash)
        public string AddAccountBLL(AccountManagementModel acc, string rawPassword, string confirmPassword)
        {
            // Validation
            if (string.IsNullOrEmpty(acc.TenDangNhap)) return "Tên đăng nhập không được để trống.";
            if (acc.MaNV == null || acc.MaNV == 0) return "Vui lòng chọn nhân viên.";
            if (string.IsNullOrEmpty(rawPassword)) return "Mật khẩu không được để trống.";
            if (rawPassword != confirmPassword) return "Mật khẩu xác nhận không khớp.";
            if (rawPassword.Length < 6) return "Mật khẩu phải từ 6 ký tự trở lên.";

            if (dal.CheckUsernameExists(acc.TenDangNhap)) return "Tên đăng nhập đã tồn tại.";

            // Hash mật khẩu trước khi lưu
            acc.MatKhau = HashPassword(rawPassword);

            // Gọi DAL
            if (dal.AddAccount(acc))
                return "Success"; // Trả về chuỗi đặc biệt để nhận biết thành công
            else
                return "Lỗi hệ thống khi thêm tài khoản.";
        }
    }
}