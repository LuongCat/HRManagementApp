using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HRManagementApp.BLL;
using HRManagementApp.models;
namespace HRManagementApp.UI.Views
{
    public partial class PayrollTag : UserControl
    {
        
        private LuongService _luongService;
        public PayrollTag()
        {
            InitializeComponent();
            _luongService = new LuongService();

            // Load card data khi khởi tạo
            LoadCardData();
            
            // Load tab đầu tiên mặc định
            SetActiveTab(BtnBangLuong);
            LoadPayrollView();
        }

        
        // ==========================================
        //  Hàm Load dữ liệu cho các Card Dashboard
        // ==========================================
        private void LoadCardData()
        {
            try
            {
                // Lấy tháng và năm hiện tại để tổng hợp
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                // 1. Lấy dữ liệu tổng hợp từ DAL
                PayrollSummary summary = _luongService.GetPayrollSummary(currentMonth, currentYear);
            
                // 2. Định dạng tiền tệ
                string formatCurrency(decimal amount)
                {
                    // Ví dụ định dạng: 66.400.000 đ
                    // Dùng CultureInfo phù hợp cho VN, hoặc đơn giản hóa:
                    return amount.ToString("N0", new System.Globalization.CultureInfo("vi-VN")) + " đ";
                }

                // 3. Cập nhật TextBlock trên UI
                TotalSalary.Text = formatCurrency(summary.TotalSalary);
                PaidSalary.Text = formatCurrency(summary.PaidSalary);
                UnpaidSalary.Text = formatCurrency(summary.UnpaidSalary);
            
                // Card Số nhân viên
                TotalEmployees.Text = summary.TotalEmployees.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu thẻ lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                // Có thể gán giá trị mặc định 0 nếu xảy ra lỗi
                TotalSalary.Text = "0 đ";
                PaidSalary.Text = "0 đ";
                UnpaidSalary.Text = "0 đ";
                TotalEmployees.Text = "0";
            }
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

        

        // ================================
        //  Load các view tương ứng
        // ================================
        private void LoadPayrollView()
        {
            MainContent.Content = new PayrollView();
        }

        private void LoadDepartmentView()
        {
            MainContent.Content = new DepartmentPayrollView(); 
            
        }
        

        // ================================
        //  UI Tab Active / Inactive
        // ================================
        private void SetActiveTab(Button activeButton)
        {
            ResetTabButton(BtnBangLuong);
            ResetTabButton(BtnByDeparment);
           // ResetTabButton(BtnSetting);

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
