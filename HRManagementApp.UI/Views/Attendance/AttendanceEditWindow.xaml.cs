using System;
using System.Windows;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class AttendanceEditWindow : Window
    {
        public ChamCong ChamCongData { get; private set; }
        private bool _isEditMode = false;

        public AttendanceEditWindow(ChamCong existingItem = null)
        {
            InitializeComponent();

            if (existingItem != null)
            {
                // Chế độ Sửa
                _isEditMode = true;
                ChamCongData = existingItem;
                
                txtMaNV.Text = existingItem.MaNV.ToString();
                txtMaNV.IsEnabled = false; // Không cho sửa Mã NV khi update
                dpNgay.SelectedDate = existingItem.Ngay;
                
                if(existingItem.GioVao.HasValue) 
                    txtGioVao.Text = existingItem.GioVao.Value.ToString(@"hh\:mm");
                
                if(existingItem.GioRa.HasValue) 
                    txtGioRa.Text = existingItem.GioRa.Value.ToString(@"hh\:mm");
            }
            else
            {
                // Chế độ Thêm mới
                ChamCongData = new ChamCong();
                dpNgay.SelectedDate = DateTime.Now;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validate MaNV
            if (!int.TryParse(txtMaNV.Text, out int maNV))
            {
                MessageBox.Show("Mã nhân viên phải là số.");
                return;
            }

            // 2. Validate Giờ
            TimeSpan? gioVao = null;
            TimeSpan? gioRa = null;

            if (!string.IsNullOrEmpty(txtGioVao.Text))
            {
                if (TimeSpan.TryParse(txtGioVao.Text, out TimeSpan tIn)) gioVao = tIn;
                else { MessageBox.Show("Định dạng Giờ Vào không hợp lệ (vd: 08:00)"); return; }
            }

            if (!string.IsNullOrEmpty(txtGioRa.Text))
            {
                if (TimeSpan.TryParse(txtGioRa.Text, out TimeSpan tOut)) gioRa = tOut;
                else { MessageBox.Show("Định dạng Giờ Ra không hợp lệ (vd: 17:30)"); return; }
            }

            // 3. Gán dữ liệu
            ChamCongData.MaNV = maNV;
            ChamCongData.Ngay = dpNgay.SelectedDate;
            ChamCongData.GioVao = gioVao;
            ChamCongData.GioRa = gioRa;

            this.DialogResult = true; // Đóng window và báo thành công
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}