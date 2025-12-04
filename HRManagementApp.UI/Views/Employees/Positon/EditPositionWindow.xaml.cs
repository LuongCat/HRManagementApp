using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HRManagementApp.UI.Views
{
    public partial class EditPositionWindow : Window
    {
        public bool IsSaved { get; private set; } = false;
        public int MaChucVu { get; private set; }

        // Properties để lấy dữ liệu từ form
        public string TenChucVu => TxtTenCV.Text.Trim();
        public decimal LuongCoBan
        {
            get
            {
                if (decimal.TryParse(TxtLuongCB.Text.Trim(), out decimal result))
                    return result;
                return 0;
            }
        }
        public decimal PhuCap
        {
            get
            {
                if (decimal.TryParse(TxtPhuCap.Text.Trim(), out decimal result))
                    return result;
                return 0;
            }
        }
        public decimal PhuCapKiemNhiem
        {
            get
            {
                if (decimal.TryParse(TxtPhuCapKiemNhiem.Text.Trim(), out decimal result))
                    return result;
                return 0;
            }
        }
        public string TrangThai => ((ComboBoxItem)CbTrangThai.SelectedItem)?.Tag?.ToString() ?? "Active";

        // Lưu giá trị ban đầu để so sánh thay đổi
        private string _originalTenCV;
        private string _originalLuongCB;
        private string _originalPhuCap;
        private string _originalPhuCapKN;
        private int _originalTrangThaiIndex;

        public EditPositionWindow()
        {
            InitializeComponent();
            
            // Chỉ cho phép nhập số cho các trường tiền
            TxtLuongCB.PreviewTextInput += NumericTextBox_PreviewTextInput;
            TxtPhuCap.PreviewTextInput += NumericTextBox_PreviewTextInput;
            TxtPhuCapKiemNhiem.PreviewTextInput += NumericTextBox_PreviewTextInput;

            // Ngăn paste ký tự không hợp lệ
            TxtLuongCB.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(NumericTextBox_Pasting));
            TxtPhuCap.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(NumericTextBox_Pasting));
            TxtPhuCapKiemNhiem.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(NumericTextBox_Pasting));
        }

        /// <summary>
        /// Load dữ liệu chức vụ vào form
        /// </summary>
        public void LoadPositionData(int maChucVu, string tenChucVu, decimal luongCB, 
            decimal? phuCap, decimal? phuCapKN, string trangThai)
        {
            MaChucVu = maChucVu;
            
            // Hiển thị tên chức vụ ở header
            TxtPositionName.Text = tenChucVu;
            
            // Load dữ liệu vào form
            TxtTenCV.Text = tenChucVu;
            TxtLuongCB.Text = luongCB.ToString("0");
            TxtPhuCap.Text = phuCap.HasValue ? phuCap.Value.ToString("0") : "0";
            TxtPhuCapKiemNhiem.Text = phuCapKN.HasValue ? phuCapKN.Value.ToString("0") : "0";

            // Set trạng thái
            if (trangThai == "Active")
                CbTrangThai.SelectedIndex = 0;
            else
                CbTrangThai.SelectedIndex = 1;

            // Lưu giá trị ban đầu
            _originalTenCV = TxtTenCV.Text;
            _originalLuongCB = TxtLuongCB.Text;
            _originalPhuCap = TxtPhuCap.Text;
            _originalPhuCapKN = TxtPhuCapKiemNhiem.Text;
            _originalTrangThaiIndex = CbTrangThai.SelectedIndex;
        }

        // Chỉ cho phép nhập số
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsNumeric(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsNumeric(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        // Nút Lưu
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(TxtTenCV.Text))
            {
                MessageBox.Show("Vui lòng nhập tên chức vụ!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTenCV.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtLuongCB.Text) || LuongCoBan <= 0)
            {
                MessageBox.Show("Vui lòng nhập lương cơ bản hợp lệ!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtLuongCB.Focus();
                return;
            }

            // Kiểm tra xem có thay đổi gì không
            if (!HasChanges())
            {
                MessageBox.Show("Không có thay đổi nào để lưu!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Xác nhận lưu
            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn lưu các thay đổi cho chức vụ '{TenChucVu}'?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IsSaved = true;
                DialogResult = true;
                Close();
            }
        }

        // Nút Đóng/Hủy
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (HasChanges())
            {
                var result = MessageBox.Show(
                    "Bạn có thay đổi chưa lưu. Bạn có chắc chắn muốn đóng?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;
            }

            IsSaved = false;
            DialogResult = false;
            Close();
        }

        // Kiểm tra xem có thay đổi dữ liệu chưa
        private bool HasChanges()
        {
            return TxtTenCV.Text != _originalTenCV ||
                   TxtLuongCB.Text != _originalLuongCB ||
                   TxtPhuCap.Text != _originalPhuCap ||
                   TxtPhuCapKiemNhiem.Text != _originalPhuCapKN ||
                   CbTrangThai.SelectedIndex != _originalTrangThaiIndex;
        }
    }
}