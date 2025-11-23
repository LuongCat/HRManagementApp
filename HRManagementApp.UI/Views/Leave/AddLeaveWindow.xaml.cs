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
        private readonly int? _currentEmployeeId; // Biến lưu ID nhân viên đăng nhập (nếu có)

        // Constructor mặc định (Dành cho Admin)
        public AddLeaveWindow()
        {
            InitializeComponent();
            _donTuService = new DonTuService();
            _nhanVienService = new NhanVienService();
            _currentEmployeeId = null;
            LoadFormData();
        }

        // Constructor dành cho Nhân viên (Truyền vào ID của họ)
        public AddLeaveWindow(int employeeId) : this()
        {
            _currentEmployeeId = employeeId;
            
            // Cài đặt giao diện cho chế độ Nhân viên
            SetupEmployeeMode();
        }

        private void LoadFormData()
        {
            try 
            {
                CbNhanVien.ItemsSource = _nhanVienService.GetListNhanVien();
                CbLoaiDon.ItemsSource = _donTuService.GetLoaiDonList();
                DpTuNgay.SelectedDate = DateTime.Today;
                DpDenNgay.SelectedDate = DateTime.Today;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi");
            }
        }

        // Thiết lập giao diện khi là Nhân viên
        private void SetupEmployeeMode()
        {
            if (_currentEmployeeId.HasValue)
            {
                // Tự động chọn nhân viên đang đăng nhập
                CbNhanVien.SelectedValue = _currentEmployeeId.Value;
                
                // Khóa ComboBox để họ không chọn người khác được
                CbNhanVien.IsEnabled = false; 
                
                // (Optional) Đổi màu nền để báo hiệu read-only
                CbNhanVien.Opacity = 0.7;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            if (CbNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Lỗi: Không xác định được nhân viên.");
                return;
            }
            if (CbLoaiDon.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại đơn!");
                return;
            }
            if (!DpTuNgay.SelectedDate.HasValue || !DpDenNgay.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày!");
                return;
            }

            var don = new DonTu
            {
                MaNV = (int)CbNhanVien.SelectedValue, // Lấy giá trị đã chọn (hoặc đã fix sẵn)
                MaLoaiDon = (int)CbLoaiDon.SelectedValue,
                NgayBatDau = DpTuNgay.SelectedDate.Value,
                NgayKetThuc = DpDenNgay.SelectedDate.Value,
                LyDo = TxtLyDo.Text
            };

            try 
            {
                _donTuService.AddLeaveRequest(don);
                MessageBox.Show("Nộp đơn thành công! Vui lòng chờ duyệt.", "Thông báo");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}