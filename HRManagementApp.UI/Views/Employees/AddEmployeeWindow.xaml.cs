using System;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    /// <summary>
    /// Form thêm nhân viên mới
    /// </summary>
    public partial class AddEmployeeWindow : Window
    {
        private readonly NhanVienService _nhanVienService;
        private readonly PhongBanService _phongBanService;
        private readonly ChucVuService _chucVuService;

        public AddEmployeeWindow()
        {
            InitializeComponent();
            
            _nhanVienService = new NhanVienService();
            _phongBanService = new PhongBanService();
            _chucVuService = new ChucVuService();
            
            LoadComboBoxData();
        }

        /// <summary>
        /// Load dữ liệu cho các ComboBox
        /// </summary>
        private void LoadComboBoxData()
        {
            try
            {
                // Load Phòng ban
                var phongBans = _phongBanService.GetListPhongBan();
                CbPhongBan.ItemsSource = phongBans;
                CbPhongBan.DisplayMemberPath = "TenPB";
                CbPhongBan.SelectedValuePath = "MaPB";

                // Load Chức vụ
                var chucVus = _chucVuService.GetAllChucVu();
                CbChucVu.ItemsSource = chucVus;
                CbChucVu.DisplayMemberPath = "TenCV";
                CbChucVu.SelectedValuePath = "MaCV";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", 
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lưu thông tin nhân viên
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate dữ liệu
            if (!ValidateInput())
                return;

            try
            {
                // Lấy dữ liệu từ form
                var nhanVien = GetEmployeeDataFromForm();

                // Gọi service để lưu vào database
                // TODO: Implement AddNhanVien trong service
                
                // bool success = _nhanVienService.AddNhanVien(nhanVien);

                // Tạm thời chỉ hiển thị thông báo
                MessageBox.Show(
                    $"Dữ liệu đã nhận:\n\n" +
                    $"Họ tên: {nhanVien.HoTen}\n" +
                    $"Ngày sinh: {nhanVien.NgaySinh:dd/MM/yyyy}\n" +
                    $"Giới tính: {(nhanVien.GioiTinh ? "Nam" : "Nữ")}\n" +
                    $"CCCD: {nhanVien.SoCCCD}\n" +
                    $"Điện thoại: {nhanVien.DienThoai}\n" +
                   
                    $"Ngày vào làm: {nhanVien.NgayVaoLam:dd/MM/yyyy}\n" +
                    $"Trạng thái: {nhanVien.TrangThai}",
                    "Thông tin nhận được", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);

                // Uncomment khi đã implement service
                 if (success)
                 {
                    MessageBox.Show("Thêm nhân viên thành công!", "Thành công", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                     this.DialogResult = true;
                     this.Close();
                 }
                 else
                 {
                     MessageBox.Show("Thêm nhân viên thất bại!", "Lỗi", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                 }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu nhân viên: {ex.Message}", 
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lấy dữ liệu nhân viên từ form
        /// </summary>
        private NhanVien GetEmployeeDataFromForm()
        {
            var nhanVien = new NhanVien
            {
                // Thông tin cơ bản
                HoTen = TxtHoTen.Text.Trim(),
                NgaySinh = DpNgaySinh.SelectedDate ?? DateTime.Now,
                GioiTinh = GetGioiTinh(),
                SoCCCD = TxtCCCD.Text.Trim(),
                DienThoai = TxtDienThoai.Text.Trim(),
                

                // Thông tin công việc
                NgayVaoLam = DpNgayVaoLam.SelectedDate,
                TrangThai = GetTrangThai()
            };

            return nhanVien;
        }

        /// <summary>
        /// Lấy giới tính từ ComboBox
        /// </summary>
        private bool GetGioiTinh()
        {
            var selectedItem = CbGioiTinh.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
                return true; // Mặc định Nam

            string value = selectedItem.Content.ToString();
            return value == "Nam";
        }

        /// <summary>
        /// Lấy ID phòng ban được chọn
        /// </summary>
        private int GetSelectedPhongBanId()
        {
            if (CbPhongBan.SelectedValue != null)
            {
                return Convert.ToInt32(CbPhongBan.SelectedValue);
            }
            return 0;
        }

        /// <summary>
        /// Lấy ID chức vụ được chọn
        /// </summary>
        private int GetSelectedChucVuId()
        {
            if (CbChucVu.SelectedValue != null)
            {
                return Convert.ToInt32(CbChucVu.SelectedValue);
            }
            return 0;
        }

        /// <summary>
        /// Lấy trạng thái từ ComboBox
        /// </summary>
        private string GetTrangThai()
        {
            var selectedItem = CbTrangThai.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                return selectedItem.Content.ToString();
            }
            return "Đang làm"; // Mặc định
        }

        /// <summary>
        /// Validate dữ liệu đầu vào
        /// </summary>
        private bool ValidateInput()
        {
            // Validate Họ tên
            if (string.IsNullOrWhiteSpace(TxtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên!", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtHoTen.Focus();
                return false;
            }

            // Validate Ngày sinh
            if (!DpNgaySinh.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày sinh!", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                DpNgaySinh.Focus();
                return false;
            }

            // Validate tuổi (phải từ 18-65)
            var age = DateTime.Now.Year - DpNgaySinh.SelectedDate.Value.Year;
            if (age < 18 || age > 65)
            {
                MessageBox.Show("Nhân viên phải từ 18 đến 65 tuổi!", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                DpNgaySinh.Focus();
                return false;
            }

            // Validate Giới tính
            if (CbGioiTinh.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn giới tính!", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                CbGioiTinh.Focus();
                return false;
            }

            // Validate Điện thoại
            if (string.IsNullOrWhiteSpace(TxtDienThoai.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtDienThoai.Focus();
                return false;
            }

            // Validate format số điện thoại (10 số, bắt đầu bằng 0)
            string phone = TxtDienThoai.Text.Trim();
            if (phone.Length != 10 || !phone.StartsWith("0"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! (Phải 10 số và bắt đầu bằng 0)", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtDienThoai.Focus();
                return false;
            }

            // Validate Phòng ban
            if (CbPhongBan.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn phòng ban!", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                CbPhongBan.Focus();
                return false;
            }

            // Validate Chức vụ
            if (CbChucVu.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn chức vụ!", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                CbChucVu.Focus();
                return false;
            }

            // Validate Ngày vào làm
            if (!DpNgayVaoLam.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày vào làm!", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                DpNgayVaoLam.Focus();
                return false;
            }

            // Validate ngày vào làm không được trong tương lai
            if (DpNgayVaoLam.SelectedDate.Value > DateTime.Now)
            {
                MessageBox.Show("Ngày vào làm không được ở tương lai!", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                DpNgayVaoLam.Focus();
                return false;
            }

            // Validate Trạng thái
            if (CbTrangThai.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn trạng thái!", "Cảnh báo", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                CbTrangThai.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra email hợp lệ
        /// </summary>

        /// <summary>
        /// Đóng form
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}