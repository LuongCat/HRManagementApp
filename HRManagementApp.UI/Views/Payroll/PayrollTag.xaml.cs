using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HRManagementApp.UI.Views
{
    public partial class PayrollTag : UserControl
    {
        public PayrollTag()
        {
            InitializeComponent();

            // Load tab đầu tiên mặc định
            SetActiveTab(BtnBangLuong);
            LoadPayrollView();
        }

        // ================================
        //  Sự kiện Click Tabs
        // ================================
        private void BtnBangLuong_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnBangLuong);
            LoadPayrollView();
        }

        private void BtnByDeparemnt_click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnByDeparment);
            LoadDepartmentView();
        }

        private void BtnSetting_click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnSetting);
            LoadPositionView();
        }

        // ================================
        //  Load các view tương ứng
        // ================================
        private void LoadPayrollView()
        {
            MainContent.Content = new PayrollView();
        }

        private void LoadDepartmentView()
        {
            //MainContent.Content = new DepartmentPayrollView(); // <-- tạo UC này
            
        }

        private void LoadPositionView()
        {
           // MainContent.Content = new PayrollSettingView(); // <-- tạo UC này
        }

        // ================================
        //  UI Tab Active / Inactive
        // ================================
        private void SetActiveTab(Button activeButton)
        {
            ResetTabButton(BtnBangLuong);
            ResetTabButton(BtnByDeparment);
            ResetTabButton(BtnSetting);

            activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F2937"));
            activeButton.Foreground = Brushes.White;
        }

        private void ResetTabButton(Button button)
        {
            button.Background = Brushes.Transparent;
            button.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
        }
    }
}
