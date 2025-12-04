using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL
{
    public class EmployeeDAL {
        // Trong file DAL
        public EmployeeModel GetEmployeeSummary()
        {
            EmployeeModel data = new EmployeeModel();
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            // 1. Tổng nhân viên & Đang làm việc
            string queryEmp = "SELECT COUNT(*) as Total, SUM(CASE WHEN TrangThai = 'Còn làm việc' THEN 1 ELSE 0 END) as Active FROM nhanvien";
            DataTable dtEmp = Database.ExecuteQuery(queryEmp);
            
            if (dtEmp.Rows.Count > 0)
            {
                DataRow row = dtEmp.Rows[0];
                data.TotalEmployees = row["Total"] != DBNull.Value ? Convert.ToInt32(row["Total"]) : 0;
                data.ActiveEmployees = row["Active"] != DBNull.Value ? Convert.ToInt32(row["Active"]) : 0;
            }

            // 2. Đi muộn & Có mặt hôm nay
            string queryAtt = $@"
                SELECT 
                    COUNT(DISTINCT MaNV) as PresentCount,
                    SUM(CASE WHEN GioVao > '08:15:00' THEN 1 ELSE 0 END) as LateCount
                FROM chamcong 
                WHERE Ngay = '{today}'";

            DataTable dtAtt = Database.ExecuteQuery(queryAtt);
            int presentToday = 0;

            if (dtAtt.Rows.Count > 0)
            {
                DataRow row = dtAtt.Rows[0];
                presentToday = row["PresentCount"] != DBNull.Value ? Convert.ToInt32(row["PresentCount"]) : 0;
                data.LateToday = row["LateCount"] != DBNull.Value ? Convert.ToInt32(row["LateCount"]) : 0;
            }

            // 3. Vắng mặt
            data.AbsentToday = data.ActiveEmployees - presentToday;
            if (data.AbsentToday < 0) data.AbsentToday = 0;

            // 4. Đơn chờ duyệt
            string queryReq = "SELECT COUNT(*) FROM dontu WHERE TrangThai = 'Chưa duyệt'";
            DataTable dtReq = Database.ExecuteQuery(queryReq);
            if (dtReq.Rows.Count > 0 && dtReq.Rows[0][0] != DBNull.Value)
            {
                data.PendingRequests = Convert.ToInt32(dtReq.Rows[0][0]);
            }

            return data;
        }

        public Dictionary<string, int> GetEmployeeByDepartment()
        {
            var result = new Dictionary<string, int>();

            string query = @"
                SELECT pb.TenPB, COUNT(nv.MaNV) as SoLuong
                FROM phongban pb
                LEFT JOIN nhanvien nv ON pb.MaPB = nv.MaPB AND nv.TrangThai = 'Còn làm việc'
                GROUP BY pb.TenPB";

            DataTable dt = Database.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                string tenPB = row["TenPB"] != DBNull.Value ? row["TenPB"].ToString() : "Chưa phân bổ";
                int soLuong = row["SoLuong"] != DBNull.Value ? Convert.ToInt32(row["SoLuong"]) : 0;
                
                if (!result.ContainsKey(tenPB))
                {
                    result.Add(tenPB, soLuong);
                }
            }

            return result;
        }
    }
};