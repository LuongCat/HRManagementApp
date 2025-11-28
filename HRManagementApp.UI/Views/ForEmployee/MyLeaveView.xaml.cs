using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views.Leave
{
    public partial class MyLeaveView : UserControl
    {
        private readonly DonTuService _service;
        private int _currentEmployeeId ; 

        // Giả sử chúng ta sẽ truyền ID nhân viên vào đây
        // Trong thực tế, bạn sẽ lấy từ biến toàn cục Session.CurrentEmployeeId
        public MyLeaveView(int employeeId)
        {
            InitializeComponent();
            _service = new DonTuService();
            _currentEmployeeId = employeeId;
            LoadData();
        }

        private void LoadData()
        {
            // Lấy danh sách đơn của riêng nhân viên này
            DgMyRequests.ItemsSource = _service.GetMyLeaveRequests(_currentEmployeeId);
        }

        private void BtnCreateRequest_Click(object sender, RoutedEventArgs e)
        {
            // Gọi lại window AddLeaveWindow nhưng truyền ID vào
            // -> Giao diện sẽ tự động chuyển sang chế độ "Nộp đơn cá nhân"
            var window = new AddLeaveWindow();
            
            if (window.ShowDialog() == true)
            {
                LoadData(); // Refresh lại danh sách sau khi nộp
            }
        }
    }
}