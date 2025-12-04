using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HRManagementApp.BLL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views.Leave
{
    public partial class LeaveManagementView : UserControl
    {
        private readonly DonTuService _service;
        private List<DonTu> _allRequests;

        public LeaveManagementView()
        {
            InitializeComponent();
            _service = new DonTuService();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load danh sách đơn
                _allRequests = _service.GetAllDonTu();
                
                // Mặc định load vào cả 2 bảng (nhưng chỉ hiện DgRequests)
                DgRequests.ItemsSource = _allRequests;
                DgPendingRequests.ItemsSource = _allRequests.Where(x => x.TrangThai == "Chưa duyệt").ToList();
                
                DgLeaveTypes.ItemsSource = _service.GetLoaiDonList();

                if (_allRequests != null)
                {
                    TxtTotal.Text = _allRequests.Count.ToString();
                    TxtPending.Text = _allRequests.Count(x => x.TrangThai == "Chưa duyệt").ToString();
                    TxtApproved.Text = _allRequests.Count(x => x.TrangThai == "Đã duyệt").ToString();
                    TxtRejected.Text = _allRequests.Count(x => x.TrangThai == "Từ chối").ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            
            string tag = btn.Tag?.ToString();

            ResetTabStyles();
            btn.Background = Brushes.White;
            btn.Foreground = Brushes.Black;

            // XỬ LÝ HIỂN THỊ BẢNG VÀ NÚT BẤM
            if (tag == "Types") 
            {
                // Tab Loại đơn
                TxtListTitle.Text = "Danh mục loại đơn";
                DgRequests.Visibility = Visibility.Collapsed;
                DgPendingRequests.Visibility = Visibility.Collapsed;
                DgLeaveTypes.Visibility = Visibility.Visible;

                BtnAction.Content = "+ Thêm loại đơn";
                BtnAction.Visibility = Visibility.Visible; 
            }
            else if (tag == "Pending") 
            {
                // Tab Chờ duyệt -> HIỆN BẢNG DgPendingRequests
                TxtListTitle.Text = "Đơn chờ duyệt";
                DgRequests.Visibility = Visibility.Collapsed;
                DgPendingRequests.Visibility = Visibility.Visible; // Bảng riêng
                DgLeaveTypes.Visibility = Visibility.Collapsed;
                
                // Reload data mới nhất
                DgPendingRequests.ItemsSource = _allRequests.Where(x => x.TrangThai == "Chưa duyệt").ToList();

                BtnAction.Visibility = Visibility.Collapsed; // Ẩn nút
            }
            else 
            {
                // Tab Tất cả
                TxtListTitle.Text = "Danh sách đơn từ";
                DgRequests.Visibility = Visibility.Visible;
                DgPendingRequests.Visibility = Visibility.Collapsed;
                DgLeaveTypes.Visibility = Visibility.Collapsed;
                
                DgRequests.ItemsSource = _allRequests;

                BtnAction.Content = "+ Tạo đơn mới";
                BtnAction.Visibility = Visibility.Visible;
            }
        }

        private void ResetTabStyles()
        {
            var grayBrush = (Brush)new BrushConverter().ConvertFromString("#6B7280");
            
            BtnTabAll.Background = Brushes.Transparent;
            BtnTabAll.Foreground = grayBrush;

            BtnTabPending.Background = Brushes.Transparent;
            BtnTabPending.Foreground = grayBrush;

            BtnTabTypes.Background = Brushes.Transparent;
            BtnTabTypes.Foreground = grayBrush;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            string content = BtnAction.Content.ToString();

            if (content.Contains("Thêm loại đơn"))
            {
                // ✅ Mở cửa sổ thêm loại đơn
                var window = new AddLeaveTypeWindow();
                if (window.ShowDialog() == true)
                {
                    LoadData(); // Refresh lại danh sách loại đơn
                }
            }
            else
            {
                var window = new AddLeaveWindow();
                if (window.ShowDialog() == true)
                {
                    LoadData(); 
                }
            }
        }

        // ==========================================
        // XỬ LÝ SỬA LOẠI ĐƠN
        // ==========================================
        private void BtnEditType_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is LoaiDon selectedItem)
            {
                // Mở window nhập liệu
                var window = new AddLeaveTypeWindow();
                
                // Chuyển sang chế độ Edit và truyền dữ liệu sang
                window.LoadDataForEdit(selectedItem);
                
                if (window.ShowDialog() == true)
                {
                    LoadData(); // Refresh lại danh sách
                }
            }
        }

        // ==========================================
        // XỬ LÝ XÓA LOẠI ĐƠN
        // ==========================================
        private void BtnDeleteType_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag.ToString(), out int maLoai))
            {
                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa loại đơn này?\nLưu ý: Các đơn từ cũ thuộc loại này sẽ bị mất thông tin loại đơn.", 
                    "Xác nhận xóa", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool deleted = _service.DeleteLeaveType(maLoai);
                        if (deleted)
                        {
                            MessageBox.Show("Xóa thành công!", "Thông báo");
                            LoadData(); // Refresh lại danh sách
                        }
                        else
                        {
                            MessageBox.Show("Xóa thất bại!", "Lỗi");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            }
        }

        private void BtnApprove_Click(object sender, RoutedEventArgs e)
        {
            HandleAction(sender, "Duyệt", _service.ApproveRequest);
        }

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            HandleAction(sender, "Từ chối", _service.RejectRequest);
        }

        // Hàm xử lý chung cho duyệt/từ chối
        private void HandleAction(object sender, string actionName, Func<int, string, bool> action)
        {
            if (sender is FrameworkElement element && int.TryParse(element.Tag.ToString(), out int maDon))
            {
                if (MessageBox.Show($"Bạn có chắc muốn {actionName} đơn này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (action(maDon, "Admin"))
                    {
                        LoadData();
                        // Nếu đang ở tab Pending thì cần refresh lại view ngay
                        if (DgPendingRequests.Visibility == Visibility.Visible)
                        {
                            DgPendingRequests.ItemsSource = _allRequests.Where(x => x.TrangThai == "Chưa duyệt").ToList();
                        }
                    }
                }
            }
        }
    }
}