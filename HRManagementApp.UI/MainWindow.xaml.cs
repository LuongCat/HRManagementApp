using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HRManagementApp.BLL;
using HRManagementApp.models;
using HRManagementApp.UI.Views;
using HRManagementApp.UI.Views.Leave;
using HRManagementApp.UI.Views.Report; // Thêm namespace Report

namespace HRManagementApp.UI
{
    public partial class MainWindow : Window
    {
        private Button currentActiveButton = null!;
        private AuthenticationBLL _authBLL = new AuthenticationBLL();

        public MainWindow()
        {
            InitializeComponent();
            LoadUserInfo(); 
            
            ApplyPermissions();
            
            InitializeEventHandlers();
            SetActiveButton(DashboardBtn);
            LoadSectionContent("dashboard");
        }
        
        private void ApplyPermissions()
        {
            // Mặc định: Ẩn các chức năng nhạy cảm
            if (EmployeesBtn != null) EmployeesBtn.Visibility = Visibility.Collapsed;
            if (RoleBtn != null) RoleBtn.Visibility = Visibility.Collapsed;
            if (AccountBtn != null) AccountBtn.Visibility = Visibility.Collapsed;
            if (ReportsBtn != null) ReportsBtn.Visibility = Visibility.Collapsed;
            if (EditPayrollBtn != null) EditPayrollBtn.Visibility = Visibility.Collapsed;
            if (LeaveBtn != null) LeaveBtn.Visibility = Visibility.Collapsed;
            if (PayrollBtn != null) PayrollBtn.Visibility = Visibility.Collapsed;
            if (AttendanceBtn != null) AttendanceBtn.Visibility = Visibility.Collapsed;

            // Logic: Nếu là Admin (UserSession.VaiTro == "Admin") thì hiện tất cả (Optional)
            // Hoặc kiểm tra chi tiết từng quyền trong UserSession.QuyenHan
            bool isAdmin = UserSession.VaiTro == "Admin";
            
            if (isAdmin || UserSession.HasPermission("QuanLyNhanVien")){ EmployeesBtn.Visibility = Visibility.Visible; }
            if (isAdmin || UserSession.HasPermission("XemBaoCao")) { ReportsBtn.Visibility = Visibility.Visible; }
            if (isAdmin || UserSession.HasPermission("Attandance")) { AttendanceBtn.Visibility = Visibility.Visible; }
            if (isAdmin || UserSession.HasPermission("QuanLiDonTu")) { LeaveBtn.Visibility = Visibility.Visible; }
            if (isAdmin || UserSession.HasPermission("QuanTriHeThong"))
            {
                RoleBtn.Visibility = Visibility.Visible;
                AccountBtn.Visibility = Visibility.Visible;
            }
            if (isAdmin || UserSession.HasPermission("EditPayroll")) 
            {   
                PayrollBtn.Visibility = Visibility.Visible;
                EditPayrollBtn.Visibility = Visibility.Visible; 
            }
        }
        private void LoadUserInfo()
        {
            if (txtUserName != null)
            {
                txtUserName.Text = UserSession.HoTen;
            }
            if (txtUserRole != null)
            {
                txtUserRole.Text = UserSession.VaiTro;
            }
        }

        private void InitializeEventHandlers()
        {
            DashboardBtn.Click += (s, e) => NavigateTo("Dashboard", s as Button);
            EmployeesBtn.Click += (s, e) => NavigateTo("Employees", s as Button);
            AttendanceBtn.Click += (s, e) => NavigateTo("Attendance", s as Button);
            PayrollBtn.Click += (s, e) => NavigateTo("PayrollTag", s as Button);
            LeaveBtn.Click += (s, e) => NavigateTo("Leave Management", s as Button);
            ReportsBtn.Click += (s, e) => NavigateTo("Reports", s as Button);
            SettingsBtn.Click += (s, e) => NavigateTo("Settings", s as Button);
            RoleBtn.Click += (s, e) => NavigateTo("Roles", s as Button);
            EditPayrollBtn.Click += (s, e) => NavigateTo("EditPayroll", s as Button);
            LogoutBtn.Click += LogoutBtn_Click; 
            AccountBtn.Click += (s, e) => NavigateTo("Account", s as Button);
            RoleBtn.Click += (s, e) => NavigateTo("Role", s as Button);
        }

        private void NavigateTo(string section, Button clickedButton)
        {
            SetActiveButton(clickedButton);
            LoadSectionContent(section);
        }

        private void SetActiveButton(Button activeButton)
        {
            if (currentActiveButton != null)
            {
                currentActiveButton.Background = Brushes.Transparent;
            }

            if (activeButton != null)
            {
                activeButton.Background = new SolidColorBrush(Color.FromRgb(26, 188, 156));
                currentActiveButton = activeButton;
            }
        }

        private void LoadSectionContent(string section)
        {
            switch (section.ToLower())
            {
                case "dashboard":
                    LoadDoashboardSection();
                    break;
                case "employees":
                    LoadEmployeeSection();
                    break;
                case "attendance":
                    LoadAttendanceSection();
                    break;
                case "payrolltag":
                    LoadPayrollSection();
                    break;
                case "leave management":
                    LoadLeaveSection();
                    break;
                case "reports":
                    LoadReportsSection();
                    break;
                case "settings":
                    LoadSettingsSection();
                    break;
                case "editpayroll":
                    LoadEditPayrollSection();
                    break;
                case "account":
                    LoadAccountsSection();
                    break;
                case "role":
                    LoadRoleSection();
                    break;
            }
        }

        private void LoadEditPayrollSection()
        {
            ContentArea.Content = new Views.EditPayroll();
        }
        
        
        private void LoadDoashboardSection()
        {
            ContentArea.Content = new Views.DashboardView();
        }
        private void LoadEmployeeSection()
        {
            ContentArea.Content = new Views.EmployeesManagementView();
        }

        private void LoadAttendanceSection()
        {
            ContentArea.Content = new Views.AttendanceTagView();
        }

        private void LoadPayrollSection()
        {
            ContentArea.Content = new Views.PayrollTag();
            // Salary calculations, pay slips, tax deductions, etc.
        }

        private void LoadLeaveSection()
        {
            // Khởi tạo và hiển thị View quản lý đơn từ
            ContentArea.Content = new LeaveManagementView(); 
        }

        private void LoadReportsSection()
        {
            ContentArea.Content = new ReportView(); 
        }

        private void LoadSettingsSection()
        {
             ContentArea.Content = new ForEmployeeManagementView();
        }
        private void LoadAccountsSection()
        {
            ContentArea.Content = new Views.AccountManagementView();

        }

        private void LoadRoleSection()
        {
            ContentArea.Content = new Views.RoleManagementView();
        }
        
        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                                       "Xác nhận",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 1. Xóa session
                _authBLL.Logout();

                // 2. Mở lại màn hình Login
                LoginWindow login = new LoginWindow();
                login.Show();

                // 3. Đóng màn hình chính
                this.Close();
            }
        }
    }
}