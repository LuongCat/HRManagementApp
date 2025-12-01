using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HRManagementApp.UI.Views
{
    public partial class AttendanceTagView : UserControl
    {
        private Button currentActiveButton = null!;

        public AttendanceTagView()
        {
            InitializeComponent();
            // Load trang mặc định: Chấm công hôm nay
            SetActiveButton(btnTodayAttendance);
            LoadTodayAttendance();
        } 

        private void BtnTodayAttendance_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            LoadTodayAttendance();
        }

        private void BtnApproval_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            LoadApprovalPage();
        }

        // ĐÃ XÓA SỰ KIỆN BtnLocations_Click

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            LoadReportsPage();
        }
    
        private void BtnSummary_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            LoadSummarysPage();
        }
        
        private void BtnManagement_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            LoadMagementAttendancePage();
        }
        
        
        private void SetActiveButton(Button activeButton)
        {
            if (currentActiveButton != null)
            {
                currentActiveButton.Background = new SolidColorBrush(Color.FromRgb(232, 232, 232));
                currentActiveButton.FontWeight = FontWeights.Normal;
            }

            if (activeButton != null)
            {
                activeButton.Background = Brushes.White;
                activeButton.FontWeight = FontWeights.SemiBold;
                currentActiveButton = activeButton;
            }
        }

        private void LoadTodayAttendance()
        {
            ContentArea.Content = new TodayAttendanceView();
        }

        private void LoadApprovalPage()
        {
            ContentArea.Content = new ApprovalView();
        }

        private void LoadReportsPage()
        {
            ContentArea.Content = new ReportsView();
        }
        private void LoadSummarysPage()
        {
            ContentArea.Content = new AttendanceSummaryView();
        }
        
        private void LoadMagementAttendancePage()
        {
            ContentArea.Content = new AttendanceManagementView();
        }
    }
}