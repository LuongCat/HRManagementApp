using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{

    /// View hiển thị danh sách chức vụ
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

  
        /// Load danh sách chức vụ từ database
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

    
        /// Format tiền tệ
        /// </summary>
        private string FormatCurrency(decimal? amount)
        {
            if (!amount.HasValue || amount.Value == 0)
                return "0";

            return amount.Value.ToString("N0");
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
                    // Mở window chỉnh sửa chức vụ
                    var editWindow = new EditPositionWindow();

                    // Load dữ liệu chức vụ vào form
                    editWindow.LoadPositionData(
                        maChucVu: position.MaChucVu,
                        tenChucVu: position.TenChucVu,
                        luongCB: position.LuongCB ?? 0,
                        phuCap: decimal.TryParse(position.PhuCapDisplay.Replace(",", ""), out decimal pc) ? pc : 0,
                        phuCapKN: position.TienPhuCapKiemNhiem,
                        trangThai: position.IsActive
                    );

                    // Hiển thị dialog và xử lý kết quả
                    if (editWindow.ShowDialog() == true && editWindow.IsSaved)
                    {
                        try
                        {
                            // Tạo đối tượng ChucVu để update
                            var chucVuUpdate = new ChucVu
                            {
                                MaCV = editWindow.MaChucVu,
                                TenCV = editWindow.TenChucVu,
                                LuongCB = editWindow.LuongCoBan,
                                PhuCap = editWindow.PhuCap,
                                TienPhuCapKiemNhiem = editWindow.PhuCapKiemNhiem,
                            };

                            // Gọi service để update
                            bool success = _chucVuService.UpdateChucVu(chucVuUpdate);

                            if (success)
                            {
                                MessageBox.Show("Cập nhật chức vụ thành công!", "Thông báo",
                                    MessageBoxButton.OK, MessageBoxImage.Information);

                                // Refresh danh sách
                                LoadPositions();
                            }
                            else
                            {
                                MessageBox.Show("Cập nhật chức vụ thất bại!", "Lỗi",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi cập nhật chức vụ: {ex.Message}", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
        
        /// Thêm chức vụ mới (Cập nhật)
        private void BtnThemCV_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddPositionWindow();

            if (addWindow.ShowDialog() == true && addWindow.IsSaved)
            {
                try
                {
                    // Tạo đối tượng ChucVu mới
                    var newChucVu = new ChucVu
                    {
                        TenCV = addWindow.TenChucVu,
                        LuongCB = addWindow.LuongCoBan,
                        PhuCap = addWindow.PhuCap,
                        TienPhuCapKiemNhiem = addWindow.PhuCapKiemNhiem,
                        IsActive = addWindow.TrangThai
                    };

                    // Gọi service để thêm mới
                    bool success = _chucVuService.InsertChucVu(newChucVu);

                    if (success)
                    {
                        MessageBox.Show("Thêm chức vụ mới thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Refresh danh sách
                        LoadPositions();
                    }
                    else
                    {
                        MessageBox.Show("Thêm chức vụ thất bại!", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm chức vụ: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        /// Thay đổi trạng thái chức vụ
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
    
    /// ViewModel cho hiển thị chức vụ
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
