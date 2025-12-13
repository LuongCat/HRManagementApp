using System;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.DAL;
using HRManagementApp.models;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views
{
    public partial class AttendanceManagementView : UserControl
    {
        private ChamCongRepository _repo;
        private SystemLogRepository _logRepo; // 1. Khai báo Repo ghi log

        public AttendanceManagementView()
        {
            InitializeComponent();
            _repo = new ChamCongRepository();
            _logRepo = new SystemLogRepository(); // 2. Khởi tạo

            // Set mặc định tháng hiện tại
            txtMonth.Text = DateTime.Now.Month.ToString();
            txtYear.Text = DateTime.Now.Year.ToString();

            // Console.WriteLine(UserSession.HoTen);
            LoadData();
        }

        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearchMaNV.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã nhân viên để xem dữ liệu.", "Thông báo");
                dgAttendance.ItemsSource = null;
            }

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearchMaNV.Text) && int.TryParse(txtSearchMaNV.Text, out int maNV))
                {
                    var list = _repo.GetChamCongByMonth(maNV, int.Parse(txtMonth.Text), int.Parse(txtYear.Text));
                    dgAttendance.ItemsSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        // --- CHỨC NĂNG THÊM (CÓ GHI LOG) ---
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new AttendanceEditWindow(null); // Mode thêm mới
            if (editWindow.ShowDialog() == true)
            {
                var newItem = editWindow.ChamCongData;
                bool success = _repo.AddChamCong(newItem);

                if (success)
                {
                    MessageBox.Show("Thêm mới thành công!");

                    // --- GHI LOG ---
                    var log = new SystemLog(
                        UserSession.HoTen, // Người làm
                        "INSERT", // Hành động
                        "ChamCong", // Bảng
                        "", // Mã bản ghi (Lưu tạm Mã NV vì chưa có Mã CC)
                        $"Thêm tay chấm công ngày {newItem.Ngay:dd/MM/yyyy}" // Mô tả
                    );
                    _logRepo.AddLog(log);
                    // ----------------

                    txtSearchMaNV.Text = newItem.MaNV.ToString();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Thêm thất bại. Vui lòng kiểm tra lại.");
                }
            }
        }

        // --- CHỨC NĂNG SỬA (CÓ GHI LOG) ---
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn?.DataContext as ChamCong;

            if (item != null)
            {
                // 1. SNAPSHOT: Lưu lại giá trị CŨ trước khi mở form sửa
                // Phải lưu ra biến riêng, vì nếu editWindow sửa trực tiếp vào object 'item', ta sẽ mất giá trị cũ
                TimeSpan? oldGioVao = item.GioVao;
                TimeSpan? oldGioRa = item.GioRa;
                DateTime? oldNgay = item.Ngay;
                int? oldMaNV = item.MaNV;
                var editWindow = new AttendanceEditWindow(item);

                if (editWindow.ShowDialog() == true)
                {
                    var updatedItem = editWindow.ChamCongData;
                    bool success = _repo.UpdateChamCong(updatedItem);

                    if (success)
                    {
                        MessageBox.Show("Cập nhật thành công!");

                        // 2. TẠO MÔ TẢ CHI TIẾT THAY ĐỔI (OLD -> NEW)
                        string changeDetails = $"Sửa chấm công ngày {oldNgay:dd/MM} của nhân viên ID: {oldMaNV}: ";
                        bool hasChange = false;

                        if (oldGioVao != updatedItem.GioVao)
                        {
                            changeDetails += $"[Vào: {oldGioVao} -> {updatedItem.GioVao}] ";
                            hasChange = true;
                        }

                        if (oldGioRa != updatedItem.GioRa)
                        {
                            changeDetails += $"[Ra: {oldGioRa} -> {updatedItem.GioRa}] ";
                            hasChange = true;
                        }

                        if (!hasChange) changeDetails += "Không thay đổi giờ, chỉ lưu lại.";

                        // 3. GHI LOG
                        var log = new SystemLog(
                            UserSession.HoTen,
                            "UPDATE",
                            "ChamCong",
                            updatedItem.MaCC.ToString(),
                            changeDetails // VD: Sửa chấm công ngày 12/10: [Vào: 08:00 -> 08:30]
                        );
                        _logRepo.AddLog(log);

                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại.");
                    }
                }
            }
        }

        // --- CHỨC NĂNG XÓA (CÓ GHI LOG) ---
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
                    // 1. LƯU THÔNG TIN CẦN BACKUP TRƯỚC KHI XÓA
                    // Format chuỗi JSON hoặc text dễ đọc để biết đã xóa cái gì
                    string oldDataInfo = $"Ngày: {item.Ngay:dd/MM/yyyy} | " +
                                         $"NV: {item.MaNV} | " +
                                         $"Vào: {item.GioVao} | " +
                                         $"Ra: {item.GioRa} | " +
                                         $"Tổng giờ: {item.ThoiGianLam}";

                    bool success = _repo.DeleteChamCong(item.MaCC);
            
                    if (success)
                    {
                        MessageBox.Show("Đã xóa thành công.");

                        // 2. GHI LOG KÈM DỮ LIỆU CŨ
                        var log = new SystemLog(
                            UserSession.HoTen,
                            "DELETE",
                            "ChamCong",
                            item.MaCC.ToString(),
                            "Đã xóa bản ghi: " + oldDataInfo 
                            // Kết quả log: "Đã xóa bản ghi: Ngày: 12/10/2025 | NV: 10 | Vào: 08:00 | Ra: 17:00..."
                        );
                        _logRepo.AddLog(log);

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