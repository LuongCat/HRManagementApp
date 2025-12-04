using System.Collections.Generic;

namespace HRManagementApp.models{    
    public class PermissionSelectionViewModel
    {
        public int MaQuyen { get; set; }
        public string TenQuyen { get; set; }
        public string MoTa { get; set; }
        public bool IsSelected { get; set; } // Trạng thái chọn
    }
}