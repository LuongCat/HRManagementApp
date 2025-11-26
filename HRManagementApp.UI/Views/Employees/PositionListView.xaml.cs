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
                        MaChucVu = $"POS{cv.MaCV:000}",
                        TenChucVu = cv.TenCV,
                        HeSoLuong = CalculateHeSoLuong(cv.LuongCB),
                        PhuCapDisplay = FormatCurrency(cv.PhuCap),
                        MoTa = GetMoTa(cv.TenCV)
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
        /// Tính hệ số lương từ lương cơ bản
        /// </summary>
        private decimal CalculateHeSoLuong(decimal? luongCB)
        {
            if (!luongCB.HasValue || luongCB.Value == 0)
                return 1.0m;

            // Giả sử lương cơ bản chuẩn là 10,000,000 VNĐ
            decimal luongChuan = 10000000m;
            return Math.Round(luongCB.Value / luongChuan, 1);
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
        /// Lấy mô tả cho chức vụ
        /// </summary>
        private string GetMoTa(string tenCV)
        {
            // Map mô tả theo tên chức vụ
            var moTaDict = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Giám đốc", "Điều hành toàn bộ công ty" },
                { "Trưởng phòng", "Quản lý phòng ban" },
                { "Phó phòng", "Hỗ trợ trưởng phòng" },
                { "Nhân viên", "Thực hiện công việc được giao" }
            };

            return moTaDict.ContainsKey(tenCV) ? moTaDict[tenCV] : "Chưa có mô tả";
        }

        /// <summary>
        /// Thêm chức vụ mới
        /// </summary>
        private void BtnThemCV_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Mở window/dialog thêm chức vụ mới
            MessageBox.Show("Mở form thêm chức vụ mới", "Thông báo", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Sau khi thêm thành công, gọi LoadPositions() để refresh
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
                var position = _positions.FirstOrDefault(p => p.MaChucVu == maChucVu);
                if (position != null)
                {
                    // TODO: Mở window/dialog chỉnh sửa chức vụ
                    MessageBox.Show(
                        $"Chỉnh sửa chức vụ: {position.TenChucVu}\n\n" +
                        $"Mã chức vụ: {position.MaChucVu}\n" +
                        $"Hệ số lương: {position.HeSoLuong}\n" +
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
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string maChucVu = button?.Tag?.ToString();
            
            if (!string.IsNullOrEmpty(maChucVu))
            {
                var position = _positions.FirstOrDefault(p => p.MaChucVu == maChucVu);
                if (position == null)
                {
                    MessageBox.Show("Không tìm thấy chức vụ!", "Lỗi", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa chức vụ?\n\n" +
                    $"Mã chức vụ: {position.MaChucVu}\n" +
                    $"Tên chức vụ: {position.TenChucVu}\n\n" +
                    $"⚠️ Cảnh báo: Xóa chức vụ có thể ảnh hưởng đến dữ liệu nhân viên!",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // TODO: Implement DeleteChucVu trong service
                        // bool success = _chucVuService.DeleteChucVu(maCV);
                        
                        // Tạm thời chỉ xóa khỏi collection
                        _positions.Remove(position);
                        MessageBox.Show("Xóa chức vụ thành công!", "Thông báo", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa chức vụ: {ex.Message}", "Lỗi", 
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
        public string MaChucVu { get; set; }
        public string TenChucVu { get; set; }
        public decimal HeSoLuong { get; set; }
        public string PhuCapDisplay { get; set; }
        public string MoTa { get; set; }
    }
}