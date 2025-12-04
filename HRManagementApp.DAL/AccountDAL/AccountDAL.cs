using HRManagementApp.models; // Nhớ đổi namespace trỏ về nơi chứa AccountModel/AccountModel của bạn
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL
{
    public class AccountDAL
    {
        // 1. Kiểm tra đăng nhập
        // Trả về đối tượng AccountModel nếu thành công, null nếu thất bại
        public AccountModel? Login(string username, string passwordHash)
        {
            string query = "SELECT * FROM taikhoan WHERE TenDangNhap = @user AND MatKhau = @pass AND TrangThai = 'Hoạt động'";
            
            var parameters = new Dictionary<string, object>
            {
                { "@user", username },
                { "@pass", passwordHash }
            };

            DataTable dt = Database.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                return MapRowToDTO(dt.Rows[0]);
            }

            return null;
        }

        // 2. Lấy danh sách tất cả tài khoản (kèm tên nhân viên)
        public List<AccountModel> GetAllAccounts()
        {
            List<AccountModel> list = new List<AccountModel>();
            
            // Join với bảng nhân viên để lấy Họ Tên hiển thị cho đẹp
            string query = @"SELECT tk.*, nv.HoTen 
                             FROM taikhoan tk 
                             LEFT JOIN nhanvien nv ON tk.MaNV = nv.MaNV 
                             WHERE tk.TrangThai != 'Đã xóa'";

            DataTable dt = Database.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToDTO(row));
            }

            return list;
        }

        // 3. Thêm tài khoản mới
        public bool AddAccount(AccountModel acc)
        {
            string query = @"INSERT INTO taikhoan (MaNV, TenDangNhap, MatKhau, TrangThai) 
                             VALUES (@MaNV, @User, @Pass, @Status)";

            var parameters = new Dictionary<string, object>
            {
                { "@MaNV", acc.MaNV ?? (object)DBNull.Value }, // Xử lý null nếu là Admin hệ thống
                { "@User", acc.TenDangNhap },
                { "@Pass", acc.MatKhau },
                { "@Status", acc.TrangThai }
            };

            int result = Database.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // 4. Cập nhật tài khoản (Đổi mật khẩu, trạng thái, hoặc gán lại nhân viên)
        public bool UpdateAccount(AccountModel acc)
        {
            string query = @"UPDATE taikhoan 
                             SET MatKhau = @Pass, 
                                 TrangThai = @Status,
                                 MaNV = @MaNV
                             WHERE MaTK = @MaTK";

            var parameters = new Dictionary<string, object>
            {
                { "@Pass", acc.MatKhau },
                { "@Status", acc.TrangThai },
                { "@MaNV", acc.MaNV ?? (object)DBNull.Value },
                { "@MaTK", acc.MaTK }
            };

            int result = Database.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // 5. Xóa tài khoản (Xóa mềm - Chuyển trạng thái sang 'Đã xóa')
        public bool DeleteAccount(int maTK)
        {
            string query = "UPDATE taikhoan SET TrangThai = 'Đã xóa' WHERE MaTK = @MaTK";
            
            var parameters = new Dictionary<string, object>
            {
                { "@MaTK", maTK }
            };

            int result = Database.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // 6. Kiểm tra tên đăng nhập đã tồn tại chưa (Dùng khi thêm mới)
        public bool CheckUsernameExists(string username)
        {
            string query = "SELECT MaTK FROM taikhoan WHERE TenDangNhap = @User LIMIT 1";
            
            var parameters = new Dictionary<string, object>
            {
                { "@User", username }
            };

            DataTable dt = Database.ExecuteQuery(query, parameters);
            return dt.Rows.Count > 0;
        }

        // 7. Lấy danh sách vai trò (Quyền) của một tài khoản
        // Hàm này quan trọng để biết tài khoản đó là Admin hay Nhân viên
        public List<int> GetRolesByAccountID(int maTK)
        {
            List<int> roles = new List<int>();
            string query = "SELECT MaVaiTro FROM taikhoan_vaitro WHERE MaTK = @MaTK";

            var parameters = new Dictionary<string, object>
            {
                { "@MaTK", maTK }
            };

            DataTable dt = Database.ExecuteQuery(query, parameters);
            
            foreach(DataRow row in dt.Rows)
            {
                roles.Add(Convert.ToInt32(row["MaVaiTro"]));
            }
            
            return roles;
        }

        // --- HÀM PHỤ TRỢ (PRIVATE) ---
        
        // Hàm này giúp chuyển đổi 1 dòng dữ liệu (DataRow) thành đối tượng (AccountModel)
        // Giúp code gọn hơn, không phải viết lại việc convert nhiều lần
        private AccountModel MapRowToDTO(DataRow row)
        {
            AccountModel acc = new AccountModel();
            
            acc.MaTK = Convert.ToInt32(row["MaTK"]);
            acc.TenDangNhap = row["TenDangNhap"].ToString();
            acc.MatKhau = row["MatKhau"].ToString();
            acc.TrangThai = row["TrangThai"].ToString();

            // Xử lý giá trị NULL của MaNV
            if (row["MaNV"] != DBNull.Value)
            {
                acc.MaNV = Convert.ToInt32(row["MaNV"]);
            }
            else
            {
                acc.MaNV = null;
            }

            // Kiểm tra xem cột HoTen có tồn tại trong kết quả truy vấn không (do lệnh SELECT có thể khác nhau)
            if (row.Table.Columns.Contains("HoTen") && row["HoTen"] != DBNull.Value)
            {
                acc.TenNhanVien = row["HoTen"].ToString();
            }

            return acc;
        }

        
    }
}