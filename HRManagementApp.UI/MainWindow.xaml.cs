using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HRManagementApp.BLL;
using HRManagementApp.models;
using HRManagementApp.UI.Views;
using HRManagementApp.UI.Views.Leave;
using HRManagementApp.Constants; // <--- 1. THÊM NAMESPACE NÀY

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
            SetActiveButton(DashboardBtn); // Set Dashboard as default active
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
            
            if (isAdmin || UserSession.HasPermission(AppPermissions.PERM_QL_NHAN_VIEN))
            { EmployeesBtn.Visibility = Visibility.Visible; }
            if (isAdmin || UserSession.HasPermission(AppPermissions.PERM_XEM_BAO_CAO)) 
            { ReportsBtn.Visibility = Visibility.Visible; }
            if (isAdmin || UserSession.HasPermission(AppPermissions.PERM_QL_CHAM_CONG)) 
            { AttendanceBtn.Visibility = Visibility.Visible; }
            if (isAdmin || UserSession.HasPermission(AppPermissions.PERM_DUYET_DON)) 
            { LeaveBtn.Visibility = Visibility.Visible; }
            if (isAdmin || UserSession.HasPermission(AppPermissions.PERM_QT_HE_THONG))
            {
                RoleBtn.Visibility = Visibility.Visible;
                AccountBtn.Visibility = Visibility.Visible;
            }
            if (isAdmin || UserSession.HasPermission(AppPermissions.PERM_QL_LUONG))
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
            // Navigation event handlers
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
            ChamCongBtn.Click += (s, e) => NavigateTo("ChamCong", s as Button);
            InforBtn.Click += (s, e) => NavigateTo("Info", s as Button);
            
            MyPayslip.Click += (s, e) => NavigateTo("mypayslip", s as Button);
        }

        private void NavigateTo(string section, Button clickedButton)
        {
            SetActiveButton(clickedButton);

            // Update the header text and description
            var headerTextBlock = FindName("HeaderText") as TextBlock;
            var descTextBlock = FindName("DescriptionText") as TextBlock;

            // Since we don't have named elements in the current XAML, we'll show a message box for now
            // In a real application, you would have a ContentFrame or similar to load different user controls
            // MessageBox.Show($"Navigating to {section} section.\n\nIn a complete application, this would load the {section} module with its specific functionality.",
            //               "Navigation", MessageBoxButton.OK, MessageBoxImage.Information);

            // Here you would typically load different UserControls or Pages based on the section
            LoadSectionContent(section);
        }

        private void SetActiveButton(Button activeButton)
        {
            // Reset previous active button
            if (currentActiveButton != null)
            {
                currentActiveButton.Background = Brushes.Transparent;
            }

            // Set new active button
            if (activeButton != null)
            {
                activeButton.Background = new SolidColorBrush(Color.FromRgb(26, 188, 156)); // #1ABC9C
                currentActiveButton = activeButton;
            }
        }

        private void LoadSectionContent(string section)
        {
            // This method would contain the logic to load different sections
            // For now, we'll just update some sample data based on the section

            switch (section.ToLower())
            {
                case "dashboard": ContentArea.Content = new Views.DashboardView(); break;
                case "employees": ContentArea.Content = new Views.EmployeesManagementView(); break;
                case "attendance": ContentArea.Content = new Views.AttendanceTagView(); break;
                case "payrolltag": ContentArea.Content = new Views.PayrollTag(); break;
                case "leave management": ContentArea.Content = new LeaveManagementView(); break;
                case "reports": ContentArea.Content = new ReportView(); break;
                case "settings": ContentArea.Content = new ForEmployeeManagementView(); break;
                case "editpayroll": ContentArea.Content = new Views.EditPayroll(); break;
                case "account": ContentArea.Content = new Views.AccountManagementView(); break;
                case "roles": ContentArea.Content = new Views.RoleManagementView(); break;
                case "chamcong": ContentArea.Content = new Views.ScanAttendanceView(); break;
                case "info": ContentArea.Content = new Views.AccountInfoView(); break;
                
                case "mypayslip": ContentArea.Content = new Views.MyPayslipView(); break;
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

        // Sample data models that would be used in a real application