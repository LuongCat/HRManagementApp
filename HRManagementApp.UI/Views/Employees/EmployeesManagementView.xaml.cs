using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HRManagementApp.UI.Views
{
    /// <summary>
    /// View chính quản lý Nhân viên với tabs (Danh sách, Phòng ban, Chức vụ)
    /// </summary>
    public partial class EmployeesManagementView : UserControl
    {
        public EmployeesManagementView()
        {
            InitializeComponent();
            
            // Load view mặc định là Danh sách nhân viên
            LoadEmployeesView();
        }

        // ✅ Xử lý click tab Danh sách nhân viên
        private void BtnDanhSach_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnDanhSach);
            LoadEmployeesView();
        }

        // ✅ Xử lý click tab Phòng ban
        private void BtnPhongBan_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnPhongBan);
            LoadDepartmentView();
        }

        // ✅ Xử lý click tab Chức vụ
        private void BtnChucVu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnChucVu);
            LoadPositionView();
        }
        
        private void BtnCaLam_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnCaLam);
            LoadShiftManagementView();
        }
        private void BtnEditCaLam_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnEditCaLam);
            LoadEditCaLamView();
            
        }
        private void LoadEditCaLamView()
        {
            MainContent.Content = new ShiftListView();
        }
        private void LoadShiftManagementView()
        {
            // Đây là UserControl chúng ta sẽ tạo ở Phần 2 bên dưới
            MainContent.Content = new ShiftManagementView(); 
        }
        
        // ✅ Load view Danh sách nhân viên
        private void LoadEmployeesView()
        {
            MainContent.Content = new EmployeesView();
        }

        // ✅ Load view Phòng ban
        private void LoadDepartmentView()
        {
            MainContent.Content = new DepartmentListView();
        }

        // ✅ Load view Chức vụ
        private void LoadPositionView()
        {
           MainContent.Content = new PositionListView();
        }

        // ✅ Đặt tab active (đổi màu nền và chữ)
        private void SetActiveTab(Button activeButton)
        {
            // Reset tất cả các tab về trạng thái inactive (không active)
            ResetTabButton(BtnDanhSach);
            ResetTabButton(BtnPhongBan);
            ResetTabButton(BtnChucVu);
            ResetTabButton(BtnCaLam);
            ResetTabButton(BtnEditCaLam);
            // Set tab được chọn thành active (nền đen, chữ trắng)
            activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F2937"));
            activeButton.Foreground = Brushes.White;
        }

        // ✅ Reset button về trạng thái inactive
        private void ResetTabButton(Button button)
        {
            button.Background = Brushes.Transparent;
            button.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
        }
    }
}