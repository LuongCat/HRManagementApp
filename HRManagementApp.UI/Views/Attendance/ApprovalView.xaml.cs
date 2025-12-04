using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class ApprovalView : UserControl
    {
        private AttendanceBLL _bll = new AttendanceBLL();

        public ApprovalView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            dgApprovals.ItemsSource = _bll.GetApprovals();
        }

        private void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var model = button?.CommandParameter as ApprovalModel;

            if (model != null)
            {
                // Lấy Role thực tế từ Session
                String currentRole = UserSession.VaiTro;

                // Mở cửa sổ chi tiết
                var detailWin = new ApprovalDetailWindow(model, currentRole);
                bool? result = detailWin.ShowDialog();

                // Reload nếu có thay đổi
                if (result == true)
                {
                    LoadData();
                }
            }
        }
    }
}