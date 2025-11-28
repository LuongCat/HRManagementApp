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