using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HRManagementApp.BLL;
using HRManagementApp.models;


namespace HRManagementApp.UI.Views
{
    public partial class DepartmentPayrollView : UserControl
    {
        private NhanVienService _nvService;
        private PayrollResultService _payrollService;
        private PhongBanService _pbService;

        public DepartmentPayrollView()
        {
            InitializeComponent();
            _nvService = new NhanVienService();
            _payrollService = new PayrollResultService();
            _pbService = new PhongBanService();

            txtMonth.Text = DateTime.Now.Month.ToString();
            txtYear.Text = DateTime.Now.Year.ToString();

            // 1. Load danh sách phòng ban vào ComboBox
            LoadDepartments();
        }

        private void LoadDepartments()
        {
            try
            {
                var listPB = _pbService.GetListPhongBan();
                
                // Thêm mục "Tất cả phòng ban" vào đầu danh sách
                // MaPB = 0 hoặc -1 để đánh dấu là chọn tất cả
                listPB.Insert(0, new PhongBan { MaPB = 0, TenPB = "--- Tất cả phòng ban ---" });

                cboPhongBan.ItemsSource = listPB;
                cboPhongBan.SelectedIndex = 0; // Mặc định chọn tất cả
            }
            catch(Exception ex) 
            {
                MessageBox.Show("Lỗi load phòng ban: " + ex.Message);
            }
        }

        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(out int month, out int year)) return;
            LoadPayrollData(month, year);
        }

        private void LoadPayrollData(int month, int year)
        {
            try
            {
                List<NhanVien> listNhanVien = _nvService.GetListNhanVien();
                if (listNhanVien == null || listNhanVien.Count == 0) return;

                List<DepartmentPayrollItem> fullList = new List<DepartmentPayrollItem>();

                // 2. Tính toán lương (Logic cũ)
                foreach (var nv in listNhanVien)
                {
                    PayrollResult result = _payrollService.GetPayrollResultForEmployee(nv, month, year);

                    string tenPb = "Chưa phân bổ";
                    int maPb = 0; 
                    if (nv.PhongBan != null && nv.PhongBan.Count > 0)
                    {
                        tenPb = nv.PhongBan[0].TenPB;
                        maPb = nv.PhongBan[0].MaPB; // Lưu lại MaPB để lọc
                    }

                    // Map sang DTO (Thêm property MaPB ẩn để lọc nếu DTO chưa có)
                    fullList.Add(new DepartmentPayrollItem
                    {
                        MaNV = result.maNV,
                        TenNV = result.TenNV,
                        TenPhongBan = tenPb,
                        // Bạn có thể cần thêm prop MaPB vào DTO nếu muốn lọc chính xác tuyệt đối,
                        // nhưng ở đây ta lọc dựa trên logic gán MaPB lúc loop này.
                        
                        LuongCoBan = result.LuongCoBan ?? 0,
                        TongNgayCong = result.TongNgayCong ?? 0,
                        TongPhuCap = result.TongPhuCap,
                        TongKhauTru = result.TongKhauTru,
                        TongThue = result.TongThue,
                        LuongThucNhan = result.LuongThucNhan ?? 0,
                        TrangThai = result.TrangThai
                    });
                }

                // 3. LOGIC LỌC THEO PHÒNG BAN
                var selectedPB = cboPhongBan.SelectedItem as PhongBan;
                IEnumerable<DepartmentPayrollItem> filteredList = fullList;

                if (selectedPB != null && selectedPB.MaPB != 0)
                {
                    // Lọc theo tên phòng ban (hoặc mã nếu bạn thêm MaPB vào DTO)
                    filteredList = fullList.Where(x => x.TenPhongBan == selectedPB.TenPB);
                }

                // 4. Gán vào View
                CollectionViewSource cvs = (CollectionViewSource)this.Resources["cvsPayroll"];
                cvs.Source = filteredList; // Gán list đã lọc
                dgPayroll.ItemsSource = cvs.View;

                // 5. Tính tổng Footer (Chỉ tính trên danh sách đã lọc)
                decimal total = filteredList.Sum(x => x.LuongThucNhan);
                txtTotalSalary.Text = total.ToString("N0") + " VNĐ";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private bool ValidateInput(out int month, out int year)
        {
            month = 0; year = 0;
            if (!int.TryParse(txtMonth.Text, out month) || month < 1 || month > 12) return false;
            if (!int.TryParse(txtYear.Text, out year) || year < 2000) return false;
            return true;
        }
    }
}