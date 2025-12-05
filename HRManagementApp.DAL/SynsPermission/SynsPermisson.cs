using HRManagementApp.Constants;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL
{
    public class SystemDAL
    {
        public void SyncPermissions()
        {
            // 1. Lấy tất cả quyền đang có trong DB
            List<string> dbPermissions = new List<string>();
            string queryGet = "SELECT TenQuyen FROM quyenhan";
            DataTable dt = Database.ExecuteQuery(queryGet);
            
            foreach (DataRow row in dt.Rows)
            {
                dbPermissions.Add(row["TenQuyen"].ToString());
            }

            // 2. Duyệt qua danh sách trong Code
            foreach (var perm in AppPermissions.ListAll)
            {
                // Nếu quyền trong Code chưa có trong DB -> Thêm mới
                if (!dbPermissions.Contains(perm.Key))
                {
                    string insertQuery = "INSERT INTO quyenhan (TenQuyen, MoTa) VALUES (@Ten, @MoTa)";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@Ten", perm.Key },
                        { "@MoTa", perm.Value }
                    };
                    Database.ExecuteNonQuery(insertQuery, parameters);
                }
                else 
                {
                    // (Tùy chọn) Cập nhật lại Mô tả nếu bạn sửa text trong code
                    string updateQuery = "UPDATE quyenhan SET MoTa = @MoTa WHERE TenQuyen = @Ten";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@Ten", perm.Key },
                        { "@MoTa", perm.Value }
                    };
                    Database.ExecuteNonQuery(updateQuery, parameters);
                }
            }
        }
    }
}