using System; // Thêm namespace System
using System.Windows;
using HRManagementApp.BLL;
using HRManagementApp.DAL;

namespace HRManagementApp.UI.Views
{
    public partial class AttendanceDetailWindow2 : Window
    {
        private ChamCongService _chamCongService = new ChamCongService();

        public AttendanceDetailWindow2(int maNV, DateTime date)
        {
            InitializeComponent();
            txtTitle.Text = $"Chi tiết chấm công ngày {date:dd/MM/yyyy}";
            
            // Lưu ý: Trong BLL tôi đã viết GetAllAttendancByMonthYear sử dụng UserSession
            // Nhưng để linh hoạt và khớp với tham số constructor, ta nên truyền maNV vào BLL hoặc BLL tự lấy
            // Ở đây gọi BLL, BLL sẽ tự lấy từ Session hoặc ta sửa BLL để nhận MaNV
            // Phương án tối ưu: Gọi DAL trực tiếp hoặc BLL passthrough cho MaNV cụ thể
            
            // Sử dụng phương thức GetByDate của DAL thông qua BLL (nếu BLL có expose) hoặc gọi Service
            // Ở đây tôi giả định bạn dùng hàm GetAllAttendancByMonthYear trong BLL tôi vừa viết (nó dùng UserSession)
            // Nếu bạn muốn xem của nhân viên khác (Admin xem), cần sửa BLL nhận tham số MaNV.
            
            var list = _chamCongService.GetAllAttendancByMonthYear(date.Day, date.Month, date.Year);
            dgDetails.ItemsSource = list;
        }
    }
}