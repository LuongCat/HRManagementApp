using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models; // Để dùng UserSession

namespace HRManagementApp.UI.Views.Leave
{
    public partial class MyLeaveView : UserControl
    {
        private readonly DonTuService _service;
        private int _currentEmployeeId; 

        public MyLeaveView(int employeeId = 0)
        {
            InitializeComponent();
            _service = new DonTuService();
            
            // Ưu tiên lấy từ Session nếu không truyền vào (hoặc truyền 0)
            if (employeeId == 0 && UserSession.MaNV.HasValue)
            {
                _currentEmployeeId = UserSession.MaNV.Value;
            }
            else
            {
                _currentEmployeeId = employeeId;
            }

            // Kiểm tra nếu có ID hợp lệ mới load
            if (_currentEmployeeId > 0)
            {
                LoadData();
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin nhân viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            // Lấy danh sách đơn của riêng nhân viên này
            DgMyRequests.ItemsSource = _service.GetMyLeaveRequests(_currentEmployeeId);
        }

        private void BtnCreateRequest_Click(object sender, RoutedEventArgs e)
        {
            // Mở form tạo đơn mới
            var window = new AddLeaveWindow();
            
            if (window.ShowDialog() == true)
            {
                LoadData(); // Refresh lại danh sách sau khi nộp thành công
            }
        }
    }
}