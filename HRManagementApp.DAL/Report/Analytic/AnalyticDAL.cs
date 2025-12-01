using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.DAL
{
    public class AnalyticsDAL {
        // Trong file DAL
        public AnalyticsModel GetAnalyticsSummary()
        {
            AnalyticsModel data = new AnalyticsModel();
            string today = DateTime.Now.ToString("yyyy-MM-dd");
        
            // 1. Tổng nhân viên & Đang làm việc
            string queryEmp = "SELECT COUNT(*) as Total, SUM(CASE WHEN TrangThai = 'Còn làm việc' THEN 1 ELSE 0 END) as Active FROM nhanvien";
            DataTable dtEmp = Database.ExecuteQuery(queryEmp);
            if (dtEmp.Rows.Count > 0)
            {
                data.TotalEmployees = Convert.ToInt32(dtEmp.Rows[0]["Total"]);
                data.ActiveEmployees = Convert.ToInt32(dtEmp.Rows[0]["Active"]);
            }
        
            // 2. Đi muộn & Có mặt hôm nay (để tính vắng)
            // Giả sử ca bắt đầu 08:00, cho phép trễ 15p -> 08:15
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
                presentToday = Convert.ToInt32(dtAtt.Rows[0]["PresentCount"]);
                data.LateToday = Convert.ToInt32(dtAtt.Rows[0]["LateCount"]);
            }
        
            // 3. Vắng mặt = (Nhân viên đang làm việc) - (Nhân viên đã chấm công hôm nay)
            data.AbsentToday = data.ActiveEmployees - presentToday;
            if (data.AbsentToday < 0) data.AbsentToday = 0;
        
            // 4. Đơn chờ duyệt
            string queryReq = "SELECT COUNT(*) FROM dontu WHERE TrangThai = 'Chưa duyệt'";
            DataTable dtReq = Database.ExecuteQuery(queryReq);
            if (dtReq.Rows.Count > 0)
            {
                data.PendingRequests = Convert.ToInt32(dtReq.Rows[0][0]);
            }
        
            return data;
        }
    }
};