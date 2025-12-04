using System;

namespace HRManagementApp.models
{
    // Map với bảng `chamcong` trong database


    // Model dùng cho phần thống kê
    public class ChamCongStats
    {
        public int SoNgayDiLam { get; set; }
        public int DiemDiTre { get; set; }
        public double TongGioLam { get; set; }
    }
}