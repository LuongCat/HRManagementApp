namespace HRManagementApp.models
{
    public class ApprovalModel
    {
        public int MaDon { get; set; }
        public string Employee { get; set; }
        public string Date { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }

        // Các trường bổ sung cho chi tiết
        public string LoaiDon { get; set; } // Loại đơn (Nghỉ phép, công tác...)
        public string NguoiDuyet { get; set; } // Tên người duyệt (nếu đã duyệt)
        public DateTime NgayGui { get; set; } // Ngày gửi đơn
    }
}