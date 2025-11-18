using System.Windows;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    /// <summary>
    /// Window hiển thị danh sách nhân viên theo phòng ban
    /// </summary>
    public partial class DepartmentEmployeesWindow : Window
    {
        public DepartmentEmployeesWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set thông tin phòng ban và danh sách nhân viên
        /// </summary>
        public void SetDepartmentInfo(string maPhongBan, string tenPhongBan, string truongPhong, string moTa)
        {
            TxtMaPhongBan.Text = maPhongBan;
            TxtDepartmentName.Text = tenPhongBan.ToUpper();
            TxtTruongPhong.Text = truongPhong;
            TxtMoTa.Text = moTa;
        }

        /// <summary>
        /// Set thống kê số lượng nhân viên
        /// </summary>
        public void SetStatistics(int total, int active, int inactive)
        {
            TxtTotalEmployees.Text = total.ToString();
            TxtActiveEmployees.Text = active.ToString();
            TxtInactiveEmployees.Text = inactive.ToString();
        }
        
        /// Set danh sách nhân viên (ItemsSource cho DataGrid)
        public void SetEmployees(List<NhanVien> employeesList)
        {
            EmployeesDataGrid.ItemsSource = employeesList;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement xuất báo cáo Excel
            MessageBox.Show("Chức năng xuất báo cáo đang được phát triển", 
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}