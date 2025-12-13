using System.Windows.Controls;
using System.Windows;
using HRManagementApp.BLL.Report;
using HRManagementApp.BLL; // Import namespace Service Log
using HRManagementApp.models; // Import namespace Models
using System.Collections.Generic;
using System.Linq; // Để dùng LINQ lọc
using System;

namespace HRManagementApp.UI.Views
{
    public partial class DashboardView : UserControl
    {
        // 1. Khai báo các Service
        private readonly DashboardStatisticService statisticService = new DashboardStatisticService();
        private readonly SystemLogService _logService = new SystemLogService();

        // Biến lưu danh sách gốc để lọc trên RAM (tránh query DB nhiều lần nếu không cần thiết)
        private List<SystemLog> _allLogs;

        // Quick Action Events
        public event Action? AddNewEmployeeRequested;

        public DashboardView()
        {
            InitializeComponent();

            // Khởi tạo dữ liệu filter Năm (Ví dụ: 2 năm trước đến năm hiện tại)
            InitYearFilter();

            LoadOverviewStatistics();
            LoadSystemLogs(); // Tải log khi mở Dashboard
        }

        private void InitYearFilter()
        {
            int currentYear = DateTime.Now.Year;
            cboFilterYear.Items.Add("All");
            cboFilterYear.Items.Add((currentYear).ToString());
            cboFilterYear.Items.Add((currentYear - 1).ToString());
            cboFilterYear.SelectedIndex = 1; // Mặc định chọn năm nay
        }

        // ========================================================
        // PHẦN 1: THỐNG KÊ (Giữ nguyên code cũ của bạn)
        // ========================================================
        public void LoadOverviewStatistics()
        {
            TxtToday.Text = "Today: " + DateTime.Now.ToString("MMMM dd, yyyy");

            int totalEmployees = statisticService.GetTotalEmployee();
            int joinedThisMonth = statisticService.GetEmployeesCountJoinedThisMonth();
            TxtTotalEmployee.Text = totalEmployees.ToString();
            TxtJoinedThisMonth.Text = joinedThisMonth + " joined this month";

            int presentToday = statisticService.GetPresentToday();
            double attendanceRate = statisticService.CalcAttendanceRate((double)presentToday, (double)totalEmployees);
            TxtPresentToday.Text = presentToday.ToString();
            TxtAttendanceRate.Text = attendanceRate + "% attendance";

            int onLeaveCount = statisticService.GetLeaveCount();
            int pendingApprovalsCount = statisticService.GetPendingApprovalsCount();
            TxtOnLeave.Text = onLeaveCount.ToString();
            TxtPendingApprovals.Text = pendingApprovalsCount +
                                       (pendingApprovalsCount == 1 ? " pending approval" : " pending approvals");

            double monthlyPayroll = statisticService.GetMonthlyPayroll();
            TxtMonthlyPayroll.Text = (monthlyPayroll / 1000).ToString("0.#") + "k";

            // ... (Phần logic DueDate giữ nguyên) ...
            bool isPaid = Math.Round(monthlyPayroll, 2) == 0.0 || statisticService.IsPaidMonthlyPayroll();
            if (isPaid)
            {
                TxtDueDate.Text = "Paid";
            }
            else
            {
                int DaysToDueDate = statisticService.GetDaysToDueDate();
                TxtDueDate.Text = DaysToDueDate < 0
                    ? "Overdue by " + (DaysToDueDate * -1) + (DaysToDueDate == -1 ? " day" : " days")
                    : DaysToDueDate == 0
                        ? "Due today"
                        : "Due in " + DaysToDueDate + " days";
            }
        }

        // ========================================================
        // PHẦN 2: LOGIC HIỂN THỊ SYSTEM LOG (Mới thêm)
        // ========================================================

        private void LoadSystemLogs()
        {
            try
            {
                // Lấy toàn bộ log (Hoặc 100 dòng mới nhất từ Service)
                _allLogs = _logService.GetAllLogs();

                // Gọi hàm lọc để hiển thị theo ComboBox hiện tại
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải lịch sử hệ thống: " + ex.Message);
            }
        }

        private void ApplyFilters()
        {
            if (_allLogs == null) return;

            var filteredList = _allLogs.AsEnumerable();

            // 1. Lọc theo THÁNG
            if (cboFilterMonth.SelectedIndex > 0)
            {
                // Lấy Content: "1", "2"... hoặc "Month: All"
                string content = (cboFilterMonth.SelectedItem as ComboBoxItem)?.Content.ToString();
                // Nếu content là số thì mới parse
                if (int.TryParse(content, out int month))
                {
                    filteredList = filteredList.Where(x => x.ThoiGian.Month == month);
                }
            }

            // 2. Lọc theo NĂM
            if (cboFilterYear.SelectedIndex > 0)
            {
                if (int.TryParse(cboFilterYear.SelectedItem?.ToString(), out int year))
                {
                    filteredList = filteredList.Where(x => x.ThoiGian.Year == year);
                }
            }

            // 3. Lọc theo HÀNH ĐỘNG
            if (cboFilterAction.SelectedIndex > 0)
            {
                string content = (cboFilterAction.SelectedItem as ComboBoxItem)?.Content.ToString();
                // content đang là "INSERT", "UPDATE"... 
                // Lưu ý: Trong XAML mình để Content="INSERT", nếu bạn để Content="Action: INSERT" thì phải cắt chuỗi.
                // Ở XAML trên mình để Content nguyên bản (INSERT, UPDATE...) nên lấy trực tiếp OK.

                if (!string.IsNullOrEmpty(content) && !content.Contains("All"))
                {
                    filteredList = filteredList.Where(x => x.HanhDong == content);
                }
            }

            // 4. Lọc theo BẢNG
            if (cboFilterTable.SelectedIndex > 0)
            {
                string content = (cboFilterTable.SelectedItem as ComboBoxItem)?.Content.ToString();
                // content đang là "Table: All", "NhanVien"...
                // Trong XAML mình viết Content="Table: All", "NhanVien"...
                // Cần sửa lại XAML một chút cho logic này dễ hơn, hoặc dùng Tag.

                // ==> ĐỂ ĐƠN GIẢN: Trong XAML bạn hãy xóa chữ "Table: " ở các item con đi, chỉ để tên bảng thôi.
                // Hoặc xử lý ở đây:
                string tableName = content.Replace("Table: ", "").Trim();
                if (!tableName.Contains("All"))
                {
                    filteredList = filteredList.Where(x => x.BangLienQuan == tableName);
                }
            }

            dgSystemLogs.ItemsSource = filteredList.ToList();
        }

        // Sự kiện khi thay đổi bất kỳ bộ lọc nào
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Nút làm mới dữ liệu
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadSystemLogs(); // Tải lại từ Database
        }

        public void GoToAddNewEmployee(object sender, RoutedEventArgs e)
        {
            AddNewEmployeeRequested?.Invoke();
        }
    }
}