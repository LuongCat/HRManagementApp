namespace HRManagementApp.models
{
    public static class UserSession
    {
        // Thông tin người dùng hiện tại đang đăng nhập
        public static int CurrentUserID { get; set; }       // MaTK hoặc MaNV
        public static string CurrentUserName { get; set; }  // Tên hiển thị (VD: Nguyễn Văn A)
        public static int CurrentRoleID { get; set; }       // 1: Admin, 2: Quản lý, 3: Nhân viên
        
        // Hàm xóa session khi đăng xuất
        public static void Clear()
        {
            CurrentUserID = 0;
            CurrentUserName = string.Empty;
            CurrentRoleID = 0;
        }
    }
}