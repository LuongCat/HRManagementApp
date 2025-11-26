using System.Collections.Generic;
using System.Windows.Controls;
using HRManagementApp.models;
using HRManagementApp.BLL;
namespace HRManagementApp.UI.Views
{
    public partial class AttendanceView : UserControl
    {
        private readonly ChamCongService _chamCongService;
        private int Day = 1;
        private int Month = 9;
        private int Year = 2025 ; // đang config cứng để test
        public AttendanceView()
        {
            _chamCongService = new ChamCongService();
            
            InitializeComponent();
            LoadDummyData();
        }

        private void LoadDummyData()
        {

            var list = _chamCongService.GetAllAttendancByMonthYear( Day,Month, Year);
            var listAttendanceItems = new List<AttendanceItem>();
            foreach (var item in list)
            {
                var attendanceItem = new AttendanceItem
                {
                    MaNV = item.MaNV.ToString(),
                    // thiếu phòng ban và HoTen
                    GioVao = item.GioVao.ToString(),
                    GioRa = item.GioRa.ToString(),
                    SoGio = item.ThoiGianLam.ToString(),
                };
                listAttendanceItems.Add(attendanceItem);
            }
            dgAttendance.ItemsSource = list;
        }
    }
    
    public class AttendanceItem
    {
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public string PhongBan { get; set; }
        public string GioVao { get; set; }
        public string GioRa { get; set; }
        public string SoGio { get; set; }
        public string TrangThai { get; set; } // "Đúng giờ", "Đi muộn", "Đang làm", "Vắng", "Nửa ngày"
    }
}