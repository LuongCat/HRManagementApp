using System;
using System.Collections.Generic;
using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views.Leave
{
    public partial class AddLeaveWindow : Window
    {
        private readonly DonTuService _donTuService;

        public AddLeaveWindow()
        {
            InitializeComponent();
            _donTuService = new DonTuService();
            LoadFormData();
        }

        private void LoadFormData()
        {
            try 
            {
                // 1. Load thông tin người dùng hiện tại vào ComboBox Nhân viên
                if (UserSession.MaNV.HasValue)
                {
                    // Tạo một list giả để gán vào ComboBox, chỉ chứa chính mình
                    var currentUserList = new List<dynamic> 
                    { 
                        new { MaNV = UserSession.MaNV.Value, HoTen = UserSession.HoTen } 
                    };
                    
                    CbNhanVien.ItemsSource = currentUserList;
                    CbNhanVien.SelectedIndex = 0; // Tự động chọn
                    CbNhanVien.IsEnabled = false; // Khóa lại không cho chọn người khác
                }

                // 2. Load danh sách loại đơn từ Database
                CbLoaiDon.ItemsSource = _donTuService.GetLoaiDonList();
                CbLoaiDon.SelectedIndex = 0;

                // 3. Set ngày mặc định là hôm nay
                DpTuNgay.SelectedDate = DateTime.Today;
                DpDenNgay.SelectedDate = DateTime.Today;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // VALIDATION
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

            // Lấy MaNV từ Session cho an toàn (tránh trường hợp UI bị hack mở khóa combobox)
            int currentMaNV = UserSession.MaNV ?? 0;
            if (currentMaNV == 0)
            {
                MessageBox.Show("Phiên đăng nhập không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // TẠO OBJECT
            var don = new DonTu
            {
                MaNV = currentMaNV,
                MaLoaiDon = (int)CbLoaiDon.SelectedValue,
                NgayBatDau = DpTuNgay.SelectedDate.Value,
                NgayKetThuc = DpDenNgay.SelectedDate.Value,
                LyDo = TxtLyDo.Text.Trim(),
            };

            // GỌI SERVICE LƯU
            try 
            {
                bool result = _donTuService.AddLeaveRequest(don);
                if (result)
                {
                    MessageBox.Show("Gửi đơn thành công! Vui lòng chờ quản lý duyệt.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true; // Đóng và báo thành công về form cha
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