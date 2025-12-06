using HRManagementApp.DAL;
using HRManagementApp.models;
using System.Collections.Generic;
using System.Data;

namespace HRManagementApp.BLL
{
    public class MyPayslipBLL
    {
        private MyPayslipDAL _dal = new MyPayslipDAL();

        public List<string> GetMonthList(int maNV)
        {
            DataTable dt = _dal.GetAvailableMonths(maNV);
            List<string> list = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add($"Tháng {row["Thang"]}/{row["Nam"]}");
            }
            return list;
        }

        public MyPayslipDTO GetPayslip(int maNV, string selectedMonthStr)
        {
            // Parse chuỗi "Tháng 10/2024" -> month=10, year=2024
            string[] parts = selectedMonthStr.Replace("Tháng ", "").Split('/');
            if (parts.Length == 2)
            {
                int month = int.Parse(parts[0]);
                int year = int.Parse(parts[1]);
                return _dal.GetPayslipDetail(maNV, month, year);
            }
            return null;
        }
    }
}