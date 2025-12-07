using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL
{
    public class AccountManagementDAL
    {
        // 1. Lấy danh sách tài khoản đầy đủ thông tin hiển thị
        public List<AccountManagementModel> GetAllAccounts()
        {
            List<AccountManagementModel> list = new List<AccountManagementModel>();

            // Query kết hợp 4 bảng để lấy đủ thông tin cho giao diện
            string query = @"
                SELECT 
                    tk.MaTK, 
                    tk.TenDangNhap, 
                    tk.MatKhau, 
                    tk.TrangThai, 
                    tk.MaNV,
                    nv.HoTen AS Name, 
                    nv.DienThoai AS SDT, 
                    pb.TenPB AS PhongBan,
                    vt.TenVaiTro AS VaiTro
                FROM taikhoan tk
                left JOIN nhanvien nv ON tk.MaNV = nv.MaNV
                left JOIN phongban pb ON nv.MaPB = pb.MaPB
                left JOIN taikhoan_vaitro tkvt ON tk.MaTK = tkvt.MaTK
                left JOIN vaitro vt ON tkvt.MaVaiTro = vt.MaVaiTro
                WHERE tk.TrangThai != 'Đã xóa'
                ORDER BY tk.MaTK DESC";

            DataTable dt = Database.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                AccountManagementModel acc = new AccountManagementModel();
                acc.MaTK = Convert.ToInt32(row["MaTK"]);
                acc.TenDangNhap = row["TenDangNhap"].ToString();
                acc.MatKhau = row["MatKhau"].ToString();
                acc.TrangThai = row["TrangThai"].ToString();

                // Xử lý null cho nhân viên (ví dụ tài khoản admin hệ thống không gắn NV)
                if (row["MaNV"] != DBNull.Value)
                    acc.MaNV = Convert.ToInt32(row["MaNV"]);
                else
                    acc.MaNV = null;

                // Map các trường hiển thị (Xử lý null nếu không có thông tin)
                acc.Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : "N/A";
                acc.SDT = row["SDT"] != DBNull.Value ? row["SDT"].ToString() : "";
                acc.PhongBan = row["PhongBan"] != DBNull.Value ? row["PhongBan"].ToString() : "Chưa phân bổ";
                acc.VaiTro = row["VaiTro"] != DBNull.Value ? row["VaiTro"].ToString() : "Nhân viên";
                list.Add(acc);
            }

            return list;
        }
        // --- MỚI: Lấy danh sách Vai trò cho ComboBox ---
        public DataTable GetRoles()
        {
            return Database.ExecuteQuery("SELECT MaVaiTro, TenVaiTro FROM vaitro");
        }

        // --- MỚI: Lấy danh sách Phòng ban cho ComboBox ---
        public DataTable GetDepartments()
        {
            return Database.ExecuteQuery("SELECT MaPB, TenPB FROM phongban");
        }

        // --- CẬP NHẬT: Hàm chỉnh sửa đầy đủ ---
        public bool UpdateFullAccountDetails(AccountManagementModel acc)
        {
            // 1. Cập nhật bảng taikhoan (Cho phép sửa TenDangNhap, MatKhau, TrangThai)
            string queryAccount = @"UPDATE taikhoan 
                                    SET MatKhau = @MatKhau, 
                                        TrangThai = @TrangThai,
                                        TenDangNhap = @TenDangNhap 
                                    WHERE MaTK = @MaTK";

            var paramsAccount = new Dictionary<string, object>
            {
                { "@MatKhau", acc.MatKhau },
                { "@TrangThai", acc.TrangThai },
                { "@TenDangNhap", acc.TenDangNhap },
                { "@MaTK", acc.MaTK }
            };

            // Kiểm tra trùng tên đăng nhập (trừ chính nó)
            string checkUserQuery = "SELECT COUNT(*) FROM taikhoan WHERE TenDangNhap = @User AND MaTK != @MaTK";
            DataTable dtCheck = Database.ExecuteQuery(checkUserQuery, new Dictionary<string, object> {
                { "@User", acc.TenDangNhap },
                { "@MaTK", acc.MaTK }
            });

            if (Convert.ToInt32(dtCheck.Rows[0][0]) > 0)
            {
                throw new Exception("Tên đăng nhập đã tồn tại!");
            }

            Database.ExecuteNonQuery(queryAccount, paramsAccount);

            // 2. Cập nhật bảng nhanvien (Họ tên, SĐT, Phòng Ban)
            if (acc.MaNV != null)
            {
                // Lấy MaPB dựa trên tên Phòng ban (Vì Model đang lưu Tên)
                string queryGetPB = "SELECT MaPB FROM phongban WHERE TenPB = @TenPB LIMIT 1";
                var paramPB = new Dictionary<string, object> { { "@TenPB", acc.PhongBan } };
                DataTable dtPB = Database.ExecuteQuery(queryGetPB, paramPB);

                object maPB = DBNull.Value;
                if (dtPB.Rows.Count > 0) maPB = dtPB.Rows[0]["MaPB"];

                string queryNhanVien = @"UPDATE nhanvien 
                                         SET HoTen = @HoTen, 
                                             DienThoai = @SDT,
                                             MaPB = @MaPB
                                         WHERE MaNV = @MaNV";
                var paramsNV = new Dictionary<string, object>
                {
                    { "@HoTen", acc.Name },
                    { "@SDT", acc.SDT },
                    { "@MaPB", maPB },
                    { "@MaNV", acc.MaNV }
                };
                Database.ExecuteNonQuery(queryNhanVien, paramsNV);
            }

            // 3. Cập nhật vai trò
            // Lấy ID vai trò từ tên
            string queryGetRoleID = "SELECT MaVaiTro FROM vaitro WHERE TenVaiTro = @TenVaiTro LIMIT 1";
            var paramRoleGet = new Dictionary<string, object> { { "@TenVaiTro", acc.VaiTro } };
            DataTable dtRole = Database.ExecuteQuery(queryGetRoleID, paramRoleGet);

            if (dtRole.Rows.Count > 0)
            {
                int roleId = Convert.ToInt32(dtRole.Rows[0]["MaVaiTro"]);

                // Kiểm tra đã có trong taikhoan_vaitro chưa
                string queryCheckRole = "SELECT * FROM taikhoan_vaitro WHERE MaTK = @MaTK";
                DataTable dtExist = Database.ExecuteQuery(queryCheckRole, new Dictionary<string, object> { { "@MaTK", acc.MaTK } });

                if (dtExist.Rows.Count > 0)
                {
                    string queryUpdateRole = "UPDATE taikhoan_vaitro SET MaVaiTro = @MaVaiTro WHERE MaTK = @MaTK";
                    Database.ExecuteNonQuery(queryUpdateRole, new Dictionary<string, object> { { "@MaVaiTro", roleId }, { "@MaTK", acc.MaTK } });
                }
                else
                {
                    string queryInsertRole = "INSERT INTO taikhoan_vaitro (MaTK, MaVaiTro) VALUES (@MaTK, @MaVaiTro)";
                    Database.ExecuteNonQuery(queryInsertRole, new Dictionary<string, object> { { "@MaTK", acc.MaTK }, { "@MaVaiTro", roleId } });
                }
            }

            return true;
        }

        public bool UpdateAccountStatus(int maTK, string newStatus)
        {
            string query = "UPDATE taikhoan SET TrangThai = @TrangThai WHERE MaTK = @MaTK";

            var parameters = new Dictionary<string, object>
    {
        { "@TrangThai", newStatus },
        { "@MaTK", maTK }
    };

            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        // public DataTable GetEmployeesWithoutAccount()
        // {
        //     // Lấy những nhân viên chưa tồn tại trong bảng Taikhoan
        //     string query = @"
        // SELECT nv.MaNV, nv.HoTen, nv.DienThoai, pb.TenPB 
        // FROM nhanvien nv
        // LEFT JOIN phongban pb ON nv.MaPB = pb.MaPB
        // WHERE nv.MaNV NOT IN (SELECT MaNV FROM taikhoan WHERE MaNV IS NOT NULL)
        // AND nv.TrangThai = 'Còn làm việc'"; // Chỉ lấy người đang làm việc

        //     return Database.ExecuteQuery(query);
        // }
        public DataTable GetEmployeesWithoutAccount()
        {
            // Lấy những nhân viên chưa tồn tại trong bảng Taikhoan
            string query = @"
        SELECT nv.MaNV, nv.HoTen, nv.DienThoai, pb.TenPB 
        FROM nhanvien nv
        LEFT JOIN phongban pb ON nv.MaPB = pb.MaPB
        WHERE nv.TrangThai = 'Còn làm việc'"; // Chỉ lấy người đang làm việc

            return Database.ExecuteQuery(query);
        }
        public DataTable GetEmployeesForAccountCreation()
        {
            // Lấy MaNV, HoTen và mã nhân viên để hiển thị dễ nhìn
            // Chỉ lấy nhân viên đang làm việc và chưa có trong bảng Taikhoan
            string query = @"
                SELECT MaNV, HoTen, DienThoai 
                FROM nhanvien 
                WHERE TrangThai = 'Còn làm việc' ";
            
            return Database.ExecuteQuery(query);
        }

        public DataTable GetEmployeeDetails(int maNV)
        {
            string query = @"
                SELECT nv.DienThoai, pb.TenPB 
                FROM nhanvien nv
                LEFT JOIN phongban pb ON nv.MaPB = pb.MaPB
                WHERE nv.MaNV = @MaNV";
            
            var parameters = new Dictionary<string, object> { { "@MaNV", maNV } };
            return Database.ExecuteQuery(query, parameters);
        }


        public bool AddAccount(AccountManagementModel acc)
        {
            // Insert bảng TaiKhoan
            string queryAccount = @"INSERT INTO taikhoan (MaNV, TenDangNhap, MatKhau, TrangThai) 
                                    VALUES (@MaNV, @User, @Pass, 'Hoạt động')";

            var paramsAccount = new Dictionary<string, object>
            {
                { "@MaNV", acc.MaNV },
                { "@User", acc.TenDangNhap },
                { "@Pass", acc.MatKhau } // Mật khẩu đã được hash trước khi truyền vào đây
            };

            if (Database.ExecuteNonQuery(queryAccount, paramsAccount) > 0)
            {
                // Lấy MaTK vừa tạo
                string queryGetID = "SELECT MAX(MaTK) FROM taikhoan";
                DataTable dt = Database.ExecuteQuery(queryGetID);
                int newMaTK = Convert.ToInt32(dt.Rows[0][0]);

                // Lấy MaVaiTro từ tên vai trò
                string queryRoleID = "SELECT MaVaiTro FROM vaitro WHERE TenVaiTro = @TenVaiTro";
                var paramRole = new Dictionary<string, object> { { "@TenVaiTro", acc.VaiTro } };
                DataTable dtRole = Database.ExecuteQuery(queryRoleID, paramRole);
                
                int maVaiTro = 3; // Mặc định nhân viên
                if (dtRole.Rows.Count > 0)
                    maVaiTro = Convert.ToInt32(dtRole.Rows[0]["MaVaiTro"]);

                // Insert bảng phân quyền
                string queryInsertRole = "INSERT INTO taikhoan_vaitro (MaTK, MaVaiTro) VALUES (@MaTK, @MaVaiTro)";
                var paramsInsertRole = new Dictionary<string, object> 
                { 
                    { "@MaTK", newMaTK }, 
                    { "@MaVaiTro", maVaiTro } 
                };
                Database.ExecuteNonQuery(queryInsertRole, paramsInsertRole);

                return true;
            }
            return false;
        }

        // Kiểm tra username tồn tại
        public bool CheckUsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM taikhoan WHERE TenDangNhap = @User";
            var param = new Dictionary<string, object> { { "@User", username } };
            DataTable dt = Database.ExecuteQuery(query, param);
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }
    }
}