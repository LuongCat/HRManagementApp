using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class AddAccountWindow : Window
    {
        public AccountManagementModel CreatedAccount { get; private set; }
        private AccountBLL bll = new AccountBLL();
        
        // Nguồn dữ liệu để filter
        private DataView _employeeDataView;
        
        // Cờ kiểm tra trạng thái chọn hợp lệ
        private bool _isSelectionValid = false;

        public AddAccountWindow()
        {
            InitializeComponent();
            LoadInitData();
            
            // Focus vào ô đầu tiên khi mở form
            txtTenDangNhap.Focus();
        }

        private void LoadInitData()
        {
            try
            {
                // 1. Load Vai trò
                cboVaiTro.ItemsSource = bll.GetRoles();
                if(cboVaiTro.Items.Count > 0) cboVaiTro.SelectedIndex = 0;

                // 2. Load Nhân viên
                DataTable dtNV = bll.GetEmployeesForCombo();
                _employeeDataView = dtNV.DefaultView;
                cboNhanVien.ItemsSource = _employeeDataView;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        // --- XỬ LÝ PHÍM ENTER ĐỂ CHUYỂN FOCUS (TAB) ---
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Nếu phím là Enter
            if (e.Key == Key.Enter)
            {
                // Bỏ qua nếu đang ở nút Tạo hoặc Hủy (để người dùng có thể nhấn Enter để submit)
                if (e.Source is Button) return;

                // Giả lập hành động nhấn phím TAB
                var request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                
                // Lấy element đang có focus hiện tại
                var element = Keyboard.FocusedElement as UIElement;
                if (element != null)
                {
                    element.MoveFocus(request);
                }
                
                // Ngăn chặn hành vi mặc định (ví dụ tiếng beep hoặc submit form ngoài ý muốn)
                e.Handled = true;
            }
        }

        // --- LOGIC TÌM KIẾM (FILTER) ---
        private void cboNhanVien_KeyUp(object sender, KeyEventArgs e)
        {
            // Nếu là các phím điều hướng thì không filter lại
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Enter || e.Key == Key.Tab)
                return;

            // Đánh dấu là người dùng đang gõ -> chưa chọn xong -> không hợp lệ
            _isSelectionValid = false;

            var cmb = sender as ComboBox;
            // Mở dropdown để thấy gợi ý
            if (!cmb.IsDropDownOpen) cmb.IsDropDownOpen = true;

            // Lấy text người dùng đang gõ
            string searchText = cmb.Text.ToLower();

            // Thực hiện Filter trên DataView
            if (_employeeDataView != null)
            {
                if (string.IsNullOrEmpty(searchText))
                {
                    _employeeDataView.RowFilter = "";
                }
                else
                {
                    // Lọc theo Tên (gần đúng)
                    // "HoTen" là tên cột trong Database/DataTable
                    _employeeDataView.RowFilter = $"HoTen LIKE '%{searchText}%'";
                }
            }

            // Mẹo: Đặt lại con trỏ chuột xuống cuối text box (do filter đôi khi làm reset con trỏ)
            var textBox = cmb.Template.FindName("PART_EditableTextBox", cmb) as TextBox;
            if (textBox != null)
            {
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        // --- LOGIC CHẤP NHẬN DỮ LIỆU KHI CHỌN ---
        private void cboNhanVien_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Chỉ xử lý khi có item được chọn thực sự (người dùng click hoặc Enter vào item)
            if (cboNhanVien.SelectedItem != null)
            {
                var row = cboNhanVien.SelectedItem as DataRowView;
                if (row != null)
                {
                    _isSelectionValid = true; // Đánh dấu hợp lệ
                    
                    // Lấy MaNV
                    int maNV = (int)row["MaNV"];
                    
                    // Gọi BLL lấy thông tin chi tiết (SĐT, Phòng ban)
                    DataRow info = bll.GetEmployeeInfo(maNV);
                    if (info != null)
                    {
                        txtPhongBan.Text = info["TenPB"].ToString();
                        txtSDT.Text = info["DienThoai"].ToString();
                    }
                }
            }
            else
            {
                _isSelectionValid = false;
                ResetEmployeeInfo();
            }
        }

        // --- LOGIC XÓA DỮ LIỆU KHI MẤT FOCUS (NẾU CHƯA CHỌN) ---
        private void cboNhanVien_LostFocus(object sender, RoutedEventArgs e)
        {
            // Nếu biến cờ báo chưa chọn hợp lệ -> Xóa trắng
            if (!_isSelectionValid)
            {
                cboNhanVien.Text = "";
                cboNhanVien.SelectedItem = null;
                
                // Reset filter để lần sau mở ra thấy đủ danh sách
                if (_employeeDataView != null) _employeeDataView.RowFilter = "";
                
                ResetEmployeeInfo();
            }
        }

        private void ResetEmployeeInfo()
        {
            txtPhongBan.Text = "";
            txtSDT.Text = "";
        }

        // --- LOGIC LƯU TÀI KHOẢN ---
        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra Validation lần cuối
            if (!_isSelectionValid || cboNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên từ danh sách gợi ý!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                cboNhanVien.Focus();
                return;
            }

            string username = txtTenDangNhap.Text.Trim();
            string pass = pwdPassword.Password;
            string confirm = pwdConfirm.Password;
            
            // Xử lý lấy vai trò (String)
            string role = "";
            if (cboVaiTro.SelectedItem != null)
            {
                // Tùy vào nguồn dữ liệu của cboVaiTro là List<string> hay DataTable
                // Code cũ bạn dùng List<string> -> ToString() là ok
                // Nếu dùng DataTable -> (cboVaiTro.SelectedItem as DataRowView)["TenVaiTro"].ToString()
                role = cboVaiTro.SelectedItem.ToString();
            }

            AccountManagementModel acc = new AccountManagementModel
            {
                TenDangNhap = username,
                VaiTro = role,
                MaNV = (int)cboNhanVien.SelectedValue
            };

            string result = bll.AddAccountBLL(acc, pass, confirm);

            if (result == "Success")
            {
                MessageBox.Show("Tạo tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                CreatedAccount = acc;
                CreatedAccount.Name = cboNhanVien.Text; // Lấy tên hiển thị
                CreatedAccount.TrangThai = "Hoạt động";
                
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(result, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}