using System.Windows.Controls;

namespace HRManagementApp.UI.Views.Report
{
    /// <summary>
    /// Interaction logic for AnalyticsReportView.xaml
    /// </summary>
    public partial class AnalyticsReportView : UserControl
    {
        public AnalyticsReportView()
        {
            InitializeComponent();
            // DataContext = new AnalyticsReportViewModel(); // Uncomment khi bạn đã sẵn sàng với ViewModel
        }
    }
}