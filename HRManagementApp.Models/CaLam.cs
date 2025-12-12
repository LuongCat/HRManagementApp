using System;
using System.ComponentModel.DataAnnotations; // Nếu bạn muốn dùng DataAnnotations
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagementApp.models
{
    [Table("calam")] // Mapping tên bảng (tùy chọn nếu dùng Entity Framework/Dapper)
    public class CaLam
    {
        [Key]
        public int MaCa { get; set; }

        public string TenCa { get; set; }

        // MySQL TIME map sang C# TimeSpan
        public TimeSpan GioBatDau { get; set; }

        public TimeSpan GioKetThuc { get; set; }

        // Enum MySQL map sang String cho dễ xử lý ("Hoạt động", "Tạm ngừng", "Đã xóa")
        public string TrangThai { get; set; } 

        // --- (Tùy chọn) Property phụ để hiển thị lên UI cho đẹp ---
        
        // Ví dụ: Hiển thị dạng "08:00 - 17:00"
        [NotMapped] // Đánh dấu để ORM không map cột này vào DB
        public string ThoiGianDisplay 
        { 
            get 
            {
                // Định dạng hh:mm (bỏ giây)
                return $"{GioBatDau:hh\\:mm} - {GioKetThuc:hh\\:mm}"; 
            } 
        }
        
        // Constructor mặc định khởi tạo giá trị
        public CaLam()
        {
            TrangThai = "Hoạt động";
            GioBatDau = TimeSpan.Zero;
            GioKetThuc = TimeSpan.Zero;
        }
    }
}   