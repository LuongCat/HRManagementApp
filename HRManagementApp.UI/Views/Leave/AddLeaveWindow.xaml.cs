using System;
using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views.Leave
{
    public partial class AddLeaveWindow : Window
    {
        private readonly DonTuService _donTuService;
        private readonly NhanVienService _nhanVienService;

        public AddLeaveWindow()
        {
            InitializeComponent();
            _donTuService = new DonTuService();
            _nhanVienService = new NhanVienService();
            
            // Gọi hàm load dữ liệu ngay khi khởi tạo
            LoadFormData();
        }

        private void LoadFormData()
        {
            try 
            {
                // Load danh sách nhân viên vào ComboBox
                CbNhanVien.ItemsSource = _nhanVienService.GetListNhanVien();
                
                // Load danh sách loại đơn vào ComboBox
                CbLoaiDon.ItemsSource = _donTuService.GetLoaiDonList();

                // Set ngày mặc định là hôm nay
                DpTuNgay.SelectedDate = DateTime.Today;
                DpDenNgay.SelectedDate = DateTime.Today;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validate dữ liệu đầu vào
            if (CbNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CbLoaiDon.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại đơn!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!DpTuNgay.SelectedDate.HasValue || !DpDenNgay.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày nghỉ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(TxtLyDo.Text))
            {
                MessageBox.Show("Vui lòng nhập lý do nghỉ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtLyDo.Focus();
                return;
            }

            // 2. Tạo đối tượng DonTu
            var don = new DonTu
            {
                MaNV = (int)CbNhanVien.SelectedValue,
                MaLoaiDon = (int)CbLoaiDon.SelectedValue,
                NgayBatDau = DpTuNgay.SelectedDate.Value,
                NgayKetThuc = DpDenNgay.SelectedDate.Value,
                LyDo = TxtLyDo.Text.Trim(),
            };

            // 3. Gọi Service để lưu xuống DB
            try 
            {
                bool result = _donTuService.AddLeaveRequest(don);
                if (result)
                {
                    MessageBox.Show("Gửi đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true; // Trả về true để cửa sổ cha (LeaveManagementView) biết mà reload lại danh sách
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gửi đơn thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}