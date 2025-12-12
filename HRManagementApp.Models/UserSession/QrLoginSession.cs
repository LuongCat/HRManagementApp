// File: QrLoginSession.cs
namespace HRManagementApp.Models
{
    public class QrLoginSession
    {
        public string SessionId { get; set; }
        public bool IsConfirmed { get; set; }
        public string Username { get; set; } // Nếu muốn biết ai đăng nhập
        public DateTime CreatedTime { get; set; }
    }
}