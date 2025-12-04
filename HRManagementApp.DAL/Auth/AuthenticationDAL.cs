using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL
{
    public class AuthenticationDAL
    {
        public bool Login(string username, string password)
        {
            // 1. Kiểm tra tài khoản & mật khẩu
            string query = @"
                SELECT 
                    tk.MaTK, tk.MaNV, tk.TenDangNhap, tk.VaiTro, 
                    nv.HoTen, nv.DienThoai, nv.SoCCCD, nv.NgaySinh, nv.GioiTinh, nv.MaPB,
                    pb.TenPB
                FROM taikhoan tk
                LEFT JOIN nhanvien nv ON tk.MaNV = nv.MaNV
                LEFT JOIN phongban pb ON nv.MaPB = pb.MaPB
                WHERE tk.TenDangNhap = @User 
                  AND tk.MatKhau = @Pass 
                  AND tk.TrangThai = 'Hoạt động'";

            var parameters = new Dictionary<string, object>
            {
                { "@User", username },
                { "@Pass", password } 
            };

            DataTable dt = Database.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                
                // Lưu thông tin Tài khoản
                UserSession.MaTK = Convert.ToInt32(row["MaTK"]);
                UserSession.TenDangNhap = row["TenDangNhap"].ToString();
                UserSession.VaiTro = row["VaiTro"].ToString(); // Lưu ý: Đây là tên vai trò lưu cứng ở bảng taikhoan (nếu có)
                
                // Lưu thông tin Nhân viên
                if (row["MaNV"] != DBNull.Value)
                {
                    UserSession.MaNV = Convert.ToInt32(row["MaNV"]);
                    UserSession.HoTen = row["HoTen"].ToString();
                    UserSession.DienThoai = row["DienThoai"] != DBNull.Value ? row["DienThoai"].ToString() : null;
                    UserSession.SoCCCD = row["SoCCCD"] != DBNull.Value ? row["SoCCCD"].ToString() : null;
                    UserSession.GioiTinh = row["GioiTinh"] != DBNull.Value ? row["GioiTinh"].ToString() : null;
                    if (row["NgaySinh"] != DBNull.Value) UserSession.NgaySinh = Convert.ToDateTime(row["NgaySinh"]);
                    if (row["MaPB"] != DBNull.Value)
                    {
                        UserSession.MaPB = Convert.ToInt32(row["MaPB"]);
                        UserSession.TenPB = row["TenPB"] != DBNull.Value ? row["TenPB"].ToString() : null;
                    }
                }
                else
                {
                    UserSession.HoTen = "Administrator";
                }

                // ---------------------------------------------------------
                // 2. LẤY DANH SÁCH QUYỀN HẠN (Mới thêm)
                // Nguyên tắc: TaiKhoan -> TaiKhoan_VaiTro -> VaiTro -> VaiTro_QuyenHan -> QuyenHan
                // ---------------------------------------------------------
                
                // Xóa quyền cũ (nếu có)
                UserSession.QuyenHan.Clear();

                string queryPerm = @"
                    SELECT DISTINCT qh.TenQuyen 
                    FROM taikhoan_vaitro tkvt
                    JOIN vaitro_quyenhan vtqh ON tkvt.MaVaiTro = vtqh.MaVaiTro
                    JOIN quyenhan qh ON vtqh.MaQuyenHan = qh.MaQuyen
                    WHERE tkvt.MaTK = @MaTK";

                var paramPerm = new Dictionary<string, object> { { "@MaTK", UserSession.MaTK } };
                DataTable dtPerm = Database.ExecuteQuery(queryPerm, paramPerm);

                foreach (DataRow dr in dtPerm.Rows)
                {
                    string tenQuyen = dr["TenQuyen"].ToString();
                    if (!string.IsNullOrEmpty(tenQuyen))
                    {
                        UserSession.QuyenHan.Add(tenQuyen);
                    }
                }

                return true;
            }

            return false;
        }
    }
}