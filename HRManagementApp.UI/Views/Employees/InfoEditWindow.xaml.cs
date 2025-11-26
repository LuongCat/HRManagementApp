using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace HRManagementApp.UI.Views
{
    public partial class InfoEditWindow : Window
    {
        private List<InfoEditItem> _originalData;
        public bool IsSaved { get; private set; } = false;
        public List<InfoEditItem> EditedData { get; private set; }

        public InfoEditWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Hiển thị window với dữ liệu cần chỉnh sửa
        /// </summary>
        public void LoadData(string title, string subtitle, List<InfoEditItem> data)
        {
            TxtTitle.Text = title;
            TxtSubtitle.Text = subtitle;

            // Lưu bản sao gốc để có thể hủy thay đổi
            _originalData = data.Select(item => new InfoEditItem
            {
                Label = item.Label,
                Value = item.Value,
                IsMultiline = item.IsMultiline,
                PropertyName = item.PropertyName
            }).ToList();

            EditedData = data;
            InfoItemsControl.ItemsSource = EditedData;
        }

        /// <summary>
        /// Nút đóng (X) - Hỏi xác nhận nếu có thay đổi
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (HasChanges())
            {
                var result = MessageBox.Show(
                    "Bạn có thay đổi chưa được lưu. Bạn có chắc muốn đóng?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    IsSaved = false;
                    this.Close();
                }
            }
            else
            {
                IsSaved = false;
                this.Close();
            }
        }

        /// <summary>
        /// Nút Hủy - Khôi phục dữ liệu gốc và đóng
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (HasChanges())
            {
                var result = MessageBox.Show(
                    "Bạn có chắc muốn hủy các thay đổi?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    RestoreOriginalData();
                    IsSaved = false;
                    this.Close();
                }
            }
            else
            {
                IsSaved = false;
                this.Close();
            }
        }

        /// <summary>
        /// Nút Lưu - Validate và lưu dữ liệu
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate dữ liệu
            var validationErrors = ValidateData();
            if (validationErrors.Any())
            {
                MessageBox.Show(
                    "Vui lòng kiểm tra lại:\n\n" + string.Join("\n", validationErrors),
                    "Lỗi nhập liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            // Xác nhận lưu
            var result = MessageBox.Show(
                "Bạn có chắc muốn lưu các thay đổi?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IsSaved = true;
                this.Close();
            }
        }

        /// <summary>
        /// Kiểm tra xem có thay đổi nào không
        /// </summary>
        private bool HasChanges()
        {
            if (_originalData == null || EditedData == null)
                return false;

            for (int i = 0; i < _originalData.Count; i++)
            {
                if (_originalData[i].Value != EditedData[i].Value)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Khôi phục dữ liệu về trạng thái ban đầu
        /// </summary>
        private void RestoreOriginalData()
        {
            if (_originalData == null || EditedData == null)
                return;

            for (int i = 0; i < _originalData.Count; i++)
            {
                EditedData[i].Value = _originalData[i].Value;
            }
        }

        /// <summary>
        /// Validate dữ liệu nhập vào
        /// </summary>
        private List<string> ValidateData()
        {
            var errors = new List<string>();

            foreach (var item in EditedData)
            {
                // Kiểm tra trường bắt buộc
                if (string.IsNullOrWhiteSpace(item.Value))
                {
                    errors.Add($"• {item.Label} không được để trống");
                }

                // Thêm các validation khác tùy theo yêu cầu
                // Ví dụ: kiểm tra email, số điện thoại, ngày tháng...
                if (item.PropertyName == "Email" && !string.IsNullOrWhiteSpace(item.Value))
                {
                    if (!IsValidEmail(item.Value))
                    {
                        errors.Add($"• {item.Label} không đúng định dạng");
                    }
                }

                if (item.PropertyName == "PhoneNumber" && !string.IsNullOrWhiteSpace(item.Value))
                {
                    if (!IsValidPhoneNumber(item.Value))
                    {
                        errors.Add($"• {item.Label} không đúng định dạng (10-11 số)");
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// Kiểm tra email hợp lệ
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra số điện thoại hợp lệ
        /// </summary>
        private bool IsValidPhoneNumber(string phone)
        {
            // Chỉ chấp nhận số, có thể có dấu + ở đầu
            string cleanPhone = phone.Replace(" ", "").Replace("-", "");
            if (cleanPhone.StartsWith("+84"))
                cleanPhone = "0" + cleanPhone.Substring(3);

            return cleanPhone.Length >= 10 && cleanPhone.Length <= 11 && cleanPhone.All(char.IsDigit);
        }
    }

    /// <summary>
    /// Class đại diện cho một trường thông tin có thể chỉnh sửa
    /// </summary>
    public class InfoEditItem : INotifyPropertyChanged
    {
        private string _value;

        /// <summary>
        /// Nhãn hiển thị (VD: "Họ và tên:", "Email:")
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Giá trị có thể chỉnh sửa
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        /// <summary>
        /// Cho phép nhập nhiều dòng hay không
        /// </summary>
        public bool IsMultiline { get; set; } = false;

        /// <summary>
        /// Tên thuộc tính trong model (để mapping về object gốc)
        /// VD: "FullName", "Email", "PhoneNumber"
        /// </summary>
        public string PropertyName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}