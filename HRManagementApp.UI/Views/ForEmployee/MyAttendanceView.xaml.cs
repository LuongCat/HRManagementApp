using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using HRManagementApp.DAL;
using HRManagementApp.models;
using HRManagementApp.BLL;
namespace HRManagementApp.UI.Views
{
    public partial class MyAttendanceView : UserControl
    {
        private ChamCongService _repo = new ChamCongService();
        private int _currentMonth = DateTime.Now.Month;
        private int _currentYear = DateTime.Now.Year;
        private int _maNV = 1; // Giả sử mã nhân viên là 1, bạn có thể truyền vào Constructor
        private ChamCong _todayRecord;
        public MyAttendanceView(int maNV = 1)
        {
            InitializeComponent();
            _maNV = maNV;
            
            // Set ngày giờ hiện tại
            txtCurrentDate.Text = DateTime.Now.ToString("dddd, dd MMMM, yyyy");
            txtRealtimeDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            
            // Timer cho đồng hồ chạy
            StartClock();
            CheckTodayStatus();
            
            // Load dữ liệu
            LoadCalendar(_currentMonth, _currentYear);
            LoadStatistics();
        }
        
        private void StartClock()
        {
            // Code đồng hồ của bạn...
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => { 
                txtRealtimeClock.Text = DateTime.Now.ToString("HH:mm:ss"); 
            };
            timer.Start();
            txtRealtimeDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        
        // Hàm kiểm tra trạng thái để đổi màu nút
        private void CheckTodayStatus()
        {
            // Lấy dữ liệu hôm nay từ DB
            _todayRecord = _repo.GetChamCongToday(_maNV);

            if (_todayRecord == null)
            {
                // TH1: Chưa chấm công -> Nút Xanh (Mặc định)
                SetButtonState("CheckIn");
            }
            else if (_todayRecord.GioVao != null && _todayRecord.GioRa == null)
            {
                // TH2: Đã vào, Chưa ra -> Nút Đỏ (Đang làm)
                SetButtonState("Working");
            }
            else
            {
                // TH3: Đã vào và Đã ra -> Nút Xám (Hoàn thành)
                SetButtonState("Done");
            }
        }
        
        private void SetButtonState(string state)
        {
            if (state == "CheckIn")
            {
                btnCheckIn.Content = "➜ Chấm Công Vào";
                btnCheckIn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4F46E5")); // Xanh
                txtStatusHint.Text = "Nhấn nút để bắt đầu ngày làm việc";
                btnCheckIn.IsEnabled = true;
            }
            else if (state == "Working")
            {
                btnCheckIn.Content = "⚠ Đang làm (Bấm để về)";
                btnCheckIn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")); // Đỏ
                // Animation nhấp nháy nếu muốn (Optional)
                txtStatusHint.Text = "Bạn đang trong ca làm việc...";
                btnCheckIn.IsEnabled = true;
            }
            else if (state == "Done")
            {
                btnCheckIn.Content = "✔ Đã hoàn thành";
                btnCheckIn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")); // Xanh lá
                txtStatusHint.Text = "Hẹn gặp lại bạn vào ngày mai!";
                btnCheckIn.IsEnabled = false; // Khóa nút lại
            }
        }

        // Sự kiện khi bấm nút
        private void BtnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (_todayRecord == null)
            {
                // --- LOGIC CHECK IN ---
                bool success = _repo.CheckIn(_maNV);
                if (success)
                {
                    MessageBox.Show("Chấm công VÀO thành công!", "Thông báo");
                    CheckTodayStatus(); // Load lại trạng thái để đổi màu nút
                    // Reload biểu đồ/thống kê nếu cần
                }
            }
            else if (_todayRecord.GioRa == null)
            {
                // --- LOGIC CHECK OUT ---
                // Xác nhận trước khi về
                var result = MessageBox.Show("Bạn có chắc chắn muốn kết thúc ca làm việc?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool success = _repo.CheckOut(_todayRecord.MaCC); // Dùng MaCC đã lưu lúc load
                    if (success)
                    {
                        MessageBox.Show("Chấm công RA thành công!", "Thông báo");
                        CheckTodayStatus(); // Đổi sang màu xanh lá
                    }
                }
            }
        }
        
        

