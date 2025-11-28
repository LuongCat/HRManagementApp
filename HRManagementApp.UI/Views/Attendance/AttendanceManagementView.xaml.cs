using System;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.DAL;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class AttendanceManagementView : UserControl
    {
        private ChamCongRepository _repo;

        public AttendanceManagementView()
        {
            InitializeComponent();
            _repo = new ChamCongRepository();
            
            // Set mặc định tháng hiện tại
            txtMonth.Text = DateTime.Now.Month.ToString();
            txtYear.Text = DateTime.Now.Year.ToString();
            
            LoadData();
        }

        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Logic tìm kiếm: Nếu có nhập Mã NV thì ưu tiên tìm theo Mã NV
                if (!string.IsNullOrEmpty(txtSearchMaNV.Text) && int.TryParse(txtSearchMaNV.Text, out int maNV))
                {
                    // Lọc theo Mã NV và Tháng Năm
                    var list = _repo.GetChamCongByMonth(maNV, int.Parse(txtMonth.Text), int.Parse(txtYear.Text));
                    dgAttendance.ItemsSource = list;
                }
                else
                {
                    // Nếu muốn load tất cả nhân viên trong tháng (Repo cần hỗ trợ hàm GetAllByMonth)
                    // Hiện tại repo của bạn chỉ có GetByMonth(maNV), nên tạm thời báo user nhập MaNV
                    MessageBox.Show("Vui lòng nhập Mã nhân viên để xem dữ liệu.", "Thông báo");
                    dgAttendance.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        // --- CHỨC NĂNG THÊM ---
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new AttendanceEditWindow(null); // Mode thêm mới
            if (editWindow.ShowDialog() == true)
            {
                var newItem = editWindow.ChamCongData;
                bool success = _repo.AddChamCong(newItem); // Gọi hàm Add trong Repo
                
                if (success)
                {
                    MessageBox.Show("Thêm mới thành công!");
                    txtSearchMaNV.Text = newItem.MaNV.ToString(); // Auto fill để reload
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Thêm thất bại. Vui lòng kiểm tra lại.");
                }
            }
        }

        // --- CHỨC NĂNG SỬA ---
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn?.DataContext as ChamCong;

            if (item != null)
            {
                // Tạo window với dữ liệu cũ
                var editWindow = new AttendanceEditWindow(item);
                
                if (editWindow.ShowDialog() == true)
                {
                    var updatedItem = editWindow.ChamCongData;
                    bool success = _repo.UpdateChamCong(updatedItem); // Gọi hàm Update trong Repo
                    
                    if (success)
                    {
                        MessageBox.Show("Cập nhật thành công!");
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại.");
                    }
                }
            }
        }

        // --- CHỨC NĂNG XÓA ---
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn?.DataContext as ChamCong;

            if (item != null)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn xóa bản ghi ngày {item.Ngay:dd/MM} của NV {item.MaNV}?", 
                                             "Xác nhận xóa", 
                                             MessageBoxButton.YesNo, 
                                             MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    bool success = _repo.DeleteChamCong(item.MaCC); // Gọi hàm Delete trong Repo
                    if (success)
                    {
                        MessageBox.Show("Đã xóa thành công.");
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại.");
                    }
                }
            }
        }
    }
}