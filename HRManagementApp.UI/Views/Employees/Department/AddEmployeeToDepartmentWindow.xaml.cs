using System.Windows;
using HRManagementApp.models;
using System.Windows.Controls;
using HRManagementApp.BLL;

namespace HRManagementApp.UI.Views
{
    public partial class AddEmployeeToDepartmentWindow : Window
    {
        private string _maPhongBan;
        private string _tenPhongBan;
        private readonly ChucVuService _chucVuService;
        private readonly VaiTroNhanVienService _vaiTroNhanVienService;
        public List<ChucVu> ChucVuList { get; set; }

        private List<NhanVien> _allEmployees = new();

        public AddEmployeeToDepartmentWindow()
        {
            InitializeComponent();
            _chucVuService = new ChucVuService();
            _vaiTroNhanVienService = new VaiTroNhanVienService();
            ChucVuList = _chucVuService.GetAllChucVu();
            this.DataContext = this;
        }

        public void SetDepartmentInfo(string maPhongBan, string tenPhongBan)
        {
            _maPhongBan = maPhongBan;
            _tenPhongBan = tenPhongBan;
            TxtDepartmentName.Text = tenPhongBan;
        }

        public void LoadEmployees(List<NhanVien> employeesList)
        {
            _allEmployees = employeesList;
            EmployeesDataGrid.ItemsSource = _allEmployees;
            UpdateSelectedCount();
        }

        // ============================
        // TÌM KIẾM NHÂN VIÊN THEO TÊN
        // ============================
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TxtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                EmployeesDataGrid.ItemsSource = _allEmployees;
            }
            else
            {
                var filtered = _allEmployees
                    .Where(nv => !string.IsNullOrEmpty(nv.HoTen) &&
                                 nv.HoTen.ToLower().Contains(searchText))
                    .ToList();
                EmployeesDataGrid.ItemsSource = filtered;
            }

            UpdateSelectedCount();
        }

        // ============================
        // CẬP NHẬT SỐ LƯỢNG ĐÃ CHỌN
        // ============================
        private void UpdateSelectedCount()
        {
            int count = _allEmployees.Count(nv => nv.IsSelected);
            TxtSelectedCount.Text = $"Đã chọn: {count} nhân viên";
        }

        // ========================================
        // ✨ LẤY DANH SÁCH NHÂN VIÊN ĐÃ CHỌN
        // ========================================


        /// Lấy danh sách nhân viên đã check 
        private List<NhanVien> GetSelectedEmployees()
        {
            return _allEmployees.Where(nv => nv.IsSelected).ToList();
        }

        /// <summary>
        /// Lấy chỉ MaNV và MaChucVuMoi (nếu cần gọi service)
        /// </summary>
        private Dictionary<int, int> GetSelectedEmployeesData()
        {
            var result = new Dictionary<int, int>();

            foreach (var nv in _allEmployees.Where(nv => nv.IsSelected))
            {
                result.Add(nv.MaNV, nv.MaChucVuMoi);
            }

            return result;
        }

        // ============================
        // THÊM NHÂN VIÊN VÀO PHÒNG BAN
        // ============================
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // 1. Lấy danh sách đã chọn
            var selectedEmployees = GetSelectedEmployees();

            // 2. Validate: Kiểm tra có chọn không
            if (selectedEmployees.Count == 0)
            {
                MessageBox.Show("Bạn chưa chọn nhân viên nào!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Validate: Kiểm tra đã chọn chức vụ chưa
            var employeesWithoutPosition =
                selectedEmployees.Where(nv => nv.MaChucVuMoi == null || nv.MaChucVuMoi == 0).ToList();

            if (employeesWithoutPosition.Count > 0)
            {
                MessageBox.Show(
                    $"Vui lòng chọn chức vụ cho:\n{string.Join("\n", employeesWithoutPosition.Select(e => $"- {e.HoTen}"))}",
                    "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 4. Hiển thị xác nhận
            var message =
                $"Bạn có chắc muốn thêm {selectedEmployees.Count} nhân viên vào phòng ban {_tenPhongBan}?\n\n" +
                string.Join("\n", selectedEmployees.Select(e => $"- {e.HoTen}"));

            var result = MessageBox.Show(message, "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 5. Gọi service để cập nhật
                int maPB = int.Parse(_maPhongBan); // nếu đã là int

                foreach (var nv in selectedEmployees)
                {
                    VaiTroNhanVien vtnv = new VaiTroNhanVien
                    {
                        MaNV = nv.MaNV,
                        MaCV = nv.MaChucVuMoi,
                        MaPB = maPB,
                        LoaiChucVu = "Kiêm nhiệm",
                        HeSoPhuCapKiemNhiem = nv.HeSoKiemNhiem
                    };
                    _vaiTroNhanVienService.InsertVaiTroNhanVien(vtnv);

                    Console.WriteLine(
                        $"{_maPhongBan} - {nv.HoTen} (MaNV: {nv.MaNV}, ChucVu: {nv.MaChucVuMoi}) , He So: {nv.HeSoKiemNhiem}");
                }

                MessageBox.Show($"Đã thêm {selectedEmployees.Count} nhân viên thành công!",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}