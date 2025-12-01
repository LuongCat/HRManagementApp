using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL; // Namespace chứa NhanVienService
using HRManagementApp.DAL; // Namespace chứa ChamCongRepository
using HRManagementApp.models;


namespace HRManagementApp.UI.Views
{
    public partial class AttendanceSummaryView : UserControl
    {
        private NhanVienService _nvService = new NhanVienService();
        private ChamCongRepository _ccRepo = new ChamCongRepository();
        
        public AttendanceSummaryView()
        {
            InitializeComponent();
            // Load mặc định tháng hiện tại
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
            // 1. Validate input
            if (!int.TryParse(txtMonth.Text, out int m) || !int.TryParse(txtYear.Text, out int y))
            {
                MessageBox.Show("Vui lòng nhập tháng năm hợp lệ");
                return;
            }

            // 2. Lấy danh sách nhân viên
            var listNV = _nvService.GetListNhanVien(); // Giả sử hàm này trả về List<NhanVien>
            var summaryList = new List<EmployeeAttendanceSummary>();

            // 3. Duyệt từng nhân viên để tính toán thống kê
            foreach (var nv in listNV)
            {
                // Gọi hàm tính toán bạn đã viết trong ChamCongRepository
                KetQuaChamCong stat = _ccRepo.GetChamCongStatistics(nv.MaNV, m, y);

                summaryList.Add(new EmployeeAttendanceSummary
                {
                    MaNV = nv.MaNV,
                    HoTen = nv.HoTen,
                    PhongBan = nv.PhongBanDisplay, // Hoặc nv.PhongBan?.TenPB
                    TongNgayCong = stat.SoNgayDiLam,
                    DiemPhat = stat.DiemDiTre
                });
            }

            // 4. Đổ dữ liệu lên Grid
            dgSummary.ItemsSource = summaryList;
        }

        private void ViewDetail_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dòng hiện tại
            var btn = sender as Button;
            var item = btn?.DataContext as EmployeeAttendanceSummary;

            if (item != null)
            {
                int m = int.Parse(txtMonth.Text);
                int y = int.Parse(txtYear.Text);

                // Mở cửa sổ mới (Window)
                var detailWindow = new AttendanceDetailWindow(item.MaNV, item.HoTen, m, y);
                detailWindow.ShowDialog(); // ShowDialog để chặn màn hình chính (Modal)
            }
        }
    }
    
    public class EmployeeAttendanceSummary
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public string PhongBan { get; set; } // Nếu có
        
        // Kết quả tổng hợp
        public int TongNgayCong { get; set; }
        public int DiemPhat { get; set; }
    }
}