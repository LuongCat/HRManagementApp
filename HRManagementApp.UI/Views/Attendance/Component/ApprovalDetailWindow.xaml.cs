using System.Windows;
using HRManagementApp.models;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views
{
    public partial class ApprovalDetailWindow : Window
    {
        private ApprovalModel _data;
        private String _userRole;
        private AttendanceBLL _bll = new AttendanceBLL();

        public ApprovalDetailWindow(ApprovalModel data, String userRole)
        {
            InitializeComponent();
            _data = data;
            _userRole = userRole;
            LoadData();
        }

        private void LoadData()
        {
            txtEmployee.Text = _data.Employee;
            txtLoaiDon.Text = _data.LoaiDon; // Đã có dữ liệu từ DAL
            txtDate.Text = _data.Date;
            txtTime.Text = $"Từ {_data.CheckIn} đến {_data.CheckOut}";
            txtReason.Text = _data.Reason;
            txtStatus.Text = _data.Status;
            txtApprover.Text = string.IsNullOrEmpty(_data.NguoiDuyet) ? "(Chưa có)" : _data.NguoiDuyet;

            // Logic hiển thị nút Duyệt/Từ chối:
            // Hiện nếu: Đơn chưa duyệt VÀ (Role là 1-Admin hoặc 2-Quản lý)
            if (_data.Status == "Chưa duyệt" && (_userRole == "Admin" || _userRole == "Quanly"))
            {
                pnlActions.Visibility = Visibility.Visible;
            }
            else
            {
                pnlActions.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnApprove_Click(object sender, RoutedEventArgs e)
        {
            // Lấy tên người đang đăng nhập để lưu vào DB
            string approverName = UserSession.VaiTro ?? "Admin";

            if (_bll.UpdateApprovalStatus(_data.MaDon, "Đã duyệt", approverName))
            {
                MessageBox.Show("Đã duyệt đơn thành công!");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra.");
            }
        }

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            string approverName = UserSession.VaiTro ?? "Admin";

            if (_bll.UpdateApprovalStatus(_data.MaDon, "Từ chối", approverName))
            {
                MessageBox.Show("Đã từ chối đơn.");
                this.DialogResult = true;
                this.Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}