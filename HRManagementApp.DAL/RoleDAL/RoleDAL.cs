using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL
{
    public class RoleDAL
    {
        // 1. Lấy danh sách vai trò
        public List<RoleModel> GetRoles()
        {
            List<RoleModel> list = new List<RoleModel>();
            DataTable dt = Database.ExecuteQuery("SELECT * FROM vaitro");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RoleModel
                {
                    MaVaiTro = Convert.ToInt32(row["MaVaiTro"]),
                    TenVaiTro = row["TenVaiTro"].ToString(),
                    MoTa = row["MoTa"].ToString()
                });
            }
            return list;
        }

        // 2. Thêm vai trò mới
        public bool AddRole(RoleModel role)
        {
            string query = "INSERT INTO vaitro (TenVaiTro, MoTa) VALUES (@Ten, @MoTa)";
            var param = new Dictionary<string, object>
            {
                { "@Ten", role.TenVaiTro },
                { "@MoTa", role.MoTa }
            };
            return Database.ExecuteNonQuery(query, param) > 0;
        }

        // 3. Lấy tất cả quyền hạn có trong hệ thống
        public List<PermissionModel> GetAllPermissions()
        {
            List<PermissionModel> list = new List<PermissionModel>();
            DataTable dt = Database.ExecuteQuery("SELECT * FROM quyenhan");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new PermissionModel
                {
                    MaQuyen = Convert.ToInt32(row["MaQuyen"]),
                    TenQuyen = row["TenQuyen"].ToString(),
                    MoTa = row["MoTa"].ToString()
                });
            }
            return list;
        }

        // 4. Lấy danh sách ID quyền của một vai trò cụ thể
        public List<int> GetPermissionIDsByRole(int maVaiTro)
        {
            List<int> list = new List<int>();
            string query = "SELECT MaQuyenHan FROM vaitro_quyenhan WHERE MaVaiTro = @MaVaiTro";
            var param = new Dictionary<string, object> { { "@MaVaiTro", maVaiTro } };

            DataTable dt = Database.ExecuteQuery(query, param);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(Convert.ToInt32(row["MaQuyenHan"]));
            }
            return list;
        }

        // 5. Cập nhật quyền (Transaction: Xóa cũ -> Thêm mới)
        public bool UpdateRolePermissions(int maVaiTro, List<int> newPermissionIds)
        {
            try
            {
                // Bước 1: Xóa hết quyền cũ
                string deleteQuery = "DELETE FROM vaitro_quyenhan WHERE MaVaiTro = @MaVaiTro";
                Database.ExecuteNonQuery(deleteQuery, new Dictionary<string, object> { { "@MaVaiTro", maVaiTro } });

                // Bước 2: Thêm quyền mới
                foreach (int permId in newPermissionIds)
                {
                    string insertQuery = "INSERT INTO vaitro_quyenhan (MaVaiTro, MaQuyenHan) VALUES (@Role, @Perm)";
                    var param = new Dictionary<string, object>
                    {
                        { "@Role", maVaiTro },
                        { "@Perm", permId }
                    };
                    Database.ExecuteNonQuery(insertQuery, param);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        // Trong RoleDAL.cs

        // 6. Cập nhật thông tin vai trò
        public bool UpdateRole(RoleModel role)
        {
            string query = "UPDATE vaitro SET TenVaiTro = @Ten, MoTa = @MoTa WHERE MaVaiTro = @Ma";
            var param = new Dictionary<string, object>
    {
        { "@Ten", role.TenVaiTro },
        { "@MoTa", role.MoTa },
        { "@Ma", role.MaVaiTro }
    };
            return Database.ExecuteNonQuery(query, param) > 0;
        }

        // 7. Kiểm tra vai trò có đang được sử dụng không (An toàn dữ liệu)
        // 7. Kiểm tra vai trò có đang được sử dụng không (An toàn dữ liệu)
        public bool CheckRoleInUse(int maVaiTro)
        {
            // Đếm xem có bao nhiêu dòng trong bảng taikhoan_vaitro chứa MaVaiTro này
            string query = "SELECT COUNT(*) FROM taikhoan_vaitro WHERE MaVaiTro = @Ma";
            var param = new Dictionary<string, object> { { "@Ma", maVaiTro } };

            // SỬA LỖI: Dùng ExecuteQuery thay vì ExecuteScalar
            DataTable dt = Database.ExecuteQuery(query, param);

            if (dt.Rows.Count > 0)
            {
                // Lấy giá trị ở dòng 0, cột 0
                int count = Convert.ToInt32(dt.Rows[0][0]);
                return count > 0;
            }

            return false;
        }

        // 8. Xóa vai trò
        public bool DeleteRole(int maVaiTro)
        {
            string query = "DELETE FROM vaitro WHERE MaVaiTro = @Ma";
            var param = new Dictionary<string, object> { { "@Ma", maVaiTro } };
            return Database.ExecuteNonQuery(query, param) > 0;
        }
    }
}