using System.Collections.Generic;
using System.Windows.Controls;
using HRManagementApp.models;

namespace HRManagementApp.UI.Views
{
    public partial class AttendanceView : UserControl
    {
        public AttendanceView()
        {
            InitializeComponent();
            LoadDummyData();
        }

        private void LoadDummyData()
        {
            var list = new List<AttendanceItem>
            {
                new AttendanceItem { MaNV = "NV001", HoTen = "Nguyễn Văn A", PhongBan = "IT", GioVao = "08:00", GioRa = "17:30", SoGio = "8.5h", TrangThai = "Đúng giờ" },
                new AttendanceItem { MaNV = "NV002", HoTen = "Trần Thị B", PhongBan = "Sales", GioVao = "08:15", GioRa = "17:45",  SoGio = "8.5h", TrangThai = "Đi muộn" },
                new AttendanceItem { MaNV = "NV003", HoTen = "Lê Văn C", PhongBan = "Marketing", GioVao = "08:05", GioRa = "-",  SoGio = "-h", TrangThai = "Đang làm" },
                new AttendanceItem { MaNV = "NV004", HoTen = "Phạm Thị D", PhongBan = "HR", GioVao = "-", GioRa = "-",  TrangThai = "Vắng" },
                new AttendanceItem { MaNV = "NV005", HoTen = "Hoàng Văn E", PhongBan = "Finance", GioVao = "08:00", GioRa = "12:00", SoGio = "4h", TrangThai = "Nửa ngày" },
            };

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