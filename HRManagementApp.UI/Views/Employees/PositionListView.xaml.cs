using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    /// <summary>
    /// View hiển thị danh sách chức vụ
    /// </summary>
    public partial class PositionListView : UserControl
    {
        private readonly ChucVuService _chucVuService;
        private ObservableCollection<ChucVuViewModel> _positions;

        public PositionListView()
        {
            InitializeComponent();
            _chucVuService = new ChucVuService();
            LoadPositions();
        }

        /// <summary>
        /// Load danh sách chức vụ từ database
        /// </summary>
        private void LoadPositions()
        {
            try
            {
                var chucVus = _chucVuService.GetAllChucVu();

                if (chucVus == null || chucVus.Count == 0)
                {
                    PositionDataGrid.ItemsSource = null;
                    MessageBox.Show("Không có dữ liệu chức vụ!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Chuyển đổi sang ViewModel để hiển thị
                _positions = new ObservableCollection<ChucVuViewModel>(
                    chucVus.Select(cv => new ChucVuViewModel
                    {
                        MaChucVu = cv.MaCV,
                        TenChucVu = cv.TenCV,
                        PhuCapDisplay = FormatCurrency(cv.PhuCap),
                        TienPhuCapKiemNhiem = cv.TienPhuCapKiemNhiem,
                        LuongCB = cv.LuongCB,
                        IsActive = cv.IsActive,
                    })
                );

                PositionDataGrid.ItemsSource = _positions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách chức vụ: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Format tiền tệ
        /// </summary>
        private string FormatCurrency(decimal? amount)
        {
            if (!amount.HasValue || amount.Value == 0)
                return "0";

            return amount.Value.ToString("N0");
        }


        /// <summary>
        /// Thêm chức vụ mới
        /// </summary>
        private void BtnThemCV_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Mở window/dialog thêm chức vụ mới
            var window = new AddPositionWindow();
            window.ShowDialog();

            LoadPositions();
        }

        /// <summary>
        /// Chỉnh sửa chức vụ
        /// </summary>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string maChucVu = button?.Tag?.ToString();

            if (!string.IsNullOrEmpty(maChucVu))
            {
                var position = _positions.FirstOrDefault(p => p.MaChucVu == int.Parse(maChucVu));
                if (position != null)
                {
                    // TODO: Mở window/dialog chỉnh sửa chức vụ
                    MessageBox.Show(
                        $"Chỉnh sửa chức vụ: {position.TenChucVu}\n\n" +
                        $"Mã chức vụ: {position.MaChucVu}\n" +
                        $"Phụ cấp: {position.PhuCapDisplay} VNĐ",
                        "Chỉnh sửa",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Sau khi sửa thành công, gọi LoadPositions() để refresh
                }
            }
        }

        /// <summary>
        /// Xóa chức vụ
        /// </summary>
        private void BtnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string maChucVu = button?.Tag?.ToString();

            if (!string.IsNullOrEmpty(maChucVu))
            {
                var position = _positions.FirstOrDefault(p => p.MaChucVu == int.Parse(maChucVu));
                if (position == null)
                {
                    MessageBox.Show("Không tìm thấy chức vụ!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn thay đổi trạng thái chức vụ?\n\n" +
                    $"Mã chức vụ: {position.MaChucVu}\n" +
                    $"Tên chức vụ: {position.TenChucVu}\n\n",
                    "Xác nhận thay đổi",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = _chucVuService.ChangeStatus(int.Parse(maChucVu), position.IsActive);

                        if (success)
                        {
                            MessageBox.Show("Thay đổi trạng thái chức vụ thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadPositions();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi thay đổi trạng thái chức vụ: {ex.Message}", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ViewModel cho hiển thị chức vụ
    /// </summary>
    public class ChucVuViewModel
    {
        public int MaChucVu { get; set; }
        public string TenChucVu { get; set; }
        public string PhuCapDisplay { get; set; }
        public string MoTa { get; set; }
        public decimal? TienPhuCapKiemNhiem { get; set; }
        public decimal? LuongCB { get; set; }
        public string IsActive { get; set; }
    }
}