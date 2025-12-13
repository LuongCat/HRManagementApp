using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views
{
    public partial class AllowanceManagementWindow : Window
    {
        private NhanVien _targetEmployee;

        // TODO: Inject Service phụ cấp
        private PhuCapService _phuCapService;
        private LuongService _luongService;

        public AllowanceManagementWindow(NhanVien employee)
        {
            InitializeComponent();
            _targetEmployee = employee;
            _phuCapService = new PhuCapService();
            _luongService = new LuongService();
            // Hiển thị info
            TxtEmployeeName.Text = $"{_targetEmployee.HoTen} (Mã: {_targetEmployee.MaNV})";

            // Set mặc định ngày bắt đầu là hôm nay
            DpTuNgay.SelectedDate = DateTime.Now;

            LoadData();

            DateTime prev = DateTime.Now.AddMonths(-1);

            int month = prev.Month;
            int year = prev.Year;
            LoadRewardEvaluation(month: month, year: year);
        }

        private void LoadData()
        {
            // TODO: Gọi Service lấy danh sách từ DB
            // var list = _phuCapService.GetByEmployee(_targetEmployee.MaNV);

            // Giả lập load từ object truyền vào
            if (_targetEmployee.PhuCaps == null)
                _targetEmployee.PhuCaps = new List<PhuCapNhanVien>();

            DgAllowances.ItemsSource = null;
            DgAllowances.ItemsSource = _targetEmployee.PhuCaps;
        }

        // ==========================
        // UI EVENTS
        // ==========================
        private void DgAllowances_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgAllowances.SelectedItem is PhuCapNhanVien selected)
            {
                TxtTenPhuCap.Text = selected.TenPhuCap;
                TxtSoTien.Text = selected.SoTien.ToString("N0"); // Format số nguyên có dấu phẩy
                DpTuNgay.SelectedDate = selected.ApDungTuNgay;
                DpDenNgay.SelectedDate = selected.ApDungDenNgay;

                // Chế độ Edit
                BtnAdd.IsEnabled = false;
                BtnEdit.IsEnabled = true;
                BtnDelete.IsEnabled = true;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            DgAllowances.SelectedItem = null;
            TxtTenPhuCap.Text = string.Empty;
            TxtSoTien.Text = "0";
            DpTuNgay.SelectedDate = DateTime.Now;
            DpDenNgay.SelectedDate = null;

            BtnAdd.IsEnabled = true;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        // ==========================
        // CRUD ACTIONS
        // ==========================










        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var newItem = new PhuCapNhanVien
            {
                // ID = ... (DB tự sinh),
                MaNV = _targetEmployee.MaNV,
                TenPhuCap = TxtTenPhuCap.Text.Trim(),
                SoTien = decimal.Parse(TxtSoTien.Text),
                ApDungTuNgay = DpTuNgay.SelectedDate.Value,
                ApDungDenNgay = DpDenNgay.SelectedDate
            };

            // TODO: Service Call ->
            _phuCapService.AddPhuCap(newItem);
            // Giả lập
            _targetEmployee.PhuCaps.Add(newItem);

            // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
            bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, newItem.ApDungTuNgay);

            if (isUnlocked)
            {
                MessageBox.Show($"Thêm thành công! Bảng lương tháng {newItem.ApDungTuNgay:MM/yyyy} đã được mở chốt để tính lại.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Chỉ thông báo thêm thành công, không nhắc gì tới lương vì chưa có bảng lương
                MessageBox.Show("Thêm phụ cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            LoadData();
            BtnClear_Click(null, null);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgAllowances.SelectedItem is not PhuCapNhanVien selected) return;
            if (!ValidateInput()) return;

            // Lưu lại ngày cũ trước khi sửa để mở chốt cả tháng cũ (nếu user đổi ngày áp dụng sang tháng khác)
            DateTime oldDate = selected.ApDungTuNgay;

            // Update Value
            selected.TenPhuCap = TxtTenPhuCap.Text.Trim();
            selected.SoTien = decimal.Parse(TxtSoTien.Text);
            selected.ApDungTuNgay = DpTuNgay.SelectedDate.Value;
            selected.ApDungDenNgay = DpDenNgay.SelectedDate;

            // TODO: Service Call ->
            _phuCapService.UpdatePhuCap(selected);

            // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
            bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, selected.ApDungTuNgay);

            if (isUnlocked)
            {
                MessageBox.Show($"Sửa thành công! Bảng lương tháng {selected.ApDungTuNgay:MM/yyyy} đã được mở chốt để tính lại.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Chỉ thông báo thêm thành công, không nhắc gì tới lương vì chưa có bảng lương
                MessageBox.Show("cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            LoadData();
            BtnClear_Click(null, null);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgAllowances.SelectedItem is not PhuCapNhanVien selected) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa phụ cấp '{selected.TenPhuCap}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // TODO: Service Call ->
                _phuCapService.DeletePhuCap(selected.ID);

                // Giả lập
                _targetEmployee.PhuCaps.Remove(selected);
                // === GỌI SERVICE ĐỂ MỞ CHỐT LƯƠNG ===
                bool isUnlocked = _luongService.UnLockSalary(_targetEmployee.MaNV, selected.ApDungTuNgay);

                if (isUnlocked)
                {
                    MessageBox.Show($"xóa phụ cấp thành công! Bảng lương tháng {selected.ApDungTuNgay:MM/yyyy} đã được mở chốt để tính lại.",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Chỉ thông báo thêm thành công, không nhắc gì tới lương vì chưa có bảng lương
                    MessageBox.Show("xóa phụ cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadData();
                BtnClear_Click(null, null);
            }
        }

        // ==========================
        // VALIDATION
        // ==========================
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TxtTenPhuCap.Text))
            {
                MessageBox.Show("Vui lòng nhập tên phụ cấp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtSoTien.Text, out decimal soTien) || soTien <= 0)
            {
                MessageBox.Show("Số tiền phải lớn hơn 0.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DpTuNgay.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DpDenNgay.SelectedDate != null && DpDenNgay.SelectedDate < DpTuNgay.SelectedDate)
            {
                MessageBox.Show("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnOpenRewardEvaluation_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Visible;
            DgAllowances.IsEnabled = false;
        }

        private void BtnCloseReport_Click(object sender, RoutedEventArgs e)
        {
            ReportPanel.Visibility = Visibility.Collapsed;
            DgAllowances.IsEnabled = true;
        }

        private void LoadRewardEvaluation(int month, int year)
        {
            List<RewardEvaluation> report = _phuCapService.GetRewardEvaluation(_targetEmployee.MaNV, month, year);

            TxtRewardEvaluation.Text = $"Đánh giá khen thưởng tháng ";

            DgRewardEvaluation.ItemsSource = report ?? new List<RewardEvaluation>();

            int summaryRewardPoint = report?.Sum(x => x.Diem) ?? 0;
            
            int rewardPercentRecommend = 0;

            if (summaryRewardPoint >= 350) { rewardPercentRecommend = 10; }
            else if (summaryRewardPoint >= 300) { rewardPercentRecommend = 8; }
            else if (summaryRewardPoint >= 260) { rewardPercentRecommend = 5; }
            else if (summaryRewardPoint >= 230) { rewardPercentRecommend = 2; }

            double mainSalary = Math.Round((double)(new PayrollResultService().GetPayrollResultForEmployee(_targetEmployee, month, year).luongchinh ?? 0m), 2);
            double rewardSalaryRecommend = mainSalary * rewardPercentRecommend / 100;

            TxtTongDiem.Text = $"Tổng điểm: {summaryRewardPoint}";
            TxtLuongChinh.Text = $"Lương chính (VND): {mainSalary}";
            TxtThuongPhanTram.Text = $"Đề xuất thưởng (%): {rewardPercentRecommend}";
            TxtTienThuong.Text = $"Đề xuất thưởng (VND): {rewardSalaryRecommend}";
        }
    }
}