using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class ShiftListView : UserControl
    {
        private readonly CaLamService _service = new CaLamService();

        public ShiftListView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // Lấy danh sách ca làm từ Service
            DgvShifts.ItemsSource = _service.GetAllCaLam();
        }

        // Thêm mới
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ShiftDetailWindow popup = new ShiftDetailWindow();
            popup.ShowDialog();
            if (popup.IsSaved) LoadData(); // Refresh nếu có lưu
        }

        // Sửa
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            int maCa = (int)((Button)sender).Tag;
            var ca = _service.GetCaLamById(maCa);
            
            if (ca != null)
            {
                ShiftDetailWindow popup = new ShiftDetailWindow();
                popup.LoadData(ca); // Load dữ liệu cũ lên form
                popup.ShowDialog();
                if (popup.IsSaved) LoadData();
            }
        }

        // Xóa
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            int maCa = (int)((Button)sender).Tag;
            if (MessageBox.Show("Bạn có chắc muốn xóa ca làm việc này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (_service.DeleteCaLam(maCa))
                {
                    MessageBox.Show("Đã xóa thành công!");
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!");
                }
            }
        }
    }
}