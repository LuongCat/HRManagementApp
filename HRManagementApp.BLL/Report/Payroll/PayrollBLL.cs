using HRManagementApp.DAL.Report;
using HRManagementApp.models;
using System;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Linq;
using System.IO;
namespace HRManagementApp.BLL.Report
{
    public class PayrollBLL
    {
        private PayrollDAL dal = new PayrollDAL();

        public List<PayrollReportModel> GetPayrollReport(int month, int year)
        {
            // Có thể thêm logic kiểm tra hoặc tính toán lại nếu cần
            return dal.GetPayrollReport(month, year);
        }

        // Hàm hỗ trợ lấy danh sách tháng/năm để đổ vào ComboBox (nếu cần)
        public List<int> GetYearsList()
        {
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear; i >= currentYear - 5; i--)
            {
                years.Add(i);
            }
            return years;
        }

        public void ExportPayrollToExcel(int month, int year, string filePath)
        {
            var data = dal.GetPayrollReport(month, year);

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Tạo Sheet mới
                var ws = package.Workbook.Worksheets.Add($"Luong_T{month}_{year}");

                // Header
                ws.Cells["A1"].Value = $"BẢNG THANH TOÁN LƯƠNG THÁNG {month}/{year}";
                ws.Cells["A1:H1"].Merge = true;
                ws.Cells["A1"].Style.Font.Size = 16;
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Column Headers
                string[] headers = { "Mã NV", "Họ Tên", "Phòng Ban", "Ngày Công", "Lương CB", "Phụ Cấp", "Khấu Trừ", "Thực Lãnh" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cells[3, i + 1].Value = headers[i];
                    ws.Cells[3, i + 1].Style.Font.Bold = true;
                    ws.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    ws.Cells[3, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Fill Data
                int row = 4;
                foreach (var item in data)
                {
                    ws.Cells[row, 1].Value = item.MaNV;
                    ws.Cells[row, 2].Value = item.EmployeeName;
                    ws.Cells[row, 3].Value = item.Department;
                    ws.Cells[row, 4].Value = item.WorkDays;
                    ws.Cells[row, 5].Value = item.BaseSalary;
                    ws.Cells[row, 6].Value = item.TotalAllowance;
                    ws.Cells[row, 7].Value = item.TotalDeduction;
                    ws.Cells[row, 8].Value = item.NetPay;

                    // Định dạng số tiền
                    ws.Cells[row, 5, row, 8].Style.Numberformat.Format = "#,##0";

                    // Kẻ khung
                    ws.Cells[row, 1, row, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[row, 1, row, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells[row, 1, row, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[row, 1, row, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    row++;
                }

                // AutoFit cột cho đẹp
                ws.Cells.AutoFitColumns();

                package.Save();
            }
        }
        // Trong PayrollBLL.cs

        public Dictionary<string, decimal> GetPayrollStatistics(string type, int month, int year)
        {
            switch (type)
            {
                case "Theo Tuần":
                    return dal.GetWeeklyStats(month, year);
                case "Theo Tháng":
                    return dal.GetMonthlyStats(year);
                case "Theo Năm":
                    return dal.GetYearlyStats();
                default:
                    return new Dictionary<string, decimal>();
            }
        }

        public double GetMonthlyPayroll()
        {
            return dal.CalcMonthlyPayroll();
        }

        public int GetMonthlyPayrollEmployees()
        {
            return dal.CountPayrollEmployees();
        }

        public double GetPrevMonthlyPayroll()
        {
            return dal.CalcPrevMonthlyPayroll();
        }

        public double GetMonthlyPaidPayroll()
        {
            return dal.CalcMonthlyPaidPayroll();
        }

        public double[] GetDepartmentPayroll()
        {
            return dal.GetDepartmentPayroll();
        }

        public double[] GetSalaryTrend()
        {
            return dal.GetSalaryTrend();
        }
    }
}