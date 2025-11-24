using System.Windows;
using System.Windows.Controls;

namespace HRManagementApp.UI.Views
{
    public partial class AttendanceTag : UserControl
    {
        public AttendanceTag()
        {
            InitializeComponent();
            // Load mặc định view Chấm công hôm nay
            MainContent.Content = new AttendanceView();
        }

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as RadioButton;
            if (btn == null) return;

            string tabName = btn.Content.ToString();

            // Logic chuyển Tab
            switch (tabName)
            {
                case "Chấm công hôm nay":
                    MainContent.Content = new AttendanceView();
                    break;
                case "Công của nhân viên":
                     MainContent.Content = new AttendanceSummaryView(); 
                    break;
            }
        }
    }
}   