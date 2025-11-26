using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HRManagementApp.UI
{
    public partial class MainWindow : Window
    {
        private Button currentActiveButton = null!;

        public MainWindow()
        {
            InitializeComponent();
            InitializeEventHandlers();
            SetActiveButton(DashboardBtn); // Set Dashboard as default active
            LoadSectionContent("dashboard");
        }

        private void InitializeEventHandlers()
        {
            // Navigation event handlers
            DashboardBtn.Click += (s, e) => NavigateTo("Dashboard", s as Button);
            EmployeesBtn.Click += (s, e) => NavigateTo("Employees", s as Button);
            AttendanceBtn.Click += (s, e) => NavigateTo("Attendance", s as Button);
            PayrollBtn.Click += (s, e) => NavigateTo("Payroll", s as Button);
            LeaveBtn.Click += (s, e) => NavigateTo("Leave Management", s as Button);
            ReportsBtn.Click += (s, e) => NavigateTo("Reports", s as Button);
            SettingsBtn.Click += (s, e) => NavigateTo("Settings", s as Button);
            LogoutBtn.Click += LogoutBtn_Click; 
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
                case "dashboard":
                    LoadDoashboardSection();
                    break;
                case "employees":
                    LoadEmployeeSection();
                    break;
                case "attendance":
                    LoadAttendanceSection();
                    break;
                case "payroll":
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
            }
        }
        private void LoadDoashboardSection()
        {
            var view = new Views.DashboardView();
            ContentArea.Content = view;

            view.AddNewEmployeeRequested += () =>
            {
                SetActiveButton(EmployeesBtn);
                var employeeView = new Views.EmployeesView();
                ContentArea.Content = employeeView;
                employeeView.BtnThemNV.RaiseEvent(
                    new RoutedEventArgs(Button.ClickEvent)
                );
            };
        }
        private void LoadEmployeeSection()
        {
            ContentArea.Content = new Views.EmployeesManagementView();
        }

        private void LoadAttendanceSection()
        {
            // Attendance management logic
            // Time tracking, attendance reports, etc.
        }

        private void LoadPayrollSection()
        {
            ContentArea.Content = new Views.PayrollView();
            // Salary calculations, pay slips, tax deductions, etc.
        }

        private void LoadLeaveSection()
        {
            // Leave management logic
            // Leave applications, approval workflow, leave balance, etc.
        }

        private void LoadReportsSection()
        {
            // Reports and analytics
            // Generate various HR reports, charts, export functionality
            ContentArea.Content = new Views.ReportView();
        }

        private void LoadSettingsSection()
        {
            // System settings
            // User management, company settings, system configuration
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?",
                                       "Logout Confirmation",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // In a real application, you would:
                // 1. Clear user session data
                // 2. Navigate to login window
                // 3. Close current window

                MessageBox.Show("Logout functionality would be implemented here.\nSession cleared and returning to login screen.",
                              "Logout", MessageBoxButton.OK, MessageBoxImage.Information);

                // Example logout process:
                // LoginWindow loginWindow = new LoginWindow();
                // loginWindow.Show();
                // this.Close();
            }
        }
    }
}

        // Sample data models that would be used in a real application