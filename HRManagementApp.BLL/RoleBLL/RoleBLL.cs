using HRManagementApp.DAL;
using HRManagementApp.models;
using System.Collections.Generic;
using System.Linq;

namespace HRManagementApp.BLL
{
    public class RoleBLL
    {
        private RoleDAL dal = new RoleDAL();

        public List<RoleModel> GetRoles() => dal.GetRoles();

        public bool AddRole(RoleModel role)
        {
            if (string.IsNullOrEmpty(role.TenVaiTro)) return false;
            return dal.AddRole(role);
        }

        // Hàm logic lấy danh sách quyền kèm trạng thái Checked/Unchecked cho UI
        public List<PermissionSelectionViewModel> GetPermissionsForEdit(int maVaiTro)
        {
            var allPerms = dal.GetAllPermissions();
            var currentPermIds = dal.GetPermissionIDsByRole(maVaiTro);

            var viewModels = new List<PermissionSelectionViewModel>();

            foreach (var p in allPerms)
            {
                viewModels.Add(new PermissionSelectionViewModel
                {
                    MaQuyen = p.MaQuyen,
                    TenQuyen = p.TenQuyen,
                    MoTa = p.MoTa,
                    IsSelected = currentPermIds.Contains(p.MaQuyen) // Nếu có trong DB thì True
                });
            }
            return viewModels;
        }
        
        // Trong file RoleBLL.cs
       // Trong file RoleBLL.cs
public List<ModuleGroupViewModel> GetGroupedPermissions(int maVaiTro)
{
    var allPerms = dal.GetAllPermissions(); // Lấy tất cả quyền từ DB
    var currentPermIds = dal.GetPermissionIDsByRole(maVaiTro); // Lấy ID quyền vai trò này đang có

    var groups = new List<ModuleGroupViewModel>();

    // Gom nhóm dựa trên tiền tố trước dấu chấm (VD: "NhanVien.Xem" -> Key là "NhanVien")
    var rawGroups = allPerms.GroupBy(p => p.TenQuyen.Contains('.') ? p.TenQuyen.Split('.')[0] : "Khac");

    foreach (var g in rawGroups)
    {
        string code = g.Key;
        var moduleVM = new ModuleGroupViewModel
        {
            MaModule = code,
            TenModule = GetModuleNameDisplay(code), // Hàm đổi tên sang tiếng Việt đẹp
            DetailedPermissions = new List<PermissionItem>()
        };

        foreach (var p in g)
        {
            var item = new PermissionItem
            {
                MaQuyen = p.MaQuyen,
                TenQuyenKey = p.TenQuyen,
                MoTaHienThi = p.MoTa,
                IsSelected = currentPermIds.Contains(p.MaQuyen)
            };

            // Logic: Nếu đuôi là .Xem hoặc .TruyCap -> Là quyền chính
            if (p.TenQuyen.EndsWith(".Xem") || p.TenQuyen.EndsWith(".TruyCap"))
            {
                moduleVM.MainPermission = item;
            }
            else
            {
                moduleVM.DetailedPermissions.Add(item);
            }
        }

        // Fallback: Nếu không tìm thấy quyền Xem, lấy đại cái đầu tiên làm chính
        if (moduleVM.MainPermission == null && moduleVM.DetailedPermissions.Any())
        {
            moduleVM.MainPermission = moduleVM.DetailedPermissions[0];
            moduleVM.DetailedPermissions.RemoveAt(0);
        }

        if(moduleVM.MainPermission != null) 
            groups.Add(moduleVM);
    }

    return groups;
}

private string GetModuleNameDisplay(string code)
{
    switch (code)
    {
        case "NhanVien": return "Quản lý Hồ sơ Nhân viên";
        case "PhongBan": return "Quản lý Phòng ban & Cơ cấu";
        case "Luong": return "Quản lý Tiền lương";
        case "ChamCong": return "Quản lý Chấm công & Đơn từ";
        case "HeThong": return "Quản trị Hệ thống";
        case "DonTu": return "Quản lí đơn từ";
        case "BaoCao": return "Xem Báo Cáo";
        default: return "Chức năng khác";
    }
}
        public bool UpdatePermissions(int maVaiTro, List<int> selectedIds)
        {
            return dal.UpdateRolePermissions(maVaiTro, selectedIds);
        }

        // Trong RoleBLL.cs

        public bool UpdateRole(RoleModel role)
        {
            if (string.IsNullOrEmpty(role.TenVaiTro)) return false;
            return dal.UpdateRole(role);
        }

        // Hàm Xóa có trả về thông báo lỗi cụ thể
        public string DeleteRole(int maVaiTro)
        {
            // 1. Kiểm tra xem có ai đang dùng vai trò này không
            if (dal.CheckRoleInUse(maVaiTro))
            {
                return "Không thể xóa: Vai trò này đang được gán cho nhân viên. Vui lòng gỡ bỏ phân quyền trước.";
            }

            // 2. Nếu không ai dùng -> Xóa
            if (dal.DeleteRole(maVaiTro))
            {
                return "Success";
            }
            else
            {
                return "Lỗi hệ thống không thể xóa.";
            }
        }
    }
}