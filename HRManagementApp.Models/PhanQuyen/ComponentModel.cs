using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HRManagementApp.models
{
    // 1. Đại diện cho một quyền lẻ (checkbox con)
    public class PermissionItem : INotifyPropertyChanged
    {
        public int MaQuyen { get; set; }
        public string TenQuyenKey { get; set; } // VD: NhanVien.Them
        public string MoTaHienThi { get; set; } // VD: Thêm mới nhân viên

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // 2. Đại diện cho một dòng Module lớn (VD: Quản lý Nhân sự)
    public class ModuleGroupViewModel
    {
        public string TenModule { get; set; } // VD: Quản lý Nhân sự
        public string MaModule { get; set; }  // VD: NhanVien

        // Checkbox ngoài cùng: Chỉ gán quyền XEM (NhanVien.Xem)
        public PermissionItem MainPermission { get; set; }

        // Danh sách quyền con bên trong (Thêm, Sửa, Xóa, Excel...)
        public List<PermissionItem> DetailedPermissions { get; set; } = new List<PermissionItem>();
        
        // Helper để hiển thị số lượng quyền phụ đã chọn
        public string SummaryText => $"{DetailedPermissions.Count(x => x.IsSelected)} quyền phụ";
    }
}