        private void LoadStatistics()
        {
            // Lấy thống kê từ repo (dùng hàm có sẵn của bạn)
            var stats = _repo.GetChamCongStatistics(_maNV, _currentMonth, _currentYear);
            
            // Cập nhật UI
            txtOnTime.Text = (stats.SoNgayDiLam - stats.DiemDiTre).ToString(); // Giả sử tính đúng giờ đơn giản
            txtLate.Text = stats.DiemDiTre.ToString(); 
            txtWorkDays.Text = $"{stats.SoNgayDiLam} ngày làm việc";
            
            // Tính tổng giờ làm (cần query thêm list chi tiết để sum)
            var list = _repo.GetChamCongByMonth(_maNV, _currentMonth, _currentYear);
            double totalHours = list.Where(x => x.ThoiGianLam.HasValue).Sum(x => x.ThoiGianLam.Value.TotalHours);
            
            txtTotalHours.Text = $"{totalHours:N1}h";
            
            if (stats.SoNgayDiLam > 0)
                txtAvgHours.Text = $"{totalHours / stats.SoNgayDiLam:N1}h";
            else
                txtAvgHours.Text = "0h";
        }

        private void LoadCalendar(int month, int year)
        {
            CalendarGrid.Children.Clear();
            txtCalendarMonth.Text = $"Tháng {month}, {year}";

            // Lấy dữ liệu chấm công tháng này để tô màu
            var listChamCong = _repo.GetChamCongByMonth(_maNV, month, year);
            // Tạo HashSet chứa các ngày đã đi làm (để tra cứu cho nhanh)
            HashSet<int> daysWorked = new HashSet<int>();
            foreach(var item in listChamCong)
            {
                if(item.Ngay.HasValue) daysWorked.Add(item.Ngay.Value.Day);
            }

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            
            // DayOfWeek: Sunday = 0, Monday = 1...
            int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek; 

            // 1. Vẽ các ô trống trước ngày mùng 1
            for (int i = 0; i < startDayOfWeek; i++)
            {
                CalendarGrid.Children.Add(new Border()); // Ô trống
            }

            // 2. Vẽ các ngày trong tháng
            for (int day = 1; day <= daysInMonth; day++)
            {
                Button btnDay = new Button();
                btnDay.Content = day.ToString();
                btnDay.Height = 50;
                btnDay.Margin = new Thickness(3);
                btnDay.BorderThickness = new Thickness(0);
                
                // Style bo góc cho Button
                ControlTemplate template = new ControlTemplate(typeof(Button));
                FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
                borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
                borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
                FrameworkElementFactory contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
                borderFactory.AppendChild(contentFactory);
                template.VisualTree = borderFactory;
                btnDay.Template = template;

                // LOGIC TÔ MÀU
                if (daysWorked.Contains(day))
                {
                    // Đã chấm công -> Màu Xanh lá
                    btnDay.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E")); 
                    btnDay.Foreground = Brushes.White;
                    btnDay.FontWeight = FontWeights.Bold;
                    btnDay.Cursor = System.Windows.Input.Cursors.Hand;
                    
                    // Gắn sự kiện Click để mở chi tiết
                    int selectedDay = day; // Capture variable
                    btnDay.Click += (s, e) => OpenDetailWindow(selectedDay, month, year);
                }
                else
                {
                    // Chưa chấm -> Màu Xám
                    btnDay.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB")); 
                    btnDay.Foreground = Brushes.Gray;
                }

                CalendarGrid.Children.Add(btnDay);
            }
        }

        private void OpenDetailWindow(int day, int month, int year)
        {
            DateTime selectedDate = new DateTime(year, month, day);
            var detailWin = new AttendanceDetailWindow2(_maNV, selectedDate);
            detailWin.ShowDialog();
        }

        private void PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth--;
            if (_currentMonth < 1) { _currentMonth = 12; _currentYear--; }
            LoadCalendar(_currentMonth, _currentYear);
            LoadStatistics();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth++;
            if (_currentMonth > 12) { _currentMonth = 1; _currentYear++; }
            LoadCalendar(_currentMonth, _currentYear);
            LoadStatistics();
        }
    }
}