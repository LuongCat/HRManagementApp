using System;
using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class ShiftDetailWindow : Window
    {
        private readonly CaLamService _service = new CaLamService();
        public bool IsSaved { get; private set; } = false;
        private int _currentId = 0; // 0 là thêm mới, >0 là sửa

        public ShiftDetailWindow()
        {
            InitializeComponent();
        }

        // Hàm load dữ liệu để sửa
        public void LoadData(CaLam ca)
        {
            _currentId = ca.MaCa;
            TxtTenCa.Text = ca.TenCa;
            // Format TimeSpan thành chuỗi HH:mm
            TxtGioBatDau.Text = ca.GioBatDau.ToString(@"hh\:mm");
            TxtGioKetThuc.Text = ca.GioKetThuc.ToString(@"hh\:mm");
            CboTrangThai.Text = ca.TrangThai;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validate
            if (string.IsNullOrWhiteSpace(TxtTenCa.Text))
            {
                MessageBox.Show("Vui lòng nhập tên ca!", "Cảnh báo"); return;
            }

            if (!TimeSpan.TryParse(TxtGioBatDau.Text, out TimeSpan batDau))
            {
                MessageBox.Show("Giờ bắt đầu không hợp lệ! (Định dạng đúng: HH:mm, vd: 08:00)", "Cảnh báo"); return;
            }

            if (!TimeSpan.TryParse(TxtGioKetThuc.Text, out TimeSpan ketThuc))
            {
                MessageBox.Show("Giờ kết thúc không hợp lệ!", "Cảnh báo"); return;
            }

            // 2. Tạo object
            CaLam ca = new CaLam
            {
                MaCa = _currentId,
                TenCa = TxtTenCa.Text.Trim(),
                GioBatDau = batDau,
                GioKetThuc = ketThuc,
                TrangThai = CboTrangThai.Text
            };

            // 3. Gọi Service
            bool result;
            if (_currentId == 0)
                result = _service.InsertCaLam(ca);
            else
                result = _service.UpdateCaLam(ca);

            if (result)
            {
                IsSaved = true;
                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Lưu thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}