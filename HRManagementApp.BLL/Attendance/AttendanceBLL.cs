using HRManagementApp.DAL;
using HRManagementApp.models;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
namespace HRManagementApp.BLL
{
    public class AttendanceBLL
    {
        
        private AttendanceDAL dal = new AttendanceDAL();

        public List<AttendanceTodayModel> GetTodayAttendanceList()
        {
            return dal.GetTodayAttendance();
        }

        public (int Total, int Present, int Absent, int Late) GetTodayStatistics(List<AttendanceTodayModel> list)
        {
            int total = list.Count;
            int present = list.Count(x => x.Status == "Đúng giờ" || x.Status == "Đi muộn" || x.Status == "Đang làm");
            int late = list.Count(x => x.Status == "Đi muộn");
            int absent = list.Count(x => x.Status == "Vắng");

            return (total, present, absent, late);
        }

        public List<ApprovalModel> GetApprovals()
        {
            return dal.GetPendingApprovals();
        }

        public ReportStatModel GetReport(DateTime date)
        {
            return dal.GetMonthlyReport(date.Month, date.Year);
        }
        public bool UpdateApprovalStatus(int maDon, string status, string approverName)
        {
            return dal.UpdateApprovalStatus(maDon, status, approverName);
        }
        // Trong class AttendanceBLL

        public List<AttendanceTodayModel> GetDailyAttendanceList(DateTime date)
        {
            return dal.GetAttendanceByDate(date);
        }
        public void ExportAttendanceToExcel(int month, int year, string filePath)
        {
            // 1. Lấy dữ liệu thô
            var rawData = dal.GetExportData(month, year);

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // 3. Nhóm dữ liệu theo Phòng Ban
                var departments = rawData.GroupBy(x => x.TenPB).ToList();
                int daysInMonth = DateTime.DaysInMonth(year, month);

                foreach (var deptGroup in departments)
                {
                    string deptName = deptGroup.Key;
                    // Tạo Sheet tên phòng ban (Lưu ý tên sheet excel tối đa 31 ký tự)
                    string sheetName = deptName.Length > 30 ? deptName.Substring(0, 30) : deptName;

                    // Kiểm tra trùng tên sheet
                    if (package.Workbook.Worksheets.Any(x => x.Name == sheetName))
                        sheetName += " 2";

                    var ws = package.Workbook.Worksheets.Add(sheetName);

                    // --- HEADER ---
                    // Dòng 1: Tiêu đề lớn
                    ws.Cells["A1"].Value = $"BẢNG CHẤM CÔNG THÁNG {month}/{year} - {deptName.ToUpper()}";
                    ws.Cells[1, 1, 1, 2 + (daysInMonth * 3)].Merge = true; // Merge hết chiều ngang
                    ws.Cells["A1"].Style.Font.Size = 16;
                    ws.Cells["A1"].Style.Font.Bold = true;
                    ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Dòng 2 & 3: Cấu trúc cột
                    // Cột A: Tên Nhân Viên (Merge dòng 2-3)
                    ws.Cells[2, 1].Value = "Tên Nhân Viên";
                    ws.Cells[2, 1, 3, 1].Merge = true;
                    ws.Cells[2, 1, 3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[2, 1, 3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(1).Width = 25;

                    // Các cột ngày
                    int colIndex = 2;
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        // Dòng 2: Ngày (Merge 3 ô)
                        ws.Cells[2, colIndex].Value = $"Ngày {day}";
                        ws.Cells[2, colIndex, 2, colIndex + 2].Merge = true;
                        ws.Cells[2, colIndex, 2, colIndex + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        // Dòng 3: Chi tiết (Vào, Ra, Tổng)
                        ws.Cells[3, colIndex].Value = "Vào";
                        ws.Cells[3, colIndex + 1].Value = "Ra";
                        ws.Cells[3, colIndex + 2].Value = "Giờ";

                        // Format nhỏ lại cho gọn
                        ws.Column(colIndex).Width = 6;
                        ws.Column(colIndex + 1).Width = 6;
                        ws.Column(colIndex + 2).Width = 6;

                        colIndex += 3;
                    }

                    // Cột Tổng công tháng
                    ws.Cells[2, colIndex].Value = "Tổng giờ công";
                    ws.Cells[2, colIndex, 3, colIndex].Merge = true;
                    ws.Cells[2, colIndex, 3, colIndex].Style.WrapText = true;
                    ws.Column(colIndex).Width = 10;

                    // Style Header Background
                    using (var range = ws.Cells[2, 1, 3, colIndex])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // --- FILL DATA ---
                    // Lấy danh sách nhân viên trong phòng ban này
                    var employees = deptGroup.GroupBy(x => x.MaNV).ToList();
                    int rowIndex = 4;

                    foreach (var emp in employees)
                    {
                        string empName = emp.First().HoTen;
                        ws.Cells[rowIndex, 1].Value = empName;

                        double totalMonthHours = 0;
                        int currentDataCol = 2;

                        // Duyệt qua từng ngày trong tháng
                        for (int day = 1; day <= daysInMonth; day++)
                        {
                            // Tìm dữ liệu chấm công của ngày này
                            var log = emp.FirstOrDefault(x => x.Ngay.HasValue && x.Ngay.Value.Day == day);

                            if (log != null && log.GioVao.HasValue && log.GioRa.HasValue)
                            {
                                ws.Cells[rowIndex, currentDataCol].Value = log.GioVao.Value.ToString(@"hh\:mm");
                                ws.Cells[rowIndex, currentDataCol + 1].Value = log.GioRa.Value.ToString(@"hh\:mm");

                                double hours = (log.GioRa.Value - log.GioVao.Value).TotalHours;
                                if (hours < 0) hours = 0;

                                ws.Cells[rowIndex, currentDataCol + 2].Value = Math.Round(hours, 1);
                                totalMonthHours += hours;
                            }
                            else if (log != null && log.GioVao.HasValue)
                            {
                                ws.Cells[rowIndex, currentDataCol].Value = log.GioVao.Value.ToString(@"hh\:mm");
                            }

                            // Tô màu cột ngày Chủ Nhật để dễ nhìn
                            DateTime currentDate = new DateTime(year, month, day);
                            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                            {
                                using (var range = ws.Cells[rowIndex, currentDataCol, rowIndex, currentDataCol + 2])
                                {
                                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 240, 240)); // Màu hồng nhạt
                                }
                            }

                            currentDataCol += 3;
                        }

                        // Ghi tổng giờ công
                        ws.Cells[rowIndex, currentDataCol].Value = Math.Round(totalMonthHours, 1);
                        ws.Cells[rowIndex, currentDataCol].Style.Font.Bold = true;

                        rowIndex++;
                    }

                    // Kẻ khung bảng
                    using (var range = ws.Cells[2, 1, rowIndex - 1, colIndex])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                }

                package.Save();
            }
        }
    }
}