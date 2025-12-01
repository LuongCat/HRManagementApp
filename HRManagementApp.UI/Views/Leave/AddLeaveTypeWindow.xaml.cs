using System;
using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views.Leave
{
    public partial class AddLeaveTypeWindow : Window
    {
        private readonly DonTuService _service;
        private LoaiDon _currentLoaiDon = null; // Biến để xác định đang Thêm hay Sửa

        public AddLeaveTypeWindow()
        {
            InitializeComponent();
            _service = new DonTuService();
        }

        // Hàm này được gọi từ bên ngoài để chuyển sang chế độ Sửa
        public void LoadDataForEdit(LoaiDon loai)
        {
            _currentLoaiDon = loai;
            
            // Đổi tiêu đề window và điền dữ liệu
            this.Title = "Cập nhật loại đơn"; 
            TxtTenLoai.Text = loai.TenLoaiDon;
            TxtMoTa.Text = loai.MoTa;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validate dữ liệu
            if (string.IsNullOrWhiteSpace(TxtTenLoai.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại đơn!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTenLoai.Focus();
                return;
            }

            try
            {
                bool isSuccess = false;
                string successMessage = "";

                // 2. Kiểm tra xem là Thêm mới hay Cập nhật
                if (_currentLoaiDon == null)
                {
                    // --- LOGIC THÊM MỚI ---
                    var newItem = new LoaiDon
                    {
                        TenLoaiDon = TxtTenLoai.Text.Trim(),
                        MoTa = TxtMoTa.Text.Trim()
                    };
                    isSuccess = _service.AddLeaveType(newItem);
                    successMessage = "Thêm loại đơn thành công!";
                }
                else
                {
                    // --- LOGIC CẬP NHẬT ---
                    // Update giá trị vào object hiện tại
                    _currentLoaiDon.TenLoaiDon = TxtTenLoai.Text.Trim();
                    _currentLoaiDon.MoTa = TxtMoTa.Text.Trim();
                    _currentLoaiDon.CoLuong = CmbCoLuong.Text.Trim();
                    isSuccess = _service.UpdateLeaveType(_currentLoaiDon);
                    successMessage = "Cập nhật loại đơn thành công!";
                }

                // 3. Xử lý kết quả
                if (isSuccess)
                {
                    MessageBox.Show(successMessage, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Chỉ gán DialogResult nếu cửa sổ đang mở dạng Modal (ShowDialog)
                    if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
                    {
                        this.DialogResult = true; 
                    }
                    
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Thao tác thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}