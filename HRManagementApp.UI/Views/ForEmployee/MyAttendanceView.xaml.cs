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
        private int _maNV; // Sẽ lấy từ UserSession
        private ChamCong _todayRecord;

        public MyAttendanceView(int maNV = 0) // Tham số optional
        {
            InitializeComponent();
            
            // --- LẤY THÔNG TIN TỪ SESSION ---
            if (UserSession.MaNV.HasValue)
            {
                _maNV = UserSession.MaNV.Value;
                // Cập nhật UI tên nhân viên
                txtEmployeeName.Text = UserSession.HoTen; 
                txtEmployeeID.Text = $"Mã NV: {_maNV} | {UserSession.TenPB ?? "Phòng ban"}";
                
                // Update Avatar chữ cái đầu
                if(!string.IsNullOrEmpty(UserSession.HoTen))
                    txtAvatarChar.Text = UserSession.HoTen.Substring(0, 1).ToUpper();
            }
            else
            {
                MessageBox.Show("Tài khoản này không liên kết với nhân viên nào!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                // Xử lý disable các chức năng
                btnCheckIn.IsEnabled = false;
                return;
            }

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
            _todayRecord = _repo.GetChamCongToday(_maNV);

            if (_todayRecord == null)
            {
                SetButtonState("CheckIn");
            }
            else if (_todayRecord.GioVao != null && _todayRecord.GioRa == null)
            {
                SetButtonState("Working");
            }
            else
            {
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
                txtStatusHint.Text = "Bạn đang trong ca làm việc...";
                btnCheckIn.IsEnabled = true;
            }
            else if (state == "Done")
            {
                btnCheckIn.Content = "✔ Đã hoàn thành";
                btnCheckIn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")); // Xanh lá
                txtStatusHint.Text = "Hẹn gặp lại bạn vào ngày mai!";
                btnCheckIn.IsEnabled = false;
            }
        }

        private void BtnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (_todayRecord == null)
            {
                bool success = _repo.CheckIn(_maNV);
                if (success)
                {
                    MessageBox.Show("Chấm công VÀO thành công!", "Thông báo");
                    CheckTodayStatus();
                    LoadCalendar(_currentMonth, _currentYear); // Refresh lịch
                }
            }
            else if (_todayRecord.GioRa == null)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn kết thúc ca làm việc?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool success = _repo.CheckOut(_todayRecord.MaCC);
                    if (success)
                    {
                        MessageBox.Show("Chấm công RA thành công!", "Thông báo");
                        CheckTodayStatus();
                        LoadCalendar(_currentMonth, _currentYear); // Refresh lịch
                    }
                }
            }
        }
        
        private void LoadStatistics()
        {
            var stats = _repo.GetChamCongStatistics(_maNV, _currentMonth, _currentYear);
            
            txtOnTime.Text = (stats.SoNgayDiLam - stats.DiemDiTre).ToString(); 
            txtLate.Text = stats.DiemDiTre.ToString(); 
            txtWorkDays.Text = $"{stats.SoNgayDiLam} ngày làm việc";
            
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

            var listChamCong = _repo.GetChamCongByMonth(_maNV, month, year);
            HashSet<int> daysWorked = new HashSet<int>();
            foreach(var item in listChamCong)
            {
                if(item.Ngay.HasValue) daysWorked.Add(item.Ngay.Value.Day);
            }

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek; 

            for (int i = 0; i < startDayOfWeek; i++)
            {
                CalendarGrid.Children.Add(new Border());
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                Button btnDay = new Button();
                btnDay.Content = day.ToString();
                btnDay.Height = 50;
                btnDay.Margin = new Thickness(3);
                btnDay.BorderThickness = new Thickness(0);
                
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

                if (daysWorked.Contains(day))
                {
                    btnDay.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E")); 
                    btnDay.Foreground = Brushes.White;
                    btnDay.FontWeight = FontWeights.Bold;
                    btnDay.Cursor = System.Windows.Input.Cursors.Hand;
                    
                    int selectedDay = day;
                    // Gọi popup chi tiết, truyền MaNV và ngày
                    btnDay.Click += (s, e) => OpenDetailWindow(selectedDay, month, year);
                }
                else
                {
                    btnDay.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB")); 
                    btnDay.Foreground = Brushes.Gray;
                }

                CalendarGrid.Children.Add(btnDay);
            }
        }

        private void OpenDetailWindow(int day, int month, int year)
        {
            DateTime selectedDate = new DateTime(year, month, day);
            // Truyền MaNV vào AttendanceDetailWindow2
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