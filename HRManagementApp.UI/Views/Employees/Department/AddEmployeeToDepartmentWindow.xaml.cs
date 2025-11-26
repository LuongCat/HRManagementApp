using System.Windows;
using HRManagementApp.models;
using System.Windows.Controls;
using HRManagementApp.BLL;
namespace HRManagementApp.UI.Views
{
    /// Window thêm nhân viên vào phòng ban
    public partial class AddEmployeeToDepartmentWindow : Window
{
    private string _maPhongBan;
    private string _tenPhongBan;
    private readonly ChucVuService _chucVuService;

    // Lưu danh sách gốc
    private List<NhanVien> _allEmployees = new();

    public AddEmployeeToDepartmentWindow()
    {
        InitializeComponent();
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
    private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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

    private void CbFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        // TODO: Implement filter logic
    }

    // Chọn/bỏ chọn tất cả
    private void ChkSelectAll_Click(object sender, RoutedEventArgs e)
    {
       
    }


    // Cập nhật số lượng nhân viên đã chọn
    private void UpdateSelectedCount()
    {
        int count = 0;

        foreach (var item in EmployeesDataGrid.Items)
        {
            if (item is NhanVien nv && nv.IsSelected)
                count++;
        }

        TxtSelectedCount.Text = $"Đã chọn: {count} nhân viên";
    }
    
    public List<NhanVien> GetSelectedEmployees()
    {
        return EmployeesDataGrid.Items
            .OfType<NhanVien>()
            .Where(nv => nv.IsSelected)
            .ToList();
    }
    
    
    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var selectedEmployees = GetSelectedEmployees();

        if (selectedEmployees.Count == 0)
        {
            MessageBox.Show("Bạn chưa chọn nhân viên nào!",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // TODO: Gửi selectedEmployees cho service để thêm vào phòng ban

        MessageBox.Show($"Đã chọn {selectedEmployees.Count} nhân viên để thêm.",
            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
    }


    
    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }
}

